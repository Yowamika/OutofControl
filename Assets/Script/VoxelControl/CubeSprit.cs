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

    int LoopCount = 0;
    private void Start()
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
        
        // 親分離処理
        GenerateRootObject(root);
    }
    // ------------------------------------------------
    // 取得されたリストを親にしてオブジェクトを生成
    // root 親のオブジェクト
    void GenerateRootObject(GameObject root)
    {
        while (root.transform.childCount != 0)
        {
            // オブジェクト生成
            GameObject newRoot = (GameObject)Instantiate(rootObject,
                                                     Vector3.zero,
                                                     Quaternion.identity);
            List<GameObject> cubes = new List<GameObject>();

            // 最初のサンプル
            GameObject sample;
            sample = root.transform.GetChild(0).gameObject;
            cubes.Add(sample);
            sample.transform.parent = newRoot.transform;
            // オブジェクト分離
            ObjectSplit(sample, cubes, newRoot);
        }
        
    }
    // ------------------------------------------------
    // 自分の付近にあるオブジェクトをなくなるまで取得する(分離処理）
    // cube 確認するオブジェクト
    // save 保存するリスト
    // newRoot 新しい親
    List<GameObject> ObjectSplit(GameObject cube,List<GameObject> save,GameObject newRoot)
    {
        List<GameObject> cubes = new List<GameObject>();
        // 周囲にあるオブジェクトを取得
        cubes.AddRange(GirthCheckToRay(cube));
        for (int i = 0; i < cubes.Count; i++)
        {
            Debug.Log(cubes[i].name);
            // リスト内検索
            if (!save.Contains(cubes[i]))
            {
                // 親を更新する
                cubes[i].transform.parent = newRoot.transform;
                // 保存リストに追加
                save.Add(cubes[i]);
                // 再帰
                save = ObjectSplit(cubes[i], save, newRoot);
                
            }
        }
        // 終了処理
        return save;
    }

    // レイキャストで6方向のオブジェクトを取得する
    List<GameObject> GirthCheckToRay(GameObject cube)
    {
        // 戻り値用
        List<GameObject> cubes = new List<GameObject>();
        // ベースとなるRay
        Ray ray;
        RaycastHit hit;
        // 欠片（Fragment)レイヤーを無視するための
        int layerMask = ~(1 << 9);
        // 上方向
        // 最初はリソース確保のためのnew
        ray = new Ray(cube.transform.position, cube.transform.up);
        if (Physics.Raycast(ray, out hit, 1.0f,layerMask))
            if(hit.collider.tag == "Block")
                 cubes.Add(hit.collider.gameObject);
        // 下方向
        // インスタンスを再利用する
        ray.origin = cube.transform.position;
        ray.direction = -cube.transform.up;
        if (Physics.Raycast(ray, out hit, 1.0f, layerMask))
            if (hit.collider.tag == "Block")
                cubes.Add(hit.collider.gameObject);
        // 左方向
        ray.origin = cube.transform.position;
        ray.direction = -cube.transform.right;
        if (Physics.Raycast(ray, out hit, 1.0f, layerMask))
            if (hit.collider.tag == "Block")
                cubes.Add(hit.collider.gameObject);
        // 右方向
        ray.origin = cube.transform.position;
        ray.direction = cube.transform.right;
        if (Physics.Raycast(ray, out hit, 1.0f, layerMask))
            if (hit.collider.tag == "Block")
                cubes.Add(hit.collider.gameObject);
        // 前方
        ray.origin = cube.transform.position;
        ray.direction = cube.transform.forward;
        if (Physics.Raycast(ray, out hit, 1.0f, layerMask))
            if (hit.collider.tag == "Block")
                cubes.Add(hit.collider.gameObject);
        // 後方
        ray.origin = cube.transform.position;
        ray.direction = -cube.transform.forward;
        if (Physics.Raycast(ray, out hit, 1.0f, layerMask))
            if (hit.collider.tag == "Block")
                cubes.Add(hit.collider.gameObject);

        return cubes;
    }
}
