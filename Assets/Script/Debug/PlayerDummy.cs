// デバッグ用のプレイヤー
// 2020/11/09
// 佐竹晴登
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    // 物理
    Rigidbody rigid;
    // --------------------------------------------------------
    // 最初のフレーム
    void Start()
    {
        
    }

    // --------------------------------------------------------
    // 更新関数
    void Update()
    {
        // 移動処理
        Movement();
    }
    // --------------------------------------------------------
    // 移動関数
    void Movement()
    {
        // 前進
        if(Input.GetKey(KeyCode.W))
            this.transform.Translate(0.0f, 0.0f, 0.5f);
        // 後退
        else if(Input.GetKey(KeyCode.S))
            this.transform.Translate(0.0f, 0.0f, -0.5f);
        // 左回転
        if (Input.GetKey(KeyCode.A))
            this.transform.Rotate(0.0f, -1.0f, 0.0f);
        // 右回転
        else if (Input.GetKey(KeyCode.D))
            this.transform.Rotate(0.0f, 1.0f, 0.0f);
    }
}
