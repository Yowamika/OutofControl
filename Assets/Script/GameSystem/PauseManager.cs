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

    //ゴールの音声
    public AudioClip sound;
    AudioSource audioSource;

    void Start()
    {
        Time.timeScale = 1;  // 再開
        // 最初はポーズ画面は非表示
        pausePanel.SetActive(false);
        // ゲーム再開ボタンにResume関数をアタッチ
        resumeButton.onClick.AddListener(Resume);
        // タイトルに戻るボタンにReturnTitle関数をアタッチ
        returnButton.onClick.AddListener(ReturnTitle);

        //Componentを取得
        audioSource = GetComponent<AudioSource>();
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
        //ボタン押したら音声を鳴らす
        audioSource.PlayOneShot(sound);
        SceneManager.LoadScene("TitleScene");
        
    }
    // ゲームを再開
    private void Resume()
    {
        //ボタンを押したら音声を鳴らす
        audioSource.PlayOneShot(sound);
        Time.timeScale = 1;  // 再開
        pausePanel.SetActive(false);
    }
    // ポーズ画面に移行
    private void OnPause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }
    // ポーズかどうかを取得、設定
    public bool Pause
    {
        get { return pauseFlag; }
        set { pauseFlag = value; }
    }
    // ポーズフラグのゲッタ
    public bool GetFlag()
    {
        return pauseFlag;
    }
}