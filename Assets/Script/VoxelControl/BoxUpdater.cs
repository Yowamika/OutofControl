// BoxのUpdateマネージャー
// 2020/09/18
// 佐竹晴登

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxUpdater : MonoBehaviour
{
    // boxのリスト
    List<Cube> boxList = new List<Cube>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var v in boxList)
        {
            v.UpdateMe();
        }
    }

    public void AddUpdater(Cube v)
    {
        boxList.Add(v);
    }
    public void DeleteUpdater(Cube v)
    {
        boxList.Remove(v);
    }
}
