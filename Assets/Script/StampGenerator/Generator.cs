using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    // ステージ生成用スクリプト
    [SerializeField]
    ObjectGenerator objectGenerator;

    string csvDataFile = "/Resources/Data/csv/Stamp/";
    private void Awake()
    {
        
    }
    /// <summary>
    /// スタンプファイルを基にプレファブを生成する関数
    /// </summary>
    void PrefabGenerate()
    {

    }
}
