// 2020/12/25
// 本編でスタンプを管理するスクリプト
// 佐竹晴登

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class StampManager : MonoBehaviour
{
    // ジェネレーター
    ObjectGenerator generator;
    // スタンプの名前リスト
    public List<string> stampNameList;
    // スタンプの座標リスト
    public List<List<GameObject>> stampPosList;
    // スタンプのマテリアル番号リスト
    public List<List<int>> matNumList;
    // 作成したPrefabのリスト
    public List<GameObject> prefabList;
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        generator = this.GetComponent<ObjectGenerator>();
    }
    /// <summary>
    /// 最初のフレーム
    /// </summary>
    void Start()
    {
        // フォルダパス
        string path = Application.dataPath + "/Prefab/VoxelControl/Stamps/";

        // フォルダ内にあるファイルを全て読み込む
        string[] files = Directory.GetFiles(path, "*.csv", System.IO.SearchOption.AllDirectories);
        
        // 読み込んだフォルダを元にPrefabを作成する
        for (int i = 0; i < files.Length; i++)
        {
            // 読み込んだCSVを保存するリスト
            List<string[]> data = new List<string[]>();

            stampPosList.Add(new List<GameObject>());
            matNumList.Add(new List<int>());
            // CSV読み込み
            data = generator.LoadCSV(files[i]);
            // 親用のオブジェクト
            GameObject parent = new GameObject();
            // CSVをもとにスタンプのオブジェクト群を生成
            foreach (var d in data)
            {
                // 外部読み込みでブロック生成
                GameObject obj = generator.BlockGenerate(d,true);
            }
            // Prefab作成

        }

    }

}
