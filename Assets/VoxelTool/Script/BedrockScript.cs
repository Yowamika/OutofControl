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

    private bool flag=false;

    void Update()
    {
        if(flag)
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
            flag = false;
        }
    }


    /// <summary>
    /// 状態遷移用
    /// 岩盤サイズ変更モード
    /// </summary>
    public void ChangeSize()
    {
        // tabキーで選んでるinputFieldを変更したりモード終了したり
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //                                                                  今選択してるUIが取れる↓
            GameObject selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
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

    /// <summary>
    /// 確認パネル表示
    /// </summary>
    public void OnClickButtonBedrock()
    {
        panel.SetActive(true);
    }

    /// <summary>
    /// 確認パネルのはい
    /// 原点のブロックを変更　フィールドリセット
    /// </summary>
    public void OnClickCheckButtonYesInPanel()
    {
        panel.SetActive(false);
        flag = true;
        //holder.ResetCubeList();
        //if (sizeButton.gameObject.activeSelf)
        //{
        //    sizeButton.gameObject.SetActive(false);
        //    bedrock.SetActive(false);
        //    GameObject c = Instantiate(holder.objList[0], Vector3.zero, Quaternion.identity);
        //    holder.AddCubeList(c, 0);
        //}
        //else
        //{
        //    sizeButton.gameObject.SetActive(true);
        //    bedrock.SetActive(true);
        //}
    }

    /// <summary>
    /// 確認パネルのいいえ
    /// パネル消えるくらい
    /// </summary>
    public void OnClickCheckButtonNoInPanel()
    {
        panel.SetActive(false);
    }



    /// <summary>
    /// 岩盤サイズ変更モードに移行
    /// </summary>
    public void OnClickButtonChangeSize()
    {
        fModeChangeSize = !fModeChangeSize;
        if(fModeChangeSize)
        {
            // モード変更、inputField表示、text更新、Xを最初に選ぶようにしておく
            // text更新は一応
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
            // モード戻す、inputfield非表示
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


    /// <summary>
    /// inputfieldXの数字変更されたとき
    /// </summary>
    public void ValueChangeX()
    {
        int siz = int.Parse(fieldX.text);
        if(siz <= 0)
        {
            siz = 1;
        }
        bedrock.transform.localScale = new Vector3(siz, 1, bedrock.transform.localScale.z);
        bedrock.transform.position = new Vector3((float)(siz-1) / 2, 0, bedrock.transform.position.z);
    }



    /// <summary>
    /// inputfieldXの数字変更されたとき
    /// </summary>
    public void ValueChangeZ()
    {
        int siz = int.Parse(fieldZ.text);
        if (siz <= 0)
        {
            siz = 1;
        }
        bedrock.transform.localScale = new Vector3(bedrock.transform.localScale.x, 1, siz);
        bedrock.transform.position = new Vector3(bedrock.transform.position.x, 0, (float)(siz - 1) / 2);
    }
}
