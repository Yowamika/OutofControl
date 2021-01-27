// リザルトシーン管理スクリプト
// 2020/01/27
// 佐竹晴登

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // リザルトボタンの挙動
    public void PushResultButton()
    {
        // タイトルシーンに遷移
        SceneManager.LoadScene("TitleScene");
    }
}
