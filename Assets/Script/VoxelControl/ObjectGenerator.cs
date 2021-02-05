// オブジェクトを生成するスクリプト
// 2020/09/14
// 佐竹晴登

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using UnityEngine.UI;

[System.SerializableAttribute]
public class MultipleList
{
    public List<GameObject> list;
}
public class ObjectGenerator : MonoBehaviour
{
    // 親オブジェクト
    [SerializeField]
    Transform parentObject;
    // でかい立方体を生成する時の必要変数 -------------------------------
    // オブジェクトの縦幅(縦のブロック数)
    [SerializeField]
    int OBJECT_HEIGHT = 2;
    // オブジェクトの横幅(横のブロック数)
    [SerializeField]
    int OBJECT_WIDTH  = 2;
    // オブジェクトの奥行
    [SerializeField]
    int OBJECT_DEPTH  = 2;
    // オブジェクト群(三次配列)
    Cube[,,] objects;
    // -----------------------------------------------------------------

    // ツールで作ったステージを読み込む時の必要変数 --------------------
    // キューブ元
    [SerializeField]
    GameObject objectPrefab;
    // 破片を保存しておくリスト
    List<GameObject> fragments = new List<GameObject>();
    // 岩盤オブジェクト
    [SerializeField]
    Transform bedlock;
    // ステージ名
    [SerializeField]
    public string stagename;
    // csvファイル
    string filepath;
    // cubeのGameObject情報
    [SerializeField]
    List<Cube> cubeList = new List<Cube>();
    // cubeのマテリアル情報
    [SerializeField]
    List<int> cubeMatList = new List<int>();
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

    // スタンプのPrefabリスト
    [SerializeField]
    List<GameObject> stampList = new List<GameObject>();
    // スタンプの名前を集めたリスト
    List<string> stampNameList = new List<string>();
    // ステージがロードできたかどうか
    bool isStageLoaded = false;
    // 存在するオブジェクトの種類
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
    // 車の情報
    [SerializeField]
    GameObject car;
    // ゴールの挙動
    [SerializeField]
    GameObject goal;
    // チェックポイントのPrefab情報
    [SerializeField]
    GameObject checkPoint;
    // 優しい坂以上のアレ
    public const int MULTIPLE_NUM = 8;
    // ステージデータcsvの保存ファイル
    string csvDataFile = "/StreamingAssets/Data/csv/StageFile/";
    // 情報系csvの保存ファイル
    string csvInfoFile = "/StreamingAssets/Data/csv/VoxelInfo/";
    [SerializeField]
    List<string> loadMaterialStr = new List<string>();
    // ロード中表示UI
    //　シーンロード中に表示するUI画面
    [SerializeField]
    GameObject loadUI;
    //　読み込み率を表示するスライダー
    [SerializeField]
    Slider slider;
    // ジェネレートカウント用
    int gene_count = 0;
    // ディレクター
    GameDirector director;

