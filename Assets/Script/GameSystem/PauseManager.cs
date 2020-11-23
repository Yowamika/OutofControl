using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // パネル
    [SerializeField]
    private GameObject pausePanel;
    // 再開ボタン
    [SerializeField]
    private Button resumeButton;
    // タイトルに戻るボタン
    [SerializeField]
    private Button returnButton;

    // 停止のフラグ
    private bool pauseFlag = false;

    void Start()
    {
        // 最初はポーズ画面は非表示
        pausePanel.SetActive(false);
        // ゲーム再開ボタンにResume関数をアタッチ
        resumeButton.onClick.AddListener(Resume);
        // タイトルに戻るボタンにReturnTitle関数をアタッチ
        returnButton.onClick.AddListener(ReturnTitle);
    }

    private void Update()
    {
        // escapeキーが押された時
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // フラグを入れ替える
            pauseFlag = !pauseFlag;
            if(pauseFlag)
            {
                OnPause();
            }
            else
            {
                Resume();
                
            }
        }
    }
    // タイトルに戻る
    private void ReturnTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
    // ゲームを再開
    private void Resume()
    {
        Time.timeScale = 1;  // 再開
        pausePanel.SetActive(false);
    }
    // ポーズ画面に移行
    private void OnPause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }
    // ポーズかどうかを取得
    public bool Pause
    {
        get
        {
            return pauseFlag;
        }
    }
}