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

    // スタンプの名前
    public List<string> stampNameList;
    // 結局使わんかったじゃんお前…
    public List<List<GameObject>> stampPosList;
    // スタンプのマテリアル番号のリスト
    public List<List<int>> matNumList;
    // 作ったprefabのリスト
    public List<GameObject> prefabList;
    // 表示用
    private GameObject prefabObj;
    // 右クリックの回転角
    private int rotAngle = 0;

    // 一個前に選んでたQuad
    private GameObject prevQuad = null;
    // 配置したいスタンプが配置済スタンプと重なってないか　重なってたらtrue
    private bool fIsPiled = false;

    // Start is called before the first frame update
    void Start()
    {
        stampPosList = new List<List<GameObject>>();
        matNumList = new List<List<int>>();
        prefabList = new List<GameObject>();
        prefabObj = null;

        stampDropdown.ClearOptions();
        stampNameList = new List<string>();

        // 読み込みたいフォルダのアドレスを用意
        string path = Application.dataPath + "/VoxelTool/BoxesData/Stamps";

        // GetFilesでフォルダ内にあるファイルを読み込む System.IO
        // GetFiles(フォルダアドレス、読み込みたいファイル名（*で名前を指定しない）、読み込み条件的なやつ）
        // AllDirectoriesだとフォルダ内のフォルダも調べる　TopDirectoryOnlyだとフォルダ直下のやつだけ
        string[] files = Directory.GetFiles(path, "*.csv", System.IO.SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++) 
        {
            stampPosList.Add(new List<GameObject>());
            matNumList.Add(new List<int>());
            holder.LoadCSV(files[i], stampPosList[i], matNumList[i]);

            GameObject obj = new GameObject();
            for (int j = 0; j < stampPosList[i].Count; j++)
            {
                stampPosList[i][j].transform.parent = obj.transform;
            }
            // prefab作成
            // GetFileNameWithoutExtention() 拡張子とかフォルダとかを抜きにしたファイル名だけくれるやつ(string)
            string name = System.IO.Path.GetFileNameWithoutExtension(files[i]);
            var prefab = PrefabUtility.SaveAsPrefabAsset(obj, "Assets/VoxelTool/Stamp/" + name + ".prefab");
            // シーンから削除
            Destroy(obj);
            // 作ったprefabを保存
            AssetDatabase.SaveAssets();
            // listに追加
            prefabList.Add(prefab);
            stampNameList.Add(name);
        }

        // dropdownに名前を追加
        stampDropdown.AddOptions(stampNameList);
        stampDropdown.value = 0;
    }

    /// <summary>
    /// 状態遷移から呼び出す用
    /// スタンプモード
    /// </summary>
    public void Stamp()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            // スタンプ生成
            if(!prefabObj)
            {
                prefabObj = Instantiate(prefabList[stampDropdown.value]);
                // 子オブジェクト含めて全部のレイヤーを２(レイキャスト対象外)に変更
                prefabObj.SetLayer(2);
            }

            
            // 右クリックで回転
            if (Input.GetMouseButtonDown(1))
            {
                rotAngle += 90;
                if (rotAngle >= 360)
                {
                    // 360°超えたら０に戻す
                    rotAngle -= 360;
                }
            }

            // 何回もスタンプの接触判定してたらすごい重くなる気がしたので必要最低限のUpdate
            if (!prevQuad || prevQuad != hit.transform.gameObject || hit.transform.parent.name == "bedrock" || Input.GetMouseButtonDown(1))
            {
                prevQuad = hit.transform.gameObject;

                // 祝☆簡略化
                Vector3 pos = holder.GetTargetPos(hit);
                // いい感じのポジションを手に入れる　jointScriptと一緒
                prefabObj.transform.position = pos;
                // prefabの上をhitしたQuadの上と一緒にする
                prefabObj.transform.up = -hit.transform.forward;
                // rotangle分回転
                prefabObj.transform.Rotate(Vector3.up, rotAngle);
                Quaternion rot = prefabObj.transform.rotation;
                rot = Quaternion.Inverse(rot);
                // 子オブジェクトに逆回転をかける
                foreach (Transform obj in prefabObj.transform)
                {
                    obj.localRotation = rot;
                }


                //// 重なり判定
                fIsPiled = holder.CompareStampList(prefabObj);
                if (fIsPiled)
                {
                    // さっきまで赤色だった可能性があるので白にする
                    var childConponents = prefabObj.GetComponentsInChildren<Renderer>();

                    foreach (var childRenderer in childConponents)
                    {
                        childRenderer.GetComponentInChildren<Renderer>().material.color = Color.red;
                    }
                }
                else
                {
                    // 配置済スタンプと重なってるので置けない　赤色にする
                    var childConponents = prefabObj.GetComponentsInChildren<Renderer>();

                    foreach (var childRenderer in childConponents)
                    {
                        childRenderer.GetComponentInChildren<Renderer>().material.color = Color.white;
                    }
                }
            }

            // 左クリックでスタンプ置く
            if (Input.GetMouseButtonUp(0) && !fIsPiled)
            {
                // prefabObjのレイヤーを変更(レイヤー番号は変える可能性ある　2以外)
                prefabObj.SetLayer(0);
                foreach (Transform obj in prefabObj.transform)
                {
                    // prefabObjの子オブジェクトとholderで重なってるやつがないか調べる
                    // 重なってたらholderの方を消す
                    holder.CompareCubeList(obj.gameObject);
                }
                // 名前を変更　というか(Instance)を消す
                prefabObj.name = stampDropdown.captionText.text;
                // 配置中のスタンプリストに追加
                holder.stampList.Add(prefabObj);
                // ポインタを消す
                prefabObj = null;
            }
            
        }
        else
        {
            Destroy(prefabObj);
            prefabObj = null;
            prevQuad = null;
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
