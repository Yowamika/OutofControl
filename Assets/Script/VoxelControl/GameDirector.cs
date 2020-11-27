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
    bool gameStart = false;
    // カウントダウン開始
    bool countDownStart = false;

    [SerializeField]
    AudioClip countdown;
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
        if (gameStart)
        {
            audioSource.PlayOneShot(countdown);
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(bgm);
            }
        }
    }
    public void SetGameStart()
    {
        gameStart = true;
    }
}
