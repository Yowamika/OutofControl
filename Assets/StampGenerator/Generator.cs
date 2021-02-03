using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEditor;

public class Generator : MonoBehaviour
{
    // 用意されたオブジェクトのリスト
    [SerializeField]
    List<GameObject> objList = new List<GameObject>();
    // 複数で１つのオブジェクトのリスト
    [SerializeField]
    List<MultipleList> multipleObjList = new List<MultipleList>();
    // 使用マテリアルリスト
    List<List<Material>> materialList = new List<List<Material>>();
    // 固定回転値を保存するリスト
    List<Vector3> rotationList = new List<Vector3>();
    enum ObjectType
    {
        CUBE = 0,
        SLOPE = 1,
        STAGE = 2,
        SHORTSLOPE = 8,
        LONGSLOPE = 9,
    };
    // ボックスのCSVデータの列挙体(行)
    enum BoxDataType
    {
        POS_X = 0,       // X座標
        POS_Y,           // Y座標
        POS_Z,           // Z座標
        OBJECT,          // オブジェクトID
        MATERIAL,        // マテリアルID
        ROTATION,        // 回転ID(回転度数ではない)

        SUMTYPE          // データタイプの合計
    };
    // スタンプのCSVデータの列挙体(行）
    enum StampDataType
    {
        STAMPNAME = 0,  // スタンプの名前
        POS_X,          // X座標
        POS_Y,          // Y座標
        POS_Z,          // Z座標
        ROT_X,          // X回転
        ROT_Y,          // Y回転
        ROT_Z,          // Z回転

        SUMTYPE         // データタイプ合計
    }
    // 優しい坂以上のアレ
    public const int MULTIPLE_NUM = 8;
    // ステージデータcsvの保存ファイル
    string csvDataFile = "/StreamingAssets/Data/csv/StageFile/";
    // 情報系csvの保存ファイル
    string csvInfoFile = "/StreamingAssets/Data/csv/VoxelInfo/";
    [SerializeField]
    List<string> loadMaterialStr = new List<string>();
    private void Awake()
    {
        // <マテリアルを取得>
        // 後ろにつくもの
        string csvExt = "Mat.csv";
        // 各マテリアル取得
        for (int i = 0; i < loadMaterialStr.Count; i++)
            SetMaterialList(loadMaterialStr[i], Application.dataPath + csvInfoFile + loadMaterialStr[i] + csvExt);

        // <回転の固定値を設定>
        SetRotationListInit(Application.dataPath + csvInfoFile + "rotationList.csv");

        PrefabGenerate();
    }
    /// <summary>
    /// スタンプファイルを基にプレファブを生成する関数
    /// </summary>
    void PrefabGenerate()
    {
        string path = Application.dataPath + "/VoxelTool/BoxesData/Stamps/";
        // フォルダ内のcsvを全て取得
        string[] files = Directory.GetFiles(path, "*.csv", System.IO.SearchOption.AllDirectories);
        // 取得したフォルダを元にPrefabを生成
        for(int i = 0; i < files.Length;i++)
        {
            // ファイル名そのものを取得する
            string name = System.IO.Path.GetFileNameWithoutExtension(files[i]);
            string[] search = Directory.GetFiles(Application.dataPath + "/Prefab/Stamp/",
                                                name + ".Prefab", 
                                                System.IO.SearchOption.AllDirectories);

            if(search.Length == 0)
            {
                // 取得ファイル内
                List<string[]> dataList = LoadCSV(files[i]);
                // 子用のオブジェクトリスト
                List<GameObject> ChildObject = new List<GameObject>();
                // 親となるオブジェクト
                GameObject parentObj = new GameObject();
                foreach(var d in dataList)
                {
                    // データをもとにオブジェクトを生成
                    GameObject g = BlockGenerate(d);
                    // 親を設定
                    g.transform.parent = parentObj.transform;
                    // リストに追加する
                    ChildObject.Add(g);
                }
                // Prefab作成
                var prefab = PrefabUtility.SaveAsPrefabAsset(parentObj
                    , "Assets/Prefab/Stamp/" + name + ".prefab");
                // シーンから削除
                Destroy(parentObj);
                // 作ったPrefabを保存
                AssetDatabase.SaveAssets();
            }
        }
    }

    /// <summary>
    /// CSVを読み込む関数
    /// </summary>
    /// <param name="path">読み込むファイルのアドレス</param>
    /// <param name="skipfirst">1行目をスキップするかどうか</param>
    /// <returns>string配列に入れたもの</returns>
    public List<string[]> LoadCSV(string path, bool skipfirst = false)
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

