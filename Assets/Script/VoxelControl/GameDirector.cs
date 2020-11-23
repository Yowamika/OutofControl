// ゲームの演出面の管理
// 2020/11/21
// 佐竹晴登

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{

    // ゴール
    [SerializeField]
    Goal goalObj;
    // タイマー
    [SerializeField]
    Timer timerObj;
    // ゴールテキスト
    [SerializeField]
    GameObject goalText;
    // ゲーム始まる
    public bool gameStart = false;
    // カウントダウン開始
    bool countDownStart = false;
    // 
    [SerializeField]
    AudioClip bgm;

    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // ゴールになったら
        if(goalObj.GetGoalFlag())
        {
            goalText.SetActive(true);
        }
        if(!audioSource.isPlaying && countDownStart)
        {
            audioSource.PlayOneShot(bgm);
        }
        gameStart = true;
    }
    public void CountStart()
    {
        countDownStart = true;
        audioSource.PlayOneShot(bgm);
    }
    public void SetGameStart()
    {
        gameStart = true;
        
    }
}
