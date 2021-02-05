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
    // ゲーム始まる
    bool gameStart = false;
    // カウントダウン開始
    bool countDownStart = false;
    // チェックポイントの最大数
    int maxCheckPoint;
    // チェックポイント通過状況
    int checkPointStatus = 0;
    

    // チェックポイント情報
    List<CheckPoint> checkpointList = new List<CheckPoint>();
    AudioSource audioSource;
    
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        // ゴールオブジェクトを非表示にする
        goalObj.gameObject.SetActive(false);
        //goalObj.GetComponent<CheckPointSeener>().GetTargetImage().SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // チェックポイントを全て通過したらゴールを出現
        if (gameStart)
        {
            if (checkPointStatus >= maxCheckPoint)
            {
                goalObj.gameObject.SetActive(true);
            }
        }
    }
    public void SetGameStart()
    {
        gameStart = true;
        GameObject[] gos = GameObject.FindGameObjectsWithTag("CheckPoint");
        foreach (var g in gos)
        { 
            CheckPoint cp = g.GetComponent<CheckPoint>();
            checkpointList.Add(cp);
            cp.gameObject.SetActive(false);
        }
        // チェックポイント確認
        CheckCheckPointVisable();
    }
    /// <summary>
    /// 今存在すべきチェックポイントはどれか確認
    /// </summary>
    void CheckCheckPointVisable()
    {
        foreach(CheckPoint cp in checkpointList)
        {
            if(cp.GetCheckPointNumber() == checkPointStatus)
            {
                cp.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// チェックポイントの最大値を設定する
    /// </summary>
    /// <param name="value"></param>
    public void SetMaxCheckPoint(int value)
    {
        maxCheckPoint = value;
    }
    /// <summary>
    /// チェックポイントを通過していいかどうか
    /// </summary>
    /// <param name="CPnumber">チェックポイントナンバー</param>
    /// <returns>通過していいかどうか</returns>
    public bool CheckPointCheck(int CPnumber)
    {
        // チェックポイント通過状況が一つ前（-1）だったら通過してヨシ
        if(checkPointStatus == CPnumber)
        {
            // 通過状況を更新する
            checkPointStatus++;
            if(maxCheckPoint != checkPointStatus)
                CheckCheckPointVisable();

            return true;
        }
        return false;
    }
}
