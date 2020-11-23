using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // 当たりましたわよ
    bool GoalHit = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    // トリガー判定
    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに当たったら
        if (other.gameObject.tag == "Player" && !GoalHit)
        {
            GoalHit = true;
            Debug.Log("ゴール");
        }
    }
    // ゲッタ
    public bool GetGoalFlag()
    {
        return GoalHit;
    }

}
