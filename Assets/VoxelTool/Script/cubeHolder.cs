using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEditor;
using System;

//Inspectorに複数データを表示するためのクラス
[System.SerializableAttribute]
public class ValueList
{
    public List<GameObject> list;
}


public class cubeHolder : MonoBehaviour
{
    enum OBJECT_NUM
    {
        CUBE=0,
        SLOPE=1,
        STAGE=2,
        SHORTSLOPE=8,
        LONGSLOPE=9
    }
    // ８以上のやつは複数オブジェクト　multipleList[オブジェクト番号/8 - 1][オブジェクト番号%8]でオブジェクトが持って来れる
    public const int OBJECT_MULTIPLE_NUM = 8;

    enum SAVE
    {
        X,
        Y,
        Z,
        OBJECTNUM,
        MATERIALNUM,
        ROTATION
    }

    enum STAMP_SAVE
    {
        NAME=0,
        POSX=0,
        POSY,
        POSZ,
        ROTX,
        ROTY,
        ROTZ
    }


    public List<GameObject> cubeList = new List<GameObject>();               // 配置中のオブジェクト集
    public List<int> cubeMatList = new List<int>();                          // 配置中のオブジェクトのマテリアル番号集　cubeListの番号と合ってる
    public List<GameObject> stampList = new List<GameObject>();              // 配置中のスタンプ集

    public Dropdown objDropdown;
    public Dropdown matDropdown;

    public List<GameObject> objList;                // 通常配置オブジェクト集　複数オブジェクトはnullが入ってるように
    public List<List<Material>> materialList;       // マテリアル集　objListの番号と合ってる
    public List<List<string>> materialTextList;     // dropdown用

    // 複数オブジェクトリスト
    public List<ValueList> multipleObjList = new List<ValueList>();

    // startで読み込むcsvの名前リスト
    [SerializeField]
    private List<string> LoadMaterialStr = new List<string>();
    [SerializeField]
    private stampScript stampObject = null;     // インスペクターのstampを入れるやつ

    // 回転をcsvに保存するためのリスト
    public List<Vector3> rotationList;

    // 上書きボタン
    [SerializeField]
    private Button buttonOverwrite = null;
    private string savePass ="";

    // Start is called before the first frame update
    void Start()
    {
        cubeMatList = new List<int>();
        cubeMatList.Add(0);
        materialList = new List<List<Material>>();
        materialTextList = new List<List<string>>();

        // マテリアルのドロップダウンを更新
        string path = "Assets/VoxelTool/BoxesData/Materials/";
        string csvExt = "Mat.csv";
        for (int i = 0; i < LoadMaterialStr.Count; i++) 
        {
            SetMaterialList(path + LoadMaterialStr[i], path + LoadMaterialStr[i] + csvExt);
        }

        // 回転リストを初期化
        rotationList = new List<Vector3>();
        rotationList.Add(new Vector3(0, 0, 0));
        rotationList.Add(new Vector3(0, 270, 0));
        rotationList.Add(new Vector3(0, 180, 0));
        rotationList.Add(new Vector3(0, 90, 0));

        rotationList.Add(new Vector3(0, 0, 90));
        rotationList.Add(new Vector3(0, 270, 90));
        rotationList.Add(new Vector3(0, 180, 90));
        rotationList.Add(new Vector3(0, 90, 90));

        //rotationList.Add(new Vector3(0, 0, 180));
        //rotationList.Add(new Vector3(0, 270, 180));
        //rotationList.Add(new Vector3(0, 180, 180));
        //rotationList.Add(new Vector3(0, 90, 180));
        rotationList.Add(new Vector3(270, 0, 0));
        rotationList.Add(new Vector3(270, 0, 270));
        rotationList.Add(new Vector3(270, 0, 180));
        rotationList.Add(new Vector3(270, 0, 90));
    }



    /// <summary>
    /// いろんなリストのリセット
    /// </summary>
    public void ResetCubeList()
    {
        foreach (GameObject obj in cubeList)
        {
            Destroy(obj);
        }
        foreach(GameObject obj in stampList)
        {
            Destroy(obj);
        }
        cubeList.Clear();
        cubeMatList.Clear();
        stampList.Clear();
        savePass = "";
    }


