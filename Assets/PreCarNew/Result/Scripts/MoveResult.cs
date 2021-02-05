using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveResult : MonoBehaviour
{

    float time;

    bool isEnd=false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 時間
        time += Time.deltaTime;

       
        if (time <= 1.0f)
        {
            this.transform.Translate(0.0f, -4.0f, 0.0f);
            
        }
        else
        {
            isEnd = true;
        }
    }

    public bool GetEnd()
    {
        return isEnd;
    }
}
