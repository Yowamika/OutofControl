using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeJointScript : MonoBehaviour
{
    // 繋げるやつ見せる用
    [SerializeField]
    List<GameObject> fadeList;
    [SerializeField]
    List<GameObject> fadeGentleList;
    private GameObject fade;
    // キューブ収納
    public cubeHolder holder;
    // 消去モード用に今マウスが乗ってるオブジェクトを保存しておくやつ
    private GameObject selectDelete;

    private int fadeRotateNum = 0;
    private List<Vector3> rotList;

    // Start is called before the first frame update
    void Start()
    {
        selectDelete = null;
        rotList = new List<Vector3>();
        rotList.Add(new Vector3(90, 0, 0));
        rotList.Add(new Vector3(270, 0, 0));
        rotList.Add(new Vector3(0, 90, 0));
        rotList.Add(new Vector3(0, 270, 0));
    }

    // -----------------------------------------------
    //
    // 状態遷移から呼び出す用
    // 要するにUpdate　キー入力を読み取る
    public void Form()
    {
        fade = fadeList[holder.objDropdown.value];
        for (int i = 0; i < fadeList.Count; i++)
        {
            if (fadeList[i] != fade)
                fadeList[i].SetActive(false);
            else
                fadeList[i].SetActive(true);
        }
        fadeGentleList[0].SetActive(false);
        fadeGentleList[1].SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (holder.objDropdown.value == 3)
            {
                fade.SetActive(false);
                fade = fadeGentleList[GetGentleSlopeNum(hit)];
                fade.SetActive(true);
            }
            if (Input.GetKey(KeyCode.LeftShift) == false)
            {
                JointBlock(hit);
            }
            else
            {
                fade.transform.position = Vector3.zero;
                if (selectDelete && selectDelete != hit.transform.parent.gameObject)
                {
                    selectDelete.GetComponentInChildren<Renderer>().material.color = Color.white;
                }
                if (hit.transform.root.name != "bedrock")
                    selectDelete = hit.transform.parent.gameObject;
                else
                    selectDelete = null;
                if(selectDelete)
                    DeleteBlock();
                fade.SetActive(false);
            }
        }
        else
        {
            fade.transform.position = Vector3.zero;
            fade.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && selectDelete)
        {
            selectDelete.GetComponentInChildren<Renderer>().material.color = Color.white;
        }
    }

    // -----------------------------------------------
    //
    // Shiftキーを押してないときに呼び出す用
    // ブロックを繋げる
    void JointBlock(RaycastHit hit)
    {
        if (holder.objDropdown.value != 0)
        {
            fade.transform.localEulerAngles = holder.rotationList[fadeRotateNum];
            if (Input.GetMouseButtonUp(1) && fade.transform.position != Vector3.zero)
            {
                fadeRotateNum++;
                if (fadeRotateNum >= 12) 
                {
                    fadeRotateNum -= 12;
                }
            }
        }
        fade.transform.position = GetFadePos(hit);
        if (Input.GetMouseButtonUp(0) && fade.transform.position != Vector3.zero)
        {
            // 置く
            GameObject original = null;
            //if (holder.objDropdown.value == 3)
            if (holder.objList[holder.objDropdown.value] == null)
            {
                int h = GetGentleSlopeNum(hit);
                original = holder.multipleObjList[0].list[h];
            }
            else
            {
                original = holder.objList[holder.objDropdown.value];
            }
            GameObject c = Instantiate(original, fade.transform.position, Quaternion.identity);

            if (holder.objDropdown.value != 0)
            {
                c.transform.GetChild(0).localEulerAngles = holder.rotationList[fadeRotateNum];
            }
            holder.AddCubeList(c);
            fade.transform.position = Vector3.zero;
        }
    }

    // -----------------------------------------------
    //
    // Shiftキーを押してるときに呼び出す用
    // ブロックを消す(原点のは無理)
    void DeleteBlock()
    {
        selectDelete.GetComponentInChildren<Renderer>().material.color = Color.red;

        if (Input.GetMouseButtonUp(0))
        {
            if (selectDelete.transform.position != Vector3.zero) 
            {
                holder.DeleteCubeList(selectDelete);
                selectDelete = null;
                fade.SetActive(false);
            }
        }
    }

    // -----------------------------------------------
    //
    // 次に置くゆるやか坂がどっちか求める
    // Qを押すと低い坂、Eを押すと高い坂を手動で出す
    int GetGentleSlopeNum(RaycastHit hit)
    {
        if(Input.GetKey(KeyCode.Q))
        {
            return 0;
        }
        else if(Input.GetKey(KeyCode.E))
        {
            return 1;
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 pos;
            // 置くブロックの角度と各方向をかける　rotListは無回転の時の各Quadを基にしている
            Quaternion rot = Quaternion.Euler(holder.rotationList[fadeRotateNum]) * Quaternion.Euler(rotList[i]);
            // さっきの角度になんか方向をかける　これで大きさ１の方向ベクトル的なのがとれる
            pos = rot * Vector3.back;
            // さっきの位置に現在の置くブロックの位置を足す　これで隣接ブロックのpositionがわかる
            pos += GetFadePos(hit);
            // 隣接ブロックを入手
            GameObject obj = holder.GetHitCube(pos);
            if (obj)
            {
                // bがtrueだと向きが隣接ブロックと一緒
                bool b = false;
                float f = Quaternion.Angle(Quaternion.Euler(obj.transform.GetChild(0).localEulerAngles), Quaternion.Euler(holder.rotationList[fadeRotateNum]));
                if (f % 360 == 0)
                {
                    // quaternion.angle :２つの角度の差を求める　
                    // 普通にVector3で比較しようとすると-90と270を一緒だと認めてくれない
                    b = true;
                }

                switch (i)
                {
                case 0:
                        if(obj.name.Contains("cube"))
                        {
                            return 1;
                        }
                        if(obj.name.Contains("TallSlope") && b)
                        {
                            return 0;
                        }
                    break;
                case 1:
                        if(obj.name.Contains("ShortSlope") && b)
                        {
                            return 1;
                        }
                    break;
                case 2:
                case 3:
                        if (obj.name.Contains("ShortSlope") && b)
                        {
                            return 0;
                        }
                        if (obj.name.Contains("TallSlope") && b)
                        {
                            return 1;
                        }
                        break;
                    default:

                    break;
                }
            }
        }
        return 0;
    }

    // -----------------------------------------------
    //
    // 次に置くオブジェクトの位置を求める
    Vector3 GetFadePos(RaycastHit hit)
    {
        Vector3 pos = new Vector3();
        if (hit.transform.parent.name == "bedrock")
        {
            pos = new Vector3(Mathf.Ceil(hit.point.x - 0.5f), Mathf.Ceil(hit.point.y), Mathf.Ceil(hit.point.z - 0.5f));
        }
        else
        {
            pos = hit.transform.parent.position + hit.transform.localPosition * 2;
        }

        return pos;
    }
}
