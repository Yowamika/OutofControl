using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    CountDown countDown;
    public static int minute = 0;

    public static float seconds = 0.0f;

    //　前のUpdateの時の秒数
    private float oldSeconds;
    //　タイマー表示用テキスト
    private Text timerText;
    // ロード
    [SerializeField]
    private ObjectGenerator load;

    [SerializeField]
    Goal goal;

    void Start()
    {
        minute = 0;
        seconds = 0.0f;
        oldSeconds = 0.0f;
        timerText = GetComponentInChildren<Text>();
    }

    void Update()
    {
        if(countDown.GetGameStart())
        {
            if (load.GetStageLoad())
            {

                if (goal.GetGoalFlag() == false)
                {

                    seconds += Time.deltaTime;
                    if (seconds >= 60.0f)
                    {
                        minute++;
                        seconds = seconds - 60.0f;
                    }
                }
                //　値が変わった時だけテキストUIを更新
                if ((int)seconds != (int)oldSeconds)
                {
                    timerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
                }
                oldSeconds = seconds;
            }
        }
    }

    public static float GetSecond()
    {
        return seconds;
    }

    public static int GetMinute()
    {
        return minute;
    }
}