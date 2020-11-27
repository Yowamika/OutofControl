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

    enum SAVE
    {
        X,
        Y,
        Z,
        OBJECTNUM,
        MATERIALNUM,
        ROTATION
    }
    public const int OBJECT_MULTIPLE_NUM = 8;

    public List<GameObject> cubeList;               // 配置中のオブジェクト集
    public List<int> cubeMatList;                   // 配置中のオブジェクトのマテリアル番号集　cubeListの番号と合ってる

    public Dropdown objDropdown;
    public Dropdown matDropdown;

    public List<GameObject> objList;                // 配置オブジェクト集
    public List<List<Material>> materialList;       // マテリアル集　objListの番号と合ってる
    public List<List<string>> materialTextList;     // dropdown用

    public List<ValueList> multipleObjList = new List<ValueList>();
    [SerializeField]
    private List<string> LoadMaterialStr = new List<string>();

    // cube以外の回転をcsvに保存するためのリスト
    public List<Vector3> rotationList;

    // Start is called before the first frame update
    void Start()
    {
        cubeMatList = new List<int>();
        cubeMatList.Add(0);
        materialList = new List<List<Material>>();
        materialTextList = new List<List<string>>();

        //SetMaterialList();
        string path = "Assets/VoxelTool/BoxesData/Materials/";
        string csvExt = "Mat.csv";
        for (int i = 0; i < LoadMaterialStr.Count; i++) 
        {
            SetMaterialList(path + LoadMaterialStr[i], path + LoadMaterialStr[i] + csvExt);
        }
        //SetMaterialList(path+"Cube", path+"Cube"+csvExt);
        //SetMaterialList("Assets/VoxelTool/BoxesData/Materials/Slope", "Assets/VoxelTool/BoxesData/Materials/SlopeMat.csv");
        //List<Material> listM = new List<Material>();
        //List<string> listT = new List<string>();
        //listM.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/VoxelTool/startMaterial.mat"));
        //listT.Add("スタート");
        //listM.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/VoxelTool/goalMaterial.mat"));
        //listT.Add("ゴール");
        //materialList.Add(listM);
        //materialTextList.Add(listT);

        //listM.Clear();
        //listT.Clear();
        //listM.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/VoxelTool/BoxesData/Materials/defaultmaterial.mat"));
        //listT.Add("デフォルト(0)");
        //materialList.Add(listM);
        //materialTextList.Add(listT);
        //for (int i = materialList.Count; i < 10; i++)
        //{
        //    materialList.Add(new List<Material>());
        //    materialTextList.Add(new List<string>());
        //}
        //materialList[8].Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/VoxelTool/BoxesData/Materials/defaultmaterial.mat"));
        //materialList[9].Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/VoxelTool/BoxesData/Materials/defaultmaterial.mat"));
        //materialTextList[8].Add("デフォルト(0)");
        //materialTextList[9].Add("デフォルト(0)");


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


    // ---------------------------------------------------------------------------
    // キューブリストのリセット
    public void ResetCubeList()
    {
        foreach (GameObject obj in cubeList)
        {
            Destroy(obj);
        }
        cubeList.Clear();
        cubeMatList.Clear();
    }

    // ---------------------------------------------------------------------------
    public void SaveButtonClick()
    {
        // メモ SaveFilePanelInProject 似たような奴だけど保存がプロジェクトファイル内に限定される　引数も違う
        // SaveFilePanel(セーブ画面のヘッダー(？)名、一番最初に開くとこ、デフォルトのファイル名、保存形式)
        var path = EditorUtility.SaveFilePanel("Save csv",Application.dataPath, "blockData", "csv");

        Debug.Log(path);
        StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("Shift_JIS"));
        string[] s1 = { "x", "y", "z", "objectNum", "materialNum" ,"rotation"  };
        string s2 = string.Join(",", s1);   // string.join(間の文字、文字配列) 文字配列の間に入れる記号をセットして合体させられる
        sw.WriteLine(s2);

        // データ出力
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
        // StreamWriterを閉じる
        sw.Close();
    }

    // ---------------------------------------------------------------------------
    // よみこみボタンを押したとき
    // ファイル選択画面を開いて盤面をリセット、選ばれたcsvを読み込む
    public void LoadButtonClick()
    {
        var path = EditorUtility.OpenFilePanel("Select Asset", Application.dataPath, "csv");

        // フィールドlist２種の中身リセット
        ResetCubeList();
        
        LoadCSV(path, cubeList, cubeMatList);
    }

    // ---------------------------------------------------------------------------
    // LoadCSV                      csvを読み込んでcube生成してリストに保存する
    // 
    // string path                  読み込むファイルのアドレス
    // List<GameObject> listC       cubeを保存するリスト
    // List<int> listM              material番号を保存するリスト
    public void LoadCSV(string path, List<GameObject> listC, List<int> listM)
    {
        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(path);

        //List<string[]> list = new List<string[]>();
        List<List<int>> list = new List<List<int>>();
        string firstLine = sr.ReadLine();
        if(firstLine == "x,y,z,objectNum,materialNum,rotation")
        {
            // 末尾まで繰り返す
            while (!sr.EndOfStream)
            {
                // CSVファイルの一行を読み込む
                string line = sr.ReadLine();
                // 読み込んだ一行をカンマ毎に分けて配列に格納する
                string[] values = line.Split(',');

                List<int> numList = new List<int>();
                for (int i = 0; i < values.Length; i++)
                {
                    numList.Add(int.Parse(values[i]));
                }

                // 配列からリストに格納する
                list.Add(numList);
            }

            foreach (var data in list)
            {
                GameObject g;
                if (data[(int)SAVE.OBJECTNUM] / OBJECT_MULTIPLE_NUM > 0)
                {
                    g = multipleObjList[data[(int)SAVE.OBJECTNUM] / OBJECT_MULTIPLE_NUM - 1].list[data[(int)SAVE.OBJECTNUM] % OBJECT_MULTIPLE_NUM];
                }
                else
                {
                    g = objList[data[(int)SAVE.OBJECTNUM]];
                }
                int m = GetMaterialMultipleNum(data[(int)SAVE.OBJECTNUM]);

                Vector3 v = new Vector3(data[(int)SAVE.X], data[(int)SAVE.Y], data[(int)SAVE.Z]);
                GameObject c = Instantiate(g, v, Quaternion.identity);
                c.GetComponentInChildren<Renderer>().material = materialList[m][data[(int)SAVE.MATERIALNUM]];
                if (data[(int)SAVE.OBJECTNUM] != (int)OBJECT_NUM.CUBE && data[(int)SAVE.OBJECTNUM] != (int)OBJECT_NUM.STAGE)
                {
                    // 正方形以外なら回転をかける
                    c.transform.GetChild(0).localEulerAngles = rotationList[data[(int)SAVE.ROTATION]];
                }
                listC.Add(c);
                listM.Add(data[(int)SAVE.MATERIALNUM]);
            }
        }
        else
        {
            Debug.Log("cubeHolder.LoadCSV()");
        }
    }

    // ---------------------------------------------------------------------------
    // フォルダからマテリアル全部読み込んで順番に配列に入れる
    // …ってやってたけどこれだとマテリアル追加するたびに順番ぐっちゃぐちゃになるから供養
    void SetMaterialList()
    {
        string path = "Assets/VoxelTool/BoxesData/Materials";
        string[] folders = AssetDatabase.GetSubFolders(path);

        materialList = new List<List<Material>>();

        for (int i = 0; i < folders.Length; i++)
        {
            List<Material> listM = new List<Material>();
            List<string> listT = new List<string>();

            // 読み込むパスを用意　folders[i]
            string[] files = Directory.GetFiles(folders[i], "*.mat", System.IO.SearchOption.AllDirectories);
            listM.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/VoxelTool/BoxesData/Materials/defaultmaterial.mat"));
            listT.Add("デフォルト(0)");
            for (int j = 0; j < files.Length; j++)
            {                
                // リストに追加
                listM.Add(AssetDatabase.LoadAssetAtPath<Material>(files[j]));
                listT.Add(Path.GetFileNameWithoutExtension(files[j]));
            }
            // リストを追加
            materialList.Add(listM);
            materialTextList.Add(listT);
        }
    }

    // ---------------------------------------------------------------------------
    // csvからマテリアルと名前読み込んで順番に配列に入れる
    //
    // folderPath       読み込みたいフォルダの場所
    // csvPath          読み込むcsvの場所
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

    // ---------------------------------------------------------------------------
    // listに追加するやつ
    // マテリアルはドロップダウンから自動取得
    public void AddCubeList(GameObject obj)
    {
        obj.GetComponentInChildren<Renderer>().material = materialList[objDropdown.value][matDropdown.value];
        cubeList.Add(obj);
        cubeMatList.Add(matDropdown.value);
    }

    // ---------------------------------------------------------------------------
    // listに追加するやつ
    // マテリアル番号を入力できる
    public void AddCubeList(GameObject obj, int matNum)
    {
        cubeList.Add(obj);
        cubeMatList.Add(matNum);
    }

    // ---------------------------------------------------------------------------
    // listから消すやつ
    // materialListからも消える
    public void DeleteCubeList(GameObject obj)
    {
        int lNum = cubeList.IndexOf(obj);
        cubeMatList.RemoveAt(lNum);
        cubeList.Remove(obj);
        Destroy(obj);
    }

    // ---------------------------------------------------------------------------
    // listに同じ位置のオブジェクトがないか探すやつ
    // 存在してた場合DeleteCubeListで元々あった方を消す
    public bool CompareCubeList(GameObject obj)
    {
        //for (int i = 0; i < cubeList.Count; i++)
        //{
        //    if (obj.transform.position == cubeList[i].transform.position) 
        //    {
        //        DeleteCubeList(cubeList[i]);
        //        return true;
        //    }
        //}
        GameObject g = GetHitCube(obj.transform.position);
        if(g)
        {
            DeleteCubeList(g);
            return true;
        }

        return false;
    }

    // ---------------------------------------------------------------------------
    // オブジェクトの名前からオブジェクト番号を返すやつ
    public int GetObjectNum(GameObject obj)
    {
        // 緩やか坂対策
        if (obj.name.Contains("gentle"))
        {
            if (obj.name.Contains("Short"))
            {
                return (int)OBJECT_NUM.SHORTSLOPE;
            }
            if (obj.name.Contains("Tall"))
            {
                return (int)OBJECT_NUM.LONGSLOPE;
            }
        }

        for (int i = 0; i < objList.Count; i++)
        {
            // メモ：string.Contains(string value) 左のstringの中にvalueが含まれていたらtrue なかったらfalseを返す
            if(obj.name.Contains(objList[i].name))
            {
                return i;
            }
        }

        Debug.Log("error:cubeHolder.GetObjectNum()");
        return objList.Count + 1;
    }

    // ---------------------------------------------------------------------------
    // ローカルのrotationからrotationListの番号を返すやつ
    public int GetRotateNum(Vector3 rot)
    {
        for (int i = 0; i < rotationList.Count; i++)
        {
            float f = Quaternion.Angle(Quaternion.Euler(rot), Quaternion.Euler(rotationList[i]));

            if (f % 360 == 0)
            {
                return i;
            }
        }
        if (rot == Vector3.zero)
        {
            return 0;
        }

        Debug.Log("error:cubeHolder.GetRotateNum()");
        return 12;
    }

    // ---------------------------------------------------------------------------
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

    // ---------------------------------------------------------------------------
    public int GetMaterialMultipleNum(int num)
    {
        if (num < OBJECT_MULTIPLE_NUM)
        {
            return num;
        }
        else
        {
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

    // ---------------------------------------------------------------------------
    public List<GameObject> GetMultipleMaterialList(int num)
    {
        int c = 0;
        for (int i = 0; i < num; i++)
        {
            if (objList[i] == null)
            {
                c++;
            }
        }
        return multipleObjList[c].list;
    }
}