    /// <summary>
    /// ほぞんボタン押したとき
    /// ファイルパネルが出てくる、csvの保存
    /// </summary>
    public void SaveButtonClick()
    {
        // SaveFilePanelInProject 似たような奴だけど保存がプロジェクトファイル内に限定される　引数も違う
        // SaveFilePanel(セーブ画面のヘッダー(？)名、一番最初に開くとこ、デフォルトのファイル名、保存形式)
        var path = EditorUtility.SaveFilePanel("Save csv",Application.dataPath, "blockData", "csv");

        SaveCSV(path);
    }

    /// <summary>
    /// csvに保存するやつ
    /// </summary>
    /// <param name="path">保存したいところ</param>
    public void SaveCSV(string path)
    {
        Debug.Log(path);
        StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("Shift_JIS"));
        string[] s1 = { "x", "y", "z", "objectNum", "materialNum", "rotation" };
        string s2 = string.Join(",", s1);   // string.join(間の文字、文字配列) 文字配列の間に入れる記号をセットして合体させられる
        sw.WriteLine(s2);

        // データ出力
        // 通常配置オブジェクト
        for (int i = 0; i < cubeList.Count; i++)
        {
            string[] str = {
                // MathfのRoundだと偶数にしたがるらしいのでMathのRoundを使う　System
                Math.Round(cubeList[i].transform.position.x).ToString(),
                Math.Round(cubeList[i].transform.position.y).ToString(),
                Math.Round(cubeList[i].transform.position.z).ToString(),
                GetObjectNum(cubeList[i]).ToString(),
                cubeMatList[i].ToString(),
                GetRotateNum(cubeList[i].transform.GetChild(0).localEulerAngles).ToString()};
            string str2 = string.Join(",", str);
            sw.WriteLine(str2);
        }

        // スタンプ保存のやつ
        if (stampList.Count != 0)
        {
            // ヘッダーを書く、stampの名前と位置と回転を取得して文字化して書く
            string[] stampHeader = { "stampName", "posX", "posY", "posZ", "rotX", "rotY", "rotZ" };
            s2 = string.Join(",", stampHeader);
            sw.WriteLine(s2);

            for (int i = 0; i < stampList.Count; i++)
            {
                string[] str =
                {
                    stampList[i].name,
                    Math.Round(stampList[i].transform.position.x).ToString(),
                    Math.Round(stampList[i].transform.position.y).ToString(),
                    Math.Round(stampList[i].transform.position.z).ToString(),
                    Math.Round(stampList[i].transform.localEulerAngles.x).ToString(),
                    Math.Round(stampList[i].transform.localEulerAngles.y).ToString(),
                    Math.Round(stampList[i].transform.localEulerAngles.z).ToString()
                };
                string str2 = string.Join(",", str);
                sw.WriteLine(str2);
            }
        }

