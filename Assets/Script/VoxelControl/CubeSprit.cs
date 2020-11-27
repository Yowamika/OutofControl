// 分離処理を行うクラス
// 2020/11/01
// 佐竹晴登

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CubeSprit : MonoBehaviour
{
    // オブジェクト生成スクリプト
    ObjectGenerator generator;
    // RootオブジェクトのPrefab
    [SerializeField]
    GameObject rootObject;
    private void Awake()
    {
        // オブジェクト生成スクリプトを取得
        generator = this.GetComponent<ObjectGenerator>();
    }
    // ------------------------------------------------
    // 分離確認をする関数
    // root 親のオブジェクト
    public void CheckSplit(GameObject root)
    {
        List<GameObject> children = new List<GameObject>();
        Debug.Log("きてはります？");
        // 親分離処理
        GenerateRootObject(root);
    }
    // ------------------------------------------------
    // 取得されたリストを親にしてオブジェクトを生成
    // root 親のオブジェクト
    void GenerateRootObject(GameObject root)
    {

        // オブジェクト生成
        GameObject newRoot = (GameObject)Instantiate(rootObject,
                                                        Vector3.zero,
                                                        Quaternion.identity);
        List<Cube> cubes = new List<Cube>();
        cubes.Add(root.transform.GetChild(0).GetComponent<Cube>());
        // オブジェクト分離
        ObjectSplit(root, cubes, newRoot);

    }
    // 自分の付近にあるオブジェクトをなくなるまで取得する(分離処理）
    // root 現在の親のオブジェクト
    // save 保存するリスト
    // newRoot 新しい親
    Cube[] ObjectSplit(GameObject root, List<Cube> save,GameObject newRoot)
    {
        List<Cube> cubes = new List<Cube>();
        for (int i = 0; i< save.Count;i++)
        {
            cubes.AddRange(generator.GirthCheck(root, save[i].GetComponent<Cube>()));
            for (int j = 0; j < cubes.Count; j++)
            {
                // リスト内検索
                if (!save.Contains(cubes[j]))
                {
                    cubes[j].transform.parent = newRoot.transform;
                    cubes[j].GetComponent<Rigidbody>().isKinematic = false;
                    save.Add(cubes[j]);
                }
                else
                    cubes.Remove(cubes[j]);
            }
        }
        // 終了処理
        if (cubes.Count == 0)
            return save.ToArray();
        Debug.Log("ぬあ");
        return ObjectSplit(root, save, newRoot);
    }
}
