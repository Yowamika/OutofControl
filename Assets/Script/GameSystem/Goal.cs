using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // 当たりましたわよ
    bool GoalHit = false;

    [SerializeField] GameObject mainCamera;      //メインカメラ格納用
    [SerializeField] GameObject subCamera;       //サブカメラ格納用 

    float time=0.0f;



    // Start is called before the first frame update
    void Start()
    {
        //サブカメラを非アクティブにする
        subCamera.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // トリガー判定
    private void OnCollisionEnter(Collision collision)
    {
        // プレイヤーに当たったら
        if (collision.gameObject.CompareTag("Player") && !GoalHit)
        {
           
            GoalHit = true;
            FadeManager.Instance.LoadScene("ResultScene", 1.0f);
            Debug.Log("ゴール");

            
            //サブカメラをアクティブに設定
            mainCamera.SetActive(false);
            subCamera.SetActive(true);
            
            
        }
       
    
    }
    // ゲッタ
    public bool GetGoalFlag()
    {
        return GoalHit;
    }

}
