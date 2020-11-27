using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BedrockScript : MonoBehaviour
{
    public Button sizeButton;
    private bool fModeChangeSize = false;

    public GameObject bedrock;
    private Vector3 prevScale;

    public cubeHolder holder;
    public paintButtonScript state;

    public InputField fieldX;
    public InputField fieldZ;

    // メモ：パネルはImageかGameObjectで取得できる
    public GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // -----------------------------------------------
    // 状態遷移
    public void ChangeSize()
    {
        //Vector3 scale = bedrock.transform.localScale;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, 100.0f,1<<9))
        //{

        //    // メモ：切り上げかMath.Roundで伸ばす値をとる　０にはなりたくない？
        //    // どっかに一個前のlocalscaleを取っておく　１伸びたら0.5位置をずらす　
        //    //Debug.Log(new Vector3Int((int)hit.point.x, (int)hit.point.y, (int)hit.point.z));
        //}
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            //Debug.Log(selected.name);
            if(selected)
            {
                if (selected == fieldX.gameObject)
                {
                    fieldZ.Select();
                }
                else if (selected == fieldZ.gameObject)
                {
                    OnClickButtonChangeSize();
                }
            }
        }
    }

    // -----------------------------------------------
    // 確認パネル表示
    public void OnClickButtonBedrock()
    {
        panel.SetActive(true);
    }

    // -----------------------------------------------
    // 確認パネルのはい押したやつ
    // 原点のブロックを変更　フィールドリセット
    public void OnClickCheckButtonYesInPanel()
    {
        holder.ResetCubeList();
        if (sizeButton.gameObject.activeSelf)
        {
            sizeButton.gameObject.SetActive(false);
            bedrock.SetActive(false);
            GameObject c = Instantiate(holder.objList[0], Vector3.zero, Quaternion.identity);
            holder.AddCubeList(c, 0);
        }
        else
        {
            sizeButton.gameObject.SetActive(true);
            bedrock.SetActive(true);
        }
        panel.SetActive(false);
    }

    // -----------------------------------------------
    // 確認パネルいいえ
    public void OnClickCheckButtonNoInPanel()
    {
        panel.SetActive(false);
    }

    // -----------------------------------------------
    // 岩盤サイズ変更モードに移行
    public void OnClickButtonChangeSize()
    {
        fModeChangeSize = !fModeChangeSize;
        if(fModeChangeSize)
        {
            state.SetPrevMode();
            fieldX.gameObject.SetActive(true);
            fieldZ.gameObject.SetActive(true);
            fieldX.text = bedrock.transform.localScale.x.ToString();
            fieldZ.text = bedrock.transform.localScale.z.ToString();
            fieldX.Select();
            //foreach (Transform obj in bedrock.transform)
            //{
            //    obj.gameObject.SetActive(true);
            //}
        }
        else
        {
            state.EndBedrockSizeMode();
            fieldX.gameObject.SetActive(false);
            fieldZ.gameObject.SetActive(false);
            //foreach (Transform obj in bedrock.transform)
            //{
            //    obj.gameObject.SetActive(false);
            //}
            //bedrock.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void ValueChangeX()
    {
        int siz = int.Parse(fieldX.text);
        bedrock.transform.localScale = new Vector3(siz, 1, bedrock.transform.localScale.z);
        bedrock.transform.position = new Vector3((float)(siz-1) / 2, 0, bedrock.transform.position.z);
    }

    public void ValueChangeZ()
    {
        int siz = int.Parse(fieldZ.text);
        bedrock.transform.localScale = new Vector3(bedrock.transform.localScale.x, 1, siz);
        bedrock.transform.position = new Vector3(bedrock.transform.position.x, 0, (float)(siz - 1) / 2);
    }
}
