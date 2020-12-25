using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeJointScript : MonoBehaviour
{
    // 繋げるやつ見せる用
    [SerializeField]
    List<GameObject> fadeList = new List<GameObject>();
    [SerializeField]
    List<GameObject> fadeGentleList = new List<GameObject>();
    private GameObject fade;
    // キューブ収納
    public cubeHolder holder;
    // 消去モード用に今マウスが乗ってるオブジェクトを保存しておくやつ
    private GameObject selectDelete;

    // 回転のやつ
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

        ChangeFade();
    }

    /// <summary>
    /// 状態遷移から呼び出す用
    /// </summary>
    public void Form()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            // 緩やか坂を選んでた場合　fadeを変更
            if (holder.objDropdown.value == 3)
            {
                fade.SetActive(false);
                fade = fadeGentleList[GetGentleSlopeNum(hit)];
            }
            fade.SetActive(true);

            // 左Shift押してると消すやつ　押してないと付けるやつ
            if (Input.GetKey(KeyCode.LeftShift) == false)
            {
                JointBlock(hit);
            }
            else
            {
                // 消す時fadeいらないので原点に置いとく
                fade.transform.position = Vector3.zero;
                if (selectDelete && selectDelete != hit.transform.parent.gameObject)
                {
                    // 一個前に選んでたオブジェクトが今選択してるやつと違う場合
                    // 一個前のやつの色を元に戻しておく
                    if (selectDelete.transform.root != selectDelete)
                    {
                        // スタンプ対策
                        var childConponents = selectDelete.transform.root.GetComponentsInChildren<Renderer>();

                        foreach (var childRenderer in childConponents)
                        {
                            childRenderer.GetComponentInChildren<Renderer>().material.color = Color.white;
                        }
                    }
                    else
                        selectDelete.GetComponentInChildren<Renderer>().material.color = Color.white;
                }
                if (hit.transform.root.name != "bedrock")
                {
                    // 岩盤じゃないのでselectDeleteを変更
                    selectDelete = hit.transform.parent.gameObject;
                }
                else
                {
                    // 岩盤を無視
                    selectDelete = null;
                }
                if(selectDelete)
                {
                    // selectDeleteの処遇を決める
                    DeleteBlock();
                }
                // fade非表示
                fade.SetActive(false);
            }
        }
        else
        {
            // 何とも当たってない場合
            // fadeを非表示
            fade.transform.position = Vector3.zero;
            fade.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && selectDelete)
        {
            // selectDeleteを元に戻す
            if (selectDelete.transform.root != selectDelete)
            {
                // スタンプ対策
                var childConponents = selectDelete.transform.root.GetComponentsInChildren<Renderer>();

                foreach (var childRenderer in childConponents)
                {
                    childRenderer.GetComponentInChildren<Renderer>().material.color = Color.white;
                }
            }
            else
                selectDelete.GetComponentInChildren<Renderer>().material.color = Color.white;
        }
    }

    /// <summary>
    /// Shiftキーを押してないときに呼び出す用
    /// ブロックを繋げる
    /// </summary>
    /// <param name="hit">Rayのやつ</param>
    void JointBlock(RaycastHit hit)
    {
        //if (holder.objDropdown.value != 0)
        if(!fade.name.Contains("cube"))
        {
            // 右クリックで回転　cubeだと回転の意味ないので省略
            fade.transform.localEulerAngles = holder.rotationList[fadeRotateNum];
            if (Input.GetMouseButtonUp(1) && fade.transform.position != Vector3.zero)
            {
                fadeRotateNum++;
                if (fadeRotateNum >= 12) 
                {
                    // 12超えたら0に戻す
                    fadeRotateNum -= 12;
                }
            }
        }

        // fade表示位置を取得
        fade.transform.position = holder.GetTargetPos(hit);
        if (Input.GetMouseButtonUp(0) && fade.transform.position != Vector3.zero)
        {
            // この中身置く処理

            GameObject original = null;
            if (holder.objList[holder.objDropdown.value] == null)
            {
                // どっちの緩やか坂使うかチェック
                int h = GetGentleSlopeNum(hit);
                original = holder.multipleObjList[0].list[h];
            }
            else
            {
                original = holder.objList[holder.objDropdown.value];
            }
            GameObject obj = Instantiate(original, fade.transform.position, Quaternion.identity);
            if (!fade.name.Contains("cube"))
                obj.transform.GetChild(0).localEulerAngles = holder.rotationList[fadeRotateNum];

            // listに追加
            holder.AddCubeList(obj);
            // fadeを戻す
            fade.transform.position = Vector3.zero;
            fade.SetActive(false);
        }
    }


    /// <summary>
    /// Shiftキーを押してるときに呼び出す用
    /// ブロックを消す(原点のは無理)
    /// </summary>
    void DeleteBlock()
    {
        if (selectDelete.transform.root != selectDelete)
        {
            // 選んだのがスタンプの時
            // selectDeleteの中身を全部赤くする
            var childConponents = selectDelete.transform.root.GetComponentsInChildren<Renderer>();

            foreach (var childRenderer in childConponents)
            {
                childRenderer.GetComponentInChildren<Renderer>().material.color = Color.red;
            }
        }
        else
        {
            // 今選択してるやつを赤く表示
            selectDelete.GetComponentInChildren<Renderer>().material.color = Color.red;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (selectDelete.transform.position != Vector3.zero) 
            {
                // 原点のブロックじゃなかったら消す
                // holderのlistから消す、selectDeleteなかったことにする
                holder.DeleteCubeList(selectDelete);
                selectDelete = null;
            }
        }
    }


    /// <summary>
    /// 次に置くゆるやか坂がどっちか求める
    /// Qを押すと低い坂、Eを押すと高い坂を手動で出す
    /// </summary>
    /// <param name="hit">Rayのやつ</param>
    /// <returns></returns>
    int GetGentleSlopeNum(RaycastHit hit)
    {
        // キー入力優先
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
            pos += holder.GetTargetPos(hit);
            // 隣接ブロックを入手
            GameObject obj = holder.GetHitCube(pos);
            if (obj)
            {
                // bがtrueだと向きが隣接ブロックと一緒
                bool b = false;
                float f = Quaternion.Angle(Quaternion.Euler(obj.transform.GetChild(0).localEulerAngles), Quaternion.Euler(holder.rotationList[fadeRotateNum]));
                if (f % 360 == 0)
                {
                    // quaternion.angle :２つの角度(quaternion)の差を求める　
                    // 普通にVector3で比較しようとすると-90と270を一緒だと認めてくれない
                    b = true;
                }

                // 前後左右見て特定のオブジェクトだったらreturn
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
        // 特定のオブジェクトがなかったら低い方を返す
        return 0;
    }





    /// <summary>
    /// fadeどれにするか取得
    /// </summary>
    public void ChangeFade()
    {
        // ※fadeが緩やか坂だった場合positionの取りようがないのであとから変更
        fade = fadeList[holder.objDropdown.value];
        for (int i = 0; i < fadeList.Count; i++)
        {
            // 今使ってないfadeを非表示
            if (fadeList[i] != fade)
                fadeList[i].SetActive(false);
            else
                fadeList[i].SetActive(true);
        }
        // 緩やか坂fadeを非表示
        fadeGentleList[0].SetActive(false);
        fadeGentleList[1].SetActive(false);
    }
}