    /// <summary>
    /// CSVファイルを基にブロックを生成する
    /// </summary>
    /// <param name="data">基のデータ</param>
    /// <returns>生成したオブジェクトデータ</returns>
    public GameObject BlockGenerate(string[] data)
    {
        int[] dataInt = data.Select(int.Parse).ToArray();
        // 戻り値用オブジェクト
        GameObject returnObject = null;
        // 一時保管用
        GameObject bamp;
        // 優しい坂かそれ以外かを選別
        if (dataInt[(int)BoxDataType.OBJECT] / MULTIPLE_NUM > 0)
            bamp = multipleObjList[dataInt[(int)BoxDataType.OBJECT] / MULTIPLE_NUM - 1].list[dataInt[(int)BoxDataType.OBJECT] % MULTIPLE_NUM];
        else
            bamp = objList[dataInt[(int)BoxDataType.OBJECT]];

        // マテリアルナンバーを取得
        int m = GetMaterialNumberforMutiple(dataInt[(int)BoxDataType.OBJECT]);

        // 座標を決定
        Vector3 v = new Vector3(dataInt[(int)BoxDataType.POS_X], dataInt[(int)BoxDataType.POS_Y], dataInt[(int)BoxDataType.POS_Z]);

        // オブジェクト生成
        GameObject go = Instantiate(bamp, v, Quaternion.identity);
        // マテリアルを適用
        go.GetComponentInChildren<Renderer>().material = materialList[m][dataInt[(int)BoxDataType.MATERIAL]];
        // オブジェクト番号０(四角）以外なら
        if (dataInt[(int)BoxDataType.OBJECT] != (int)ObjectType.CUBE && dataInt[(int)BoxDataType.OBJECT] != (int)ObjectType.STAGE)
        {
            // 回転を適用
            go.transform.localEulerAngles = rotationList[dataInt[(int)BoxDataType.ROTATION]];
        }
        returnObject = go;

        return returnObject;
    }
    /// <summary>
    /// csvからマテリアルと名前読み込んで順番に配列に入れる
    /// </summary>
    /// <param name="materialType">読み込みたいフォルダの場所</param>
    /// <param name="csvPath">読み込むcsvの場所</param>
    void SetMaterialList(string materialType, string csvPath)
    {
        // <csvを読み込む>
        // CSVを読み込むリスト
        List<string[]> list = new List<string[]>();

        list = LoadCSV(csvPath);
        // <CSVに沿ってマテリアルをリストに入れる>
        // listを用意
        List<Material> listM = new List<Material>();

        listM.Add(Resources.Load<Material>("Materials/defaultmaterial"));
        // csvデータリストに沿ってマテリアルをリストに追加
        foreach (var data in list)
        {
            //// 一行目はスキップする
            //if (data[0] == "name") 
            //    continue;

            listM.Add(Resources.Load<Material>("Materials/" + materialType + "/" + data[1]));
        }
        // リストに追加
        materialList.Add(listM);
    }
    /// <summary>
    /// csvからオブジェクトの回転定数を取得
    /// </summary>
    /// <param name="csvPath">読み込みたいcsvのアドレス</param>
    void SetRotationListInit(string csvPath)
    {
        // <指定されたCSVを読み込む>
        // CSVを読み込むリスト
        List<string[]> list = new List<string[]>();
        // CSV読み込み
        list = LoadCSV(csvPath);
        // csvデータに沿って回転値を設定
        foreach (var data in list)
        {
            //// 一行目はスキップする
            //if (data[0] == "x")
            //    continue;
            // １行目はスキップする
            rotationList.Add(new Vector3(int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2])));
        }
    }
    /// <summary>
    /// マルチなオブジェクトのマテリアルナンバーを取得
    /// </summary>
    /// <param name="num">オブジェクトナンバー</param>
    /// <returns></returns>
    int GetMaterialNumberforMutiple(int num)
    {
        if (num < MULTIPLE_NUM)
        {
            return num;
        }
        else
        {
            int n = num / MULTIPLE_NUM;
            int c = 0;
            for (int i = 0; i < objList.Count; i++)
            {
                if (objList[i] == null)
                {
                    c++;
                    if (c == n)
                    {
                        return i;
                    }
                }
            }

            Debug.Log("error:cubeHolder.GetMaterialMultipleNum()");
            return 0;
        }
    }
}
