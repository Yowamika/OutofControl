using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointSeener : MonoBehaviour
{
    //https://qiita.com/o8que/items/46e486f62bdf05c29559
    // めちゃくちゃちょうどいいページ　100%パクった人：あさの

    // カメラから見えてたらtrue
    private bool isSeen = false;

    [SerializeField]
    private GameObject canvas;  // UIのキャンバス
    [SerializeField]
    private GameObject wayImage;    // 表示するやつの元のprefab
    private GameObject car;

    private GameObject targetImage;
    RectTransform rectTransform = null;

    public Sprite flagImage;


    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.Find("Buggy");
        canvas = GameObject.Find("Canvas");
        targetImage = Instantiate(wayImage, canvas.transform);
        rectTransform = targetImage.GetComponent<RectTransform>();
        targetImage.GetComponent<Image>().sprite = flagImage;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(this.transform.position, car.transform.position);
        if(dist>20)
        {
            rectTransform.sizeDelta = new Vector2(40, 40);
        }
        else
        {
            if(dist<=5)
            {
                rectTransform.sizeDelta = new Vector2(0, 0);
            }
            else
            {
                float siz = 40 + (60 - dist) * 2;
                rectTransform.sizeDelta = new Vector2(siz, siz);
            }
        }
        //Debug.Log(canvas.GetComponent<RectTransform>().sizeDelta.x);
        if(isSeen)
        {
            rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, this.transform.position);
            //Debug.Log(rectTransform.position);
        }
        else
        {
            float canvasScale = transform.root.localScale.z;
            var center = 0.5f * new Vector3(Screen.width, Screen.height);

            // （画面中心を原点(0,0)とした）ターゲットのスクリーン座標を求める
            var pos = Camera.main.WorldToScreenPoint(this.transform.position) - center;
            Debug.Log(pos);
            if (pos.z < 0f)
            {
                pos.x = -pos.x;
                pos.y = -pos.y;
            }
            var halfSize = 0.5f * canvasScale * rectTransform.sizeDelta;
            float d = Mathf.Max(
                Mathf.Abs(pos.x / (center.x - halfSize.x)),
                Mathf.Abs(pos.y / (center.y - halfSize.y))
            );

            // ターゲットのスクリーン座標が画面外なら、画面端になるよう調整する
            bool isOffscreen = (pos.z < 0f || d > 1f);
            if (isOffscreen)
            {
                d *= 0.75f;
                pos.x /= d;
                pos.y /= d;
            }
            rectTransform.anchoredPosition = pos / canvasScale;
        }
    }

    void OnBecameVisible()
    {
        // 表示されるようになった時の処理
        isSeen = true;
    }
    void OnBecameInvisible()
    {
        // 表示されなくなった時の処理
        isSeen = false;
    }

    public GameObject GetTargetImage()
    {
        return targetImage;
    }

    void OnDestroy()
    {
        Destroy(targetImage);
    }
}