        // StreamWriterを閉じる
        sw.Close();
    }


    /// <summary>
    /// よみこみボタンを押したとき
    /// ファイル選択画面を開いて盤面をリセット、選ばれたcsvを読み込む
    /// </summary>
    public void LoadButtonClick()
    {
        var path = EditorUtility.OpenFilePanel("Select Asset", Application.dataPath, "csv");

        // ファイルパネルでなんか選んでた時
        if (path.Length != 0)
        {
            // フィールドlist２種の中身リセット
            ResetCubeList();
            
            // 選んだファイルを読み込む
            LoadCSV(path, cubeList, cubeMatList);

            // 選んだアドレスを記憶
            savePass = path;
            buttonOverwrite.gameObject.SetActive(true);
        }

    }


    /// <summary>
    /// csvを読み込んでcube生成してリストに保存する
    /// </summary>
    /// <param name="path">読み込むファイルのアドレス</param>
    /// <param name="listC">cubeを保存するリスト</param>
    /// <param name="listM">material番号を保存するリスト</param>
    public void LoadCSV(string path, List<GameObject> listC, List<int> listM)
    {
        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(path);

        //List<string[]> list = new List<string[]>();
        List<List<int>> blockList = new List<List<int>>();
        bool f = false;
        List<string> stampNameList = new List<string>();
        List<List<int>> stampPosList = new List<List<int>>();

        string firstLine = sr.ReadLine();
        if(firstLine == "x,y,z,objectNum,materialNum,rotation")
        {
            // 末尾まで繰り返す
            while (!sr.EndOfStream)
            {
                // CSVファイルの一行を読み込む
                string line = sr.ReadLine();

                // スタンプのヘッダーの文字が出てきたら別のリストに入れる感じで　このwhileの前にbool型のなんか宣言しておく
                if(!f && line=="stampName,posX,posY,posZ,rotX,rotY,rotZ")
                {
                    f = true;
                    continue;
                }

                // 読み込んだ一行をカンマ毎に分けて配列に格納する
                string[] values = line.Split(',');

                if(f)
                {
                    stampNameList.Add(values[(int)STAMP_SAVE.NAME]);

                    // intに変換
                    List<int> numList = new List<int>();
                    for (int i = 1; i < values.Length; i++)
                    {
                        numList.Add(int.Parse(values[i]));
                    }

                    // 配列からリストに格納する
                    stampPosList.Add(numList);
                }

                else
                {
                    // intに変換
                    List<int> numList = new List<int>();
                    for (int i = 0; i < values.Length; i++)
                    {
                        numList.Add(int.Parse(values[i]));
                    }

                    // 配列からリストに格納する
                    blockList.Add(numList);
                }

            }

            // 通常配置オブジェクトを読み込む
            foreach (var data in blockList)
            {
                GameObject obj;
                if (data[(int)SAVE.OBJECTNUM] / OBJECT_MULTIPLE_NUM > 0)
                {
                    // オブジェクト番号が８以上だったら複数オブジェクトを取得
                    obj = multipleObjList[data[(int)SAVE.OBJECTNUM] / OBJECT_MULTIPLE_NUM - 1].list[data[(int)SAVE.OBJECTNUM] % OBJECT_MULTIPLE_NUM];
                }
                else
                {
                    // ８以下なら普通に取得
                    obj = objList[data[(int)SAVE.OBJECTNUM]];
                }
                // マテリアル番号を貰う
                int materialNum = GetMaterialMultipleNum(data[(int)SAVE.OBJECTNUM]);

                Vector3 v = new Vector3(data[(int)SAVE.X], data[(int)SAVE.Y], data[(int)SAVE.Z]);                       // 座標
                GameObject c = Instantiate(obj, v, Quaternion.identity);                                                // obj生成
                c.GetComponentInChildren<Renderer>().material = materialList[materialNum][data[(int)SAVE.MATERIALNUM]]; // マテリアルを変更
                if (data[(int)SAVE.OBJECTNUM] != (int)OBJECT_NUM.CUBE && data[(int)SAVE.OBJECTNUM] != (int)OBJECT_NUM.STAGE)
                {
                    // 正方形以外なら回転をかける
                    c.transform.GetChild(0).localEulerAngles = rotationList[data[(int)SAVE.ROTATION]];
                }
                if (listC != null)
                {
                    listC.Add(c);
                }
                if (listM != null)
                {
                    listM.Add(data[(int)SAVE.MATERIALNUM]);
                }
            }

            // スタンプ配置オブジェクトを読み込む
            for (int i = 0; i < stampNameList.Count; i++)
            {
                // インデックス番号を取得
                int n = stampObject.stampNameList.IndexOf(stampNameList[i]);

                if (n != -1)
                {
                    Quaternion rot = Quaternion.Euler(stampPosList[i][(int)STAMP_SAVE.ROTX], stampPosList[i][(int)STAMP_SAVE.ROTY], stampPosList[i][(int)STAMP_SAVE.ROTZ]);
                    // prefabだああああああああ
                    GameObject stampPrefab = Instantiate(stampObject.prefabList[n],
                        new Vector3(stampPosList[i][(int)STAMP_SAVE.POSX], stampPosList[i][(int)STAMP_SAVE.POSY], stampPosList[i][(int)STAMP_SAVE.POSZ]),
                        rot);

                    // 子オブジェクトたちを逆回転させて普通にする　Inverse便利～
                    foreach(Transform child in stampPrefab.transform)
                    {
                        child.localRotation = Quaternion.Inverse(rot);
                    }
                    // (Clone)を消す
                    stampPrefab.name = stampNameList[i];
                    // stampListにさっき作ったやつを入れる
                    stampList.Add(stampPrefab);
                }
                else
                {
                    Debug.Log("cubeHolder.LoadCSV():" + stampNameList[i] + "がありません");
                }

            }
        }
        else
        {
            Debug.Log("cubeHolder.LoadCSV():ヘッダーが違います");
        }
        // 閉じ忘れないようにね！！
        sr.Close();
    }



    /// <summary>
    /// csvからマテリアルと名前読み込んで順番に配列に入れる
    /// </summary>
    /// <param name="folderPath">読み込みたいフォルダの場所</param>
    /// <param name="csvPath">読み込むcsvの場所</param>
    void SetMaterialList(string folderPath, string csvPath)
    {
        // csvを読み込む
        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(csvPath, Encoding.GetEncoding("Shift_JIS"));

        List<string[]> list = new List<string[]>();
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

        // listを用意
        List<Material> listM = new List<Material>();
        List<string> listT = new List<string>();

        listM.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/VoxelTool/BoxesData/Materials/defaultmaterial.mat"));
        listT.Add("デフォルト(0)");
        foreach (var data in list)
        {
            if (data[0] == "name") // ヘッダー飛ばす
                continue;
            listM.Add(AssetDatabase.LoadAssetAtPath<Material>(folderPath + "/" + data[1] + ".mat"));
            listT.Add(data[0]);
        }
        materialList.Add(listM);
        materialTextList.Add(listT);
    }


    /// <summary>
    /// listに追加するやつ
    /// マテリアルはドロップダウンから自動取得
    /// </summary>
    /// <param name="obj">追加するオブジェクト</param>
    public void AddCubeList(GameObject obj)
    {
        obj.GetComponentInChildren<Renderer>().material = materialList[objDropdown.value][matDropdown.value];
        cubeList.Add(obj);
        cubeMatList.Add(matDropdown.value);
    }


    /// <summary>
    /// listに追加するやつ
    /// マテリアル番号を入力できる
    /// </summary>
    /// <param name="obj">追加するオブジェクト</param>
    /// <param name="matNum">追加するマテリアル番号</param>
    public void AddCubeList(GameObject obj, int matNum)
    {
        cubeList.Add(obj);
        cubeMatList.Add(matNum);
    }


    /// <summary>
    /// listから消すやつ
    /// materialListからも消える
    /// </summary>
    /// <param name="obj">消すオブジェクト</param>
    public void DeleteCubeList(GameObject obj)
    {
        // スタンプ対策
        if (obj.transform.root.gameObject != obj)
        {
            stampList.Remove(obj.transform.root.gameObject);
            Destroy(obj.transform.root.gameObject);
        }
        else
        {
            int lNum = cubeList.IndexOf(obj);
            cubeMatList.RemoveAt(lNum);
            cubeList.Remove(obj);
            Destroy(obj);
        }
    }


    /// <summary>
    /// listに同じ位置のオブジェクトがないか探すやつ
    /// trueだったらなんか重なってた
    /// </summary>
    /// <param name="obj">同じ位置のがあるか探したいオブジェクト</param>
    /// <param name="deleteExisting">trueだったら元々置いてある方を消す　falseだったらobjを消す</param>
    /// <returns>なんか置いてあったらtrue</returns>
    public bool CompareCubeList(GameObject obj, bool deleteExisting = true)
    {
        GameObject g = GetHitCube(obj.transform.position);
        if(g)
        {
            if(deleteExisting)
            {
                DeleteCubeList(g);
            }
            else
            {
                Destroy(obj);
            }
            return true;
        }

        return false;
    }


    /// <summary>
    /// 引数のスタンプがスタンプリストの中身と重なってないか
    /// 重なってたらtrue　なかったらfalse
    /// 多分すっごい重い
    /// </summary>
    /// <param name="stamp">比較したいスタンプ　親入れる</param>
    /// <returns></returns>
    public bool CompareStampList(GameObject stamp)
    {
        foreach(GameObject list in stampList)
        {
            foreach (Transform child in list.transform)
            {
                foreach(Transform pos in stamp.transform)
                {
                    if (child.position == pos.position)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// オブジェクトの名前からオブジェクト番号(enum OBJECT_NUM)を返すやつ
    /// </summary>
    /// <param name="obj">番号を知りたいオブジェクト</param>
    /// <returns></returns>
    public int GetObjectNum(GameObject obj)
    {
        for (int i = 0; i < objList.Count; i++)
        {
            // string.Contains(string value) 左のstringの中にvalueが含まれていたらtrue なかったらfalseを返す
            if(objList[i])
            {
                if (obj.name.Contains(objList[i].name))
                {
                    return i;
                }
            }
            else
            {
                // 複数オブジェクトの時　８以上のやつが返る
                int c = 0;
                List<GameObject> list = GetMultipleMaterialList(i,c);
                for (int j = 0; j < list.Count; j++) 
                {
                    if(obj.name.Contains(list[j].name))
                    {
                        return (c + 1) * OBJECT_MULTIPLE_NUM + j;
                    }
                }
            }
        }

        Debug.Log("error:cubeHolder.GetObjectNum()");
        return objList.Count + 1;
    }


    /// <summary>
    /// ローカルのrotationからrotationListの番号を返すやつ
    /// </summary>
    /// <param name="rot">回転番号を知りたいrotation</param>
    /// <returns></returns>
    public int GetRotateNum(Vector3 rot)
    {
        for (int i = 0; i < rotationList.Count; i++)
        {
            float f = Quaternion.Angle(Quaternion.Euler(rot), Quaternion.Euler(rotationList[i]));

            if (f % 360 == 0)
            {
                // 角度が一致してたらその番号を返す
                return i;
            }
        }

        Debug.Log("error:cubeHolder.GetRotateNum()");
        return 12;
    }


    /// <summary>
    /// 指定した座標のオブジェクトを取得　なかったらnull
    /// </summary>
    /// <param name="pos">座標</param>
    /// <returns></returns>
    public GameObject GetHitCube(Vector3 pos)
    {
        for (int i = 0; i < cubeList.Count; i++)
        {
            if (cubeList[i].transform.position == pos)
            {
                return cubeList[i];
            }
        }
        return null;
    }


    /// <summary>
    /// Raycastのhitの位置からオブジェクトを置く予測位置を取得する
    /// JointにあったやつだけどStampにも使うので移植
    /// </summary>
    /// <param name="hit">Rayのやつ</param>
    /// <returns></returns>
    public Vector3 GetTargetPos(RaycastHit hit)
    {
        Vector3 pos = new Vector3();
        if (hit.transform.name == "TonsQuad" || hit.transform.parent.name == "bedrock")
        {
            // 岩盤対策　Ceil(切り上げ)を使って都合のいい位置に置けるようにする
            pos = new Vector3(Mathf.Ceil(hit.point.x - 0.5f), Mathf.Ceil(hit.point.y), Mathf.Ceil(hit.point.z - 0.5f));
        }
        else
        {
            // 触ったQuadのposition×２分(要するにブロック１個分)を触ったブロックのpositionに足す
            pos = hit.transform.parent.position + hit.transform.localPosition * 2;
        }

        return pos;
    }



    /// <summary>
    /// オブジェクト番号→マテリアル番号への変換
    /// 具体的に言うと8と9で3が帰ってくる
    /// </summary>
    /// <param name="num">オブジェクト番号</param>
    /// <returns></returns>
    public int GetMaterialMultipleNum(int num)
    {
        if (num < OBJECT_MULTIPLE_NUM)
        {
            // 普通のオブジェクトだったら普通に返す
            return num;
        }
        else
        {
            // 複数オブジェクトなので複数オブジェクトのどれを指してるか取得
            int n = num / OBJECT_MULTIPLE_NUM;
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


    /// <summary>
    /// マテリアル番号から複数オブジェクトのリストを貰う
    /// 具体的に言うと3でMultipleObjList[0]が帰ってくる
    /// </summary>
    /// <param name="num">マテリアル番号</param>
    /// <param name="c">リストの何番目か　いらなかったら入れなくてOK</param>
    /// <returns></returns>
    public List<GameObject> GetMultipleMaterialList(int num,int c = 0)
    {
        c = 0;
        for (int i = 0; i < num; i++)
        {
            if (objList[i] == null)
            {
                c++;
            }
        }
        return multipleObjList[c].list;
    }

    /// <summary>
    /// うわがきボタン押したとき
    /// </summary>
    public void ClickOverwriteButton()
    {
        if(savePass!="")
        {
            SaveCSV(savePass);
        }
        else
        {
            Debug.Log("error:cubeHolder.cs ClickOverwriteButton");
        }
    }
}
