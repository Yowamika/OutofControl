using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    float time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 時間
        time += Time.deltaTime;

        if (time <= 2.0f)
        {
            this.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
        }
        transform.Rotate(new Vector3(0, 0.5f, 0));
       

    }
}
