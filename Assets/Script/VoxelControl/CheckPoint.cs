// チェックポイントのスクリプト
// 2020/02/04
// 佐竹晴登

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // ゲームディレクター
    GameDirector director;
    // チェックポイントナンバー
    int checkPointNumber;
    // 削除する時間
    public const float DESTROY_TIME = 5f;
    // Start is called before the first frame update
    void Start()
    {
        // ゲームディレクターを取得
        director = GameObject.Find("Director").GetComponent<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// トリガー接触  
    /// </summary>
    /// <param name="other">衝突した相手のコライダー情報</param>
    private void OnTriggerEnter(Collider other)
    {
        // 車(プレイヤー)とぶつかった場合
        if (other.gameObject.tag == "Player")
        {
            // ディレクターに通過していいか判定してもらう
            if(director.CheckPointCheck(checkPointNumber))
            {
                // チェックポイント通過アニメーション開始

                // 自オブジェクトを削除する
                Destroy(this.gameObject,2f);
            }
        }
    }
    /// <summary>
    /// チェックポイントナンバーを設定する
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetCheckPointNumber(int value)
    {
        checkPointNumber = value;
    }
}
