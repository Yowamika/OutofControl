// 分離処理を行うクラス
// 2020/11/01
// 佐竹晴登

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CubeSprit : MonoBehaviour
{
    // ------------------------------------------------
    // 分離確認をする関数
    // root 親のオブジェクト
    public void CheckSplit(GameObject root)
    {
        List<GameObject> children = new List<GameObject>();
        // 子オブジェクトを全て取得
        foreach(Transform c in root.transform)

        {

            children.Add(c.transform.gameObject);

        }
        // 6方向に隣接しているオブジェクトを取得
    }
}
