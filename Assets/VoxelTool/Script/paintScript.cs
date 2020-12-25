using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class paintScript : MonoBehaviour
{
    // holder
    public cubeHolder holder;

    // 一個前に選んでたやつ格納
    private Material prevMaterial;
    private GameObject prevCube;

    // Start is called before the first frame update
    void Start()
    {
        prevCube = null;
        prevMaterial = null;
    }


    /// <summary>
    /// 状態遷移用
    /// ペイントモード
    /// </summary>
    public void Paint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            bool b = IsContainsObj(hit.transform.parent.gameObject);
            if (hit.transform.root != hit.transform.parent)
            {
                // 選んだのがスタンプだった場合
                // prevをなかったことにする
                if(prevCube)
                {
                    prevCube.GetComponentInChildren<Renderer>().material = prevMaterial;
                    prevCube = null;

                }
            }
            else if (prevCube && prevCube != hit.transform.parent.gameObject && b)
            {
                // 既にprevが存在してる場合
                // prevの更新
                prevCube.GetComponentInChildren<Renderer>().material = prevMaterial;
                prevCube = hit.transform.parent.gameObject;
                prevMaterial = new Material(prevCube.GetComponentInChildren<Renderer>().material);
            }
            else if (!prevCube && b)
            {
                // prevなかった＆塗れるオブジェクト選んでた場合
                // prev初期化
                prevCube = hit.transform.parent.gameObject;
                prevMaterial = new Material(prevCube.GetComponentInChildren<Renderer>().material);
            }
            else if (prevCube && !b)
            {
                // prevとは別の名前のオブジェクトを選んでいた場合
                // prevをなかったことにする
                prevCube.GetComponentInChildren<Renderer>().material = prevMaterial;
                prevCube = null;
            }

            //Debug.Log(prevMaterial.name);
            if(prevCube)
            {
                // prevCubeをドロップダウン選択中のマテリアルで塗る
                // [オブジェクト番号][マテリアル番号]
                prevCube.GetComponentInChildren<Renderer>().material = holder.materialList[holder.GetMaterialMultipleNum(holder.GetObjectNum(prevCube))][holder.matDropdown.value];
                if (Input.GetMouseButtonUp(0))
                {
                    // prevのマテリアルを確定させる
                    // cubeMatListを更新
                    // prevをnullにする
                    int lNum = holder.cubeList.IndexOf(prevCube);
                    holder.cubeMatList[lNum] = holder.matDropdown.value;
                    Destroy(prevMaterial);
                    prevCube = null;
                }
            }
        }
        else
        {
            // 何とも当たってない場合
            if(prevCube)
            {
                // prevをなかったことにする
                prevCube.GetComponentInChildren<Renderer>().material = prevMaterial;
            }
            prevCube = null;
        }
    }

                                                                  
    /// <summary>
    /// 今選択してるやつ(引数のGameObject)が今ドロップダウンで選択してるオブジェクトと合ってるか
    /// 複数オブジェクト対応
    /// </summary>
    /// <param name="obj">Rayのやつ</param>
    /// <returns></returns>
    bool IsContainsObj(GameObject obj)
    {
        if(holder.objList[holder.objDropdown.value])
        {
            // objListの選択してるやつがobjの名前だったらtrue
            if(obj.name.Contains(holder.objList[holder.objDropdown.value].name))
            {
                return true;
            }
            return false;
        }
        else
        {
            List<GameObject> list = holder.GetMultipleMaterialList(holder.objDropdown.value);
            for (int i = 0; i < list.Count; i++) 
            {
                // 取得した複数オブジェクトリストの中にobjの名前があったらtrue
                if (obj.name.Contains(list[i].name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
