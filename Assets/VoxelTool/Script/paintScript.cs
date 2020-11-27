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


    public void Paint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            bool b = IsContainsObj(hit.transform.parent.gameObject);
            if (prevCube && prevCube != hit.transform.parent.gameObject && b)
            {
                prevCube.GetComponentInChildren<Renderer>().material = prevMaterial;
                prevCube = hit.transform.parent.gameObject;
                prevMaterial = new Material(prevCube.GetComponentInChildren<Renderer>().material);
            }
            else if (!prevCube)
            {
                if (b)
                {
                    prevCube = hit.transform.parent.gameObject;
                    prevMaterial = new Material(prevCube.GetComponentInChildren<Renderer>().material);
                }
            }
            else if (prevCube && !b)
            {
                prevCube.GetComponentInChildren<Renderer>().material = prevMaterial;
                prevCube = null;
            }

            //Debug.Log(prevMaterial.name);
            if(prevCube)
            {
                prevCube.GetComponentInChildren<Renderer>().material = holder.materialList[holder.GetMaterialMultipleNum(holder.GetObjectNum(prevCube))][holder.matDropdown.value];
                if (Input.GetMouseButtonUp(0))
                {
                    int lNum = holder.cubeList.IndexOf(prevCube);
                    holder.cubeMatList[lNum] = holder.matDropdown.value;
                    Destroy(prevMaterial);
                    prevCube = null;
                }
            }
        }
        else
        {
            if(prevCube)
                prevCube.GetComponentInChildren<Renderer>().material = prevMaterial;
            prevCube = null;
        }
    }

    bool IsContainsObj(GameObject obj)
    {
        if(holder.objList[holder.objDropdown.value])
        {
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
                if (obj.name.Contains(list[i].name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