    // ---------------------------------------------------
    // 初期化処理
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
        // ファイルパスを初期化(ビルド後にも対応）
        filepath = Application.dataPath + csvDataFile + stagename + ".csv";
        // スタンプネームリストに追加していく
        foreach(var v in stampList)
        {
            stampNameList.Add(v.name);
        }
        // ディレクター取得
        director = this.GetComponent<GameDirector>();
    }
    // ---------------------------------------------------
    // スタート関数
    void Start()
    {
        //　ロード画面UIをアクティブにする
        loadUI.SetActive(true);
        // <立方体を読み込み(デバッグ用)
        //GenerateBox();
        // コルーチン開始
        StartCoroutine("LoadStage");
    }
    // 更新関数
    private void Update()
    {
        if(!loadUI.activeSelf)
        {
            StopAllCoroutines();
        }
    }

    // ---------------------------------------------------
    // 全体を元として範囲内にあるCubeを取得する
    // distance 範囲の距離
    // pos      中心点
    public Cube[] GetCubeInRange(float distance,Vector3 pos)
    {
        // 返り値となる配列
        List<Cube> cubes = new List<Cube>();
        // 削除予定のオブジェクトリスト
        List<Cube> save = new List<Cube>();
        foreach(var c in cubeList)
        {
            // 範囲内であれば戻り値リストに追加
            if (Vector3.Distance(pos, c.transform.position) <= distance)
            {
                cubes.Add(c);
            }     
        }
        // 配列の中身は破片となるのでリストからは削除
        foreach(var c in cubes)
        {
            cubeList.Remove(c);
        }
        
        return cubes.ToArray();
    }
    // ---------------------------------------------------
    // CSVを読み込む関数
    // path 読み込むファイルのアドレス
    // １行目をスキップするかどうか
    public List<string[]> LoadCSV(string path,bool skipfirst = false)
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
    // ---------------------------------------------------
    // CSVファイルを基にブロックを生成する
    // data CSVを読み込んだリスト
    // external 外部読み込みかどうか
    public GameObject BlockGenerate(string[] data, bool external = false)
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
        Vector3 v = new Vector3(dataInt[(int)BoxDataType.POS_X] + (bedlock.position.x + 1), dataInt[(int)BoxDataType.POS_Y] + (bedlock.position.y + 1), dataInt[(int)BoxDataType.POS_Z] + (bedlock.position.z + 1));

        // ステージ構成
        if (dataInt[(int)BoxDataType.OBJECT] == (int)ObjectType.STAGE)
        {
            int num = dataInt[(int)BoxDataType.MATERIAL];
            // ステージ構成オブジェクトはマテリアルで判別
            switch (num)
            {
                // スタート位置
                case 1:
                    car.transform.position = v;
                    break;
                // ゴール位置
                case 2:
                    goal.transform.position = v;
                    break;
                // それ以外はチェックポイント
                default:
                    // チェックポイントを生成
                    CheckPoint check = Instantiate(checkPoint, v, Quaternion.identity,parentObject).GetComponent<CheckPoint>();
                    // チェックポイントは3以上から始まるので-3で1からカウントして設定する
                    int cpnum = num - 3;
                    check.SetCheckPointNumber(cpnum);
                    // チェックポイントの最大値を設定する
                    director.SetMaxCheckPoint(cpnum + 1);
                    break;
            }
        }
        else
        {
            // オブジェクト生成
            GameObject go = Instantiate(bamp, v, Quaternion.identity);
            go.name = gene_count.ToString();
            // マテリアルを適用
            go.GetComponentInChildren<Renderer>().material = materialList[m][dataInt[(int)BoxDataType.MATERIAL]];
            // オブジェクト番号０(四角）以外なら
            if (dataInt[(int)BoxDataType.OBJECT] != (int)ObjectType.CUBE && dataInt[(int)BoxDataType.OBJECT] != (int)ObjectType.STAGE)
            {
                // 回転を適用
                go.transform.localEulerAngles = rotationList[dataInt[(int)BoxDataType.ROTATION]];
            }
            if (!external)
            {
                // 存在するキューブとして登録
                cubeList.Add(go.GetComponent<Cube>());
            }
            returnObject = go;
        }
        gene_count++;
        return returnObject;
    }
    // ---------------------------------------------------
    // CSVファイルを基にスタンプを生成する
    // data スタンプの文字列情報リスト

    void StampGenerate(string[] data)
    {
        // 入ってきたデータの名前をもとにプレファブ生成
        // 名前を基に生成するスタンプを検索して取得
        int n = stampNameList.IndexOf(data[(int)StampDataType.STAMPNAME]);
        // エラー判定
        if(n != -1)
        {
            // 回転値用意
            Quaternion rot = Quaternion.Euler(float.Parse(data[(int)StampDataType.ROT_X]),
                                              float.Parse(data[(int)StampDataType.ROT_Y]),
                                              float.Parse(data[(int)StampDataType.ROT_Z]));
            // Prefab生成(csv座標に1.0分のズレがあるため矯正する)
            GameObject stampObject = Instantiate(stampList[n],
                new Vector3(float.Parse(data[(int)StampDataType.POS_X]) + 1.0f,
                            float.Parse(data[(int)StampDataType.POS_Y]) + 1.0f,
                            float.Parse(data[(int)StampDataType.POS_Z]) + 1.0f), rot);

            //// Cubeリストに追加
            if (data[(int)StampDataType.STAMPNAME] != "Kabe" && data[(int)StampDataType.STAMPNAME] != "Kabe2")
            {
                foreach (Transform child in stampObject.transform)
                {
                    cubeList.Add(child.GetComponent<Cube>());
                }
            }
        }
        else
        {
            Debug.Log("error:'"+ data[(int)StampDataType.STAMPNAME] + "' Stamp not find");
        }
    }
    // ---------------------------------------------------
    // 指定スケールの立方体を生成
    void GenerateBox()
    {
        objects = new Cube[OBJECT_HEIGHT, OBJECT_WIDTH, OBJECT_DEPTH];
        // オブジェクト生成
        for (int i = 0; i < OBJECT_HEIGHT; i++)
        {
            for (int j = 0; j < OBJECT_WIDTH; j++)
            {
                for (int k = 0; k < OBJECT_DEPTH; k++)
                {
                    // オブジェクト生成
                    GameObject instance = (GameObject)Instantiate(objectPrefab,
                                                                    new Vector3(j, i+2f, k),
                                                                    Quaternion.identity);
                    instance.GetComponentInChildren<Renderer>().material = materialList[0][4];
                    objects[i, j, k] = instance.GetComponent<Cube>();

                    // 親設定
                    objects[i, j, k].transform.parent = parentObject;
                    // 破片として登録
                    cubeList.Add(objects[i, j, k]);
                }
            }
        }
        isStageLoaded = true;
    }
    // ----------------------------------------------------
    // csvからマテリアルと名前読み込んで順番に配列に入れる
    //
    // materialType         読み込みたいフォルダの場所
    // csvPath              読み込むcsvの場所
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

            listM.Add(Resources.Load<Material>("Materials/"+ materialType + "/" + data[1]));
        }
        // リストに追加
        materialList.Add(listM);
    }
    // ----------------------------------------------------
    // csvからオブジェクトの回転定数を取得
    // 
    // csvPath      読み込みたいcsvのアドレス
    void SetRotationListInit(string csvPath)
    {
        // <指定されたCSVを読み込む>
        // CSVを読み込むリスト
        List<string[]> list = new List<string[]>();
        // CSV読み込み
        list = LoadCSV(csvPath);
        // csvデータに沿って回転値を設定
        foreach(var data in list)
        {
            //// 一行目はスキップする
            //if (data[0] == "x")
            //    continue;
            // １行目はスキップする
            rotationList.Add(new Vector3(int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2])));
        }
    }
    // ---------------------------------------------------------------------------
    // マルチなオブジェクトのマテリアルナンバーを取得
    // num オブジェクトナンバー
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
    // ---------------------------------------------------------------------------
    // ステージロード中画面コルーチン
    IEnumerator LoadStage()
    {
        // <csvファイルを読み込み>
        // CSVを保存するリスト
        List<string[]> dataList = new List<string[]>();
        // ファイル読み込み
        dataList = LoadCSV(filepath);
        bool stampG = false;
        int i = 0;
        // <ステージ生成>
        foreach (var d in dataList)
        {
            // 文字列部分スキップ
            if (d[0] == "name")
                continue;
            // スタンプネームが出てきたら、次からはスタンプ生成になるので
            // フラグON
            else if(d[0] == "stampName")
            {
                stampG = true;
                continue;
            }
            // ステージ生成関数
            if(stampG)
            {
                StampGenerate(d);
            }
            else
            {
                BlockGenerate(d);
            }

            if (i % 20 == 0)
            {
                float progressVal = (float)i / (float)dataList.Count;
                slider.value = progressVal;

                yield return null;
            }
            i++;
        }
        director.SetGameStart();
        isStageLoaded = true;
        loadUI.SetActive(false);
        yield break;
    }
    // ---------------------------------------------------------------------------
    // ステージがロードし終わったかどうかのゲッタ
    public bool GetStageLoad()
    {
        return isStageLoaded;
    }
}
