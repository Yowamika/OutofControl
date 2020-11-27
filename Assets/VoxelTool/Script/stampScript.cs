using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;

public class stampScript : MonoBehaviour
{
    public cubeHolder holder;
    public UnityEngine.UI.Dropdown stampDropdown;


    // (こっちいらなくね…？)
    public List<List<GameObject>> stampList;
    // スタンプのマテリアル番号のリスト
    public List<List<int>> matNumList;
    // 作ったprefabのリスト
    public List<GameObject> prefabList;
    // 表示用
    private GameObject prefabObj;
    // 右クリックの回転角
    private int rotAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        stampList = new List<List<GameObject>>();
        matNumList = new List<List<int>>();
        prefabList = new List<GameObject>();
        prefabObj = null;

        stampDropdown.ClearOptions();
        List<string> list = new List<string>();

        // 読み込みたいフォルダのアドレスを用意
        string path = Application.dataPath + "/VoxelTool/BoxesData/Stamps";

        // GetFilesでフォルダ内にあるファイルを読み込む System.IO
        // GetFiles(フォルダアドレス、読み込みたいファイル名（*で名前を指定しない）、読み込み条件的なやつ）
        // AllDirectoriesだとフォルダ内のフォルダも調べる　TopDirectoryOnlyだとフォルダ直下のやつだけ
        string[] files = Directory.GetFiles(path, "*.csv", System.IO.SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++) 
        {
            stampList.Add(new List<GameObject>());
            matNumList.Add(new List<int>());
            holder.LoadCSV(files[i], stampList[i], matNumList[i]);

            GameObject obj = new GameObject();
            for (int j = 0; j < stampList[i].Count; j++)
            {
                stampList[i][j].transform.parent = obj.transform;
            }
            // プレファブ作成
            // GetFileNameWithoutExtention() 拡張子とかフォルダとかを抜きにしたファイル名だけくれるやつ(string)
            string name = System.IO.Path.GetFileNameWithoutExtension(files[i]);
            var prefab = PrefabUtility.SaveAsPrefabAsset(obj, "Assets/VoxelTool/Stamp/" + name + ".prefab");
            // 子オブジェクト含めて全部のレイヤーを２(レイキャスト対象外)に変更
            prefab.SetLayer(2);
            // シーンから削除  
            // ※DestroyImmediate(object)　Destroyの即座に消すバージョンみたいなやつ　非推奨らしいからやめとく
            Destroy(obj);
            AssetDatabase.SaveAssets();
            prefabList.Add(prefab);
            list.Add(name);
        }

        stampDropdown.AddOptions(list);
        stampDropdown.value = 0;
    }

    // -----------------------------------------------
    // 状態遷移から呼び出す用
    public void Stamp()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if(!prefabObj)
            {
                prefabObj = Instantiate(prefabList[stampDropdown.value]);
            }
            // 子が回転しないように逆回転をかける
            if (hit.transform.name == "QuadFront")
            {
                prefabObj.transform.position = hit.transform.parent.position + new Vector3(0, 0, -1);
                prefabObj.transform.localEulerAngles = new Vector3(270 + rotAngle, 270, 90);
                foreach (Transform obj in prefabObj.transform)
                {
                    obj.localEulerAngles = new Vector3(-270, rotAngle, 0);
                }
            }
            else if (hit.transform.name == "QuadBack")
            {
                prefabObj.transform.position = hit.transform.parent.position + new Vector3(0, 0, 1);
                prefabObj.transform.localEulerAngles = new Vector3(90 - rotAngle, 270, 270);
                foreach (Transform obj in prefabObj.transform)
                {
                    obj.localEulerAngles = new Vector3(-90, rotAngle, 0);
                }
            }
            else if (hit.transform.name == "QuadLeft")
            {
                prefabObj.transform.position = hit.transform.parent.position + new Vector3(-1, 0, 0);
                prefabObj.transform.localEulerAngles = new Vector3(rotAngle, 0, 90);
                foreach(Transform obj in prefabObj.transform)
                {
                    obj.localEulerAngles = new Vector3(0, rotAngle, -90);
                }
            }
            else if (hit.transform.name == "QuadRight")
            {
                prefabObj.transform.position = hit.transform.parent.position + new Vector3(1, 0, 0);
                prefabObj.transform.localEulerAngles = new Vector3(-rotAngle, 0, 270);
                foreach (Transform obj in prefabObj.transform)
                {
                    obj.localEulerAngles = new Vector3(0, rotAngle, -270);
                }
            }
            else if (hit.transform.name == "QuadTop")
            {
                // メモ：岩盤対策する　ポジションだけでいいはず
                if (hit.transform.parent.name == "bedrock")
                {
                    prefabObj.transform.position = new Vector3(Mathf.Ceil(hit.point.x - 0.5f), Mathf.Ceil(hit.point.y), Mathf.Ceil(hit.point.z - 0.5f));
                }
                else
                    prefabObj.transform.position = hit.transform.parent.position + new Vector3(0, 1, 0);
                prefabObj.transform.localEulerAngles = new Vector3(0, rotAngle, 0);
                foreach (Transform obj in prefabObj.transform)
                {
                    obj.localEulerAngles = new Vector3(0, -rotAngle, 0);
                }
            }
            else if (hit.transform.name == "QuadBottom")
            {
                prefabObj.transform.position = hit.transform.parent.position + new Vector3(0, -1, 0);
                prefabObj.transform.localEulerAngles = new Vector3(0, -rotAngle, 180);
                foreach (Transform obj in prefabObj.transform)
                {
                    obj.localEulerAngles = new Vector3(0, -rotAngle, 180);
                }
            }

            // 左クリックでスタンプ置く
            if (Input.GetMouseButtonUp(0))
            {
                // prefabObjのレイヤーを変更(レイヤー番号は変える可能性ある　2以外)
                prefabObj.SetLayer(0);
                int n = 0;
                foreach (Transform obj in prefabObj.transform)
                {
                    // prefabObjの子オブジェクトとholderで重なってるやつがないか調べる
                    // 重なってたらholderの方を消す
                    holder.CompareCubeList(obj.gameObject);
                    holder.AddCubeList(obj.gameObject, matNumList[stampDropdown.value][n]);
                    //// holderにprefabObjの子オブジェクトを入れる
                    //holder.cubeList.Add(obj.gameObject);
                    //// holderのmatの方に子オブジェクトを入れる
                    //holder.cubeMatList.Add(matNumList[SelectStumpNum][n]);
                    n++;
                }
                // 親子関係を解除
                prefabObj.transform.DetachChildren();
                // ポインタを消す
                Destroy(prefabObj);
                prefabObj = null;
            }
            
        }
        else
        {
            Destroy(prefabObj);
            prefabObj = null;
        }

        // 右クリックで回転
        if(Input.GetMouseButtonDown(1))
        {
            rotAngle += 90;
            if (rotAngle >= 360) 
            {
                // 360°超えたら０に戻す
                rotAngle -= 360;
            }
        }
    }

    // -----------------------------------------------
    // 終了時に呼び出されるやつ
    private void OnApplicationQuit()
    {
        string path = Application.dataPath + "/VoxelTool/Stamp";
        string[] files = System.IO.Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);

        // 作ったぷれふぁぶとめたふぁいるをけす
        for (int i = 0; i < files.Length; i++)
        {
            //Debug.Log("delete " + files[i]);
            File.Delete(files[i]);
            File.Delete(files[i] + ".meta");
        }
        
    }
}
