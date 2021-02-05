using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultTime : MonoBehaviour
{
    int minute;
    float second;

    private Text resultText;

    float time;

    [SerializeField]
    MoveResult result;

    // Start is called before the first frame update
    void Start()
    {
        resultText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // 時間
        if (result.GetEnd())
        {
            time += Time.deltaTime;
        }

        minute = Timer.GetMinute();
        second = Timer.GetSecond();

        resultText.text = minute.ToString("00") + ":" + ((int)second).ToString("00");

        if (time <= 1.0f&& result.GetEnd())
        {
            this.transform.Translate(-12.0f, 0.0f, 0.0f);
        }
    }
}
