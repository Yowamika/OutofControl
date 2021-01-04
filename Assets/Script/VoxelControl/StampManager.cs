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
    // スタンプの名前リスト
    public List<string> stampNameList;
    // スタンプのマテリアル番号リスト

    // Start is called before the first frame update
    void Start()
    {
        // フォルダパス
        string path = Application.dataPath + "/Prefab/VoxelControl/Stamps/";

        // フォルダ内にあるファイルを全て読み込む
        string[] files = Directory.GetFiles(path, "*.csv", System.IO.SearchOption.AllDirectories);
        // 読み込んだフォルダを元にPrefabを作成する
        for (int i = 0; i < files.Length; i++)
        {

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // ObjectGeneratorからのコピペ こいつライブラリ的なのにぶち込みたい
    /// <summary>
    /// CSVを読み込む関数
    /// </summary>
    /// <param name="path">読み込むファイルのアドレス</param>
    /// <param name="skipfirst">1行目をスキップするかどうか</param>
    /// <returns></returns>
    List<string[]> LoadCSV(string path, bool skipfirst = false)
    {
        // 読み込みたいCSVファイルのパスを指定して開く(Shift-JISではビルドが通らなかった）
        StreamReader sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
        // 戻り値
        List<string[]> list = new List<string[]>();
        // 一行読み飛ばす
        sr.ReadLine();
        // 末尾まで繰り返す
        while (!sr.EndOfStream)
        {
            // CSVファイルの一行を読み込む
            string line = sr.ReadLine();
            // 読み込んだ一行をカンマ毎に分けて配列に格納する
            string[] values = line.Split(',');
            // 配列からリストに格納する
            list.Add(values);
        }
        return list;
    }
}
