using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TimeLocation : MonoBehaviour
{
    // 座標,回転
    private Vector3 lastPos;
    private Quaternion lastRot;
    

    float time;

    // 車ぶち込んで
    [SerializeField]
    GameObject car;

    // 座標の情報を持ってくるため
    //Car.CarController carController;
    BuggyControl buggycontrol;

    // fadeControllerをぶち込んで
    [SerializeField]
    FadeController fade;

    // Start is called before the first frame update
    void Start()
    {
        // 車の情報
        buggycontrol = car.GetComponent<BuggyControl>();

        lastPos = buggycontrol.GetPos();

        lastRot = buggycontrol.GetRot();
    }

    // Update is called once per frame
    void Update()
    {


       

        // 時間
        time += Time.deltaTime;

        if(time>=5.0f)
        {
            // 座標と角度を格納する
            lastPos = buggycontrol.GetPos();
            lastRot = buggycontrol.GetRot();

            time = 0.0f;

            
        }

        if (fade.GetAlfa())
        {
            // 先頭の要素の座標にセットする
            buggycontrol.SetPos(lastPos);
            buggycontrol.SetRot(lastRot);

            Debug.Log("trete");
        }




    }



}

