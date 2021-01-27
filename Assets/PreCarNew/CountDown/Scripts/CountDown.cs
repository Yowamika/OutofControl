using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountDown : MonoBehaviour
{
    // ロード
    [SerializeField]
    private ObjectGenerator load;

    // audioSourceの変数
    [SerializeField]
    AudioSource audioSource;

    // それぞれの効果音
    [SerializeField]
    AudioClip[] se;


    // 子オブジェクトを見つける
    GameObject child;

    float time;

    private bool isEnd = false;

    // スタート時のカウント音声
    void CountSound()
    {
        audioSource.PlayOneShot(se[0]);
    }

    // スタート時の音声
    void StartSound()
    {
        audioSource.PlayOneShot(se[1]);
    }

    private void Start()
    {
        // 子オブジェクトを見つける
        child = this.transform.Find("Count").gameObject;

        time = 0.0f;
    }
    // Update is called once per frame
    void Update()
    {
        if (load.GetStageLoad())
        {
            time += Time.deltaTime;

            child.SetActive(true);
            GetComponent<Animator>().SetTrigger("CountDownTrigger");
        }

        if(time>3.5f)
        {
            isEnd = true;
            this.transform.Rotate(0.0f, 0.0f, 15.0f);
            this.transform.localScale += new Vector3(-0.1f, -0.1f, -0.1f);

            if (time > 5.0f)
            {
                Destroy(this);
                Destroy(child);
            }
        }
    }

    public bool GetCountEnd()
    {
        return isEnd;
    }
}
