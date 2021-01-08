using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    //弾道を表示するための点のプレハブ
    [SerializeField]
    private GameObject DummyObjPref;

    //弾道を表示する点の親オブジェクト Grenade
    private Transform DummyObjParent;

    //クルマオブジェクト
    private Transform CarTransform;


    //初速のベクトル
    [SerializeField]
    private Vector3 v0;

    //弾道予測の点の数
    [SerializeField]
    private int dummyCount;

    //弾道を表示する間隔の秒数
    [SerializeField]
    private float secInterval;

    //点が移動する速度
    [SerializeField]
    private float offsetSpeed = 0.5f;

    private float offset;

    Vector3 cursorPosition;
    Vector3 cursorPosition3d;
    RaycastHit hit;

    Vector3 cameraPos;
    Vector3 throwDirection;

    Vector3 Pos;
    private float GreHight = 2f; 


    private List<GameObject> dummySphereList = new List<GameObject>();
    private Rigidbody rigid;
    // Use this for initialization
    void Start()
    {
        //rigid = GetComponent<Rigidbody>();
        //if (!rigid)
        //    rigid = gameObject.AddComponent<Rigidbody>();
        //rigid.isKinematic = true;
        // DummyObjParent.transform.position = transform.position; // 親の位置に影響しない

        DummyObjParent = this.gameObject.transform;

        CarTransform = GameObject.Find("Buggy").transform;

        //弾道予測を表示するための点を生成
        for (int i = 0; i < dummyCount; i++)
        {
            var obj = (GameObject)Instantiate(DummyObjPref, DummyObjParent);
            dummySphereList.Add(obj);
        }

    }


    // Update is called once per frame
    void Update()
    {
        CarTransform = CarTransform.transform;

        // グレネードの座標を取得
        Pos = CarTransform.position;
        Pos = new Vector3(Pos.x, Pos.y+GreHight, Pos.z);

        offset = Mathf.Repeat(Time.time * offsetSpeed, secInterval);

        cameraPos = Camera.main.transform.position; // カメラの位置

        cursorPosition = Input.mousePosition; // 画面上のカーソルの位置
        cursorPosition.z = Vector3.Distance(cameraPos, this.transform.position) + 10f; //  z座標にメインカメラから車の距離を入れる
        cursorPosition3d = Camera.main.ScreenToWorldPoint(cursorPosition); // 3Dの座標になおす

        throwDirection = cursorPosition3d - Pos; // 玉を飛ばす方向
        throwDirection.y += 4.0f;

        //弾道予測の位置に点を移動
        for (int i = 0; i < dummyCount; i++)
        {
            var t = (i * secInterval) + offset;
            var x = t * throwDirection.x;
            var z = t * throwDirection.z;
            var y = (throwDirection.y * t) - 0.5f * (-Physics.gravity.y) * Mathf.Pow(t, 2.0f);
            dummySphereList[i].transform.position = Pos + new Vector3(x, y, z);
        }

        if (Input.GetMouseButtonUp(0)) // マウスの左クリックを離したとき
        {
            Invoke("Delete", 0.0f);
        }
    }

    void Delete()
    {
        for (int i = 0; i < dummyCount; i++)
        {
            Destroy(dummySphereList[i]);
        }
        Destroy(this);
    }

}
