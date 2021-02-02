using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrow : MonoBehaviour
{
    Vector3 cursorPosition;
    Vector3 cursorPosition3d;
    RaycastHit hit;

    Vector3 cameraPos;
    Vector3 throwDirection;

    [SerializeField]
    private GameObject PosParent;

    private Vector3 Pos;

    public GameObject ball;
    Rigidbody rb_ball;

    private int BombStock = 3;

    public float thrust = 1.0f;
    Rigidbody carRigid;

    // インターバルの固定値
    const float INTERVAL = 5f;

    //ボムの上限
    const int BOMBMAX = 3; 

    // インターバルのカウント
    float count = 0f;

    AudioSource audioSource;

    [SerializeField]
    AudioClip[] clip;

    // ロード
    [SerializeField]
    CountDown countDown;
    // ポーズマネージャー
    [SerializeField]
    PauseManager pauseM;

    // 点滅フラグ
    bool isBlink=false;

    // 復活フラグ
    bool isRevival = false;

    // 使用後フラグ
    bool isBombed = false;



    // Start is called before the first frame update
    void Start()
    {
        carRigid = this.GetComponent<Rigidbody>();
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!pauseM.GetFlag())
        {
            if (Camera.main != null)
            {
                Pos = PosParent.transform.position;
                Pos = new Vector3(Pos.x, Pos.y + 4.5f, Pos.z); // グレの位置を車両の近くに設定           

                cameraPos = Camera.main.transform.position; // カメラの位置

                cursorPosition = Input.mousePosition; // 画面上のカーソルの位置

                cursorPosition.z = Vector3.Distance(cameraPos, this.transform.position) + 20f; // z座標にメインカメラから車の距離を入れる

                cursorPosition3d = Camera.main.ScreenToWorldPoint(cursorPosition); // 3Dの座標になおす
                throwDirection = cursorPosition3d - this.transform.position; // 玉を飛ばす方向

                throwDirection.y += 2.0f; // グレの飛ぶ高さを指定

                

                // インターバルをしっかりとつけていくぅ
                if (BombStock != 0 && countDown.GetCountEnd())
                {
                    if (Input.GetMouseButtonDown(0)) // マウスの左クリックを押したとき
                    {
                        rb_ball = Instantiate(ball, Pos, transform.rotation).GetComponent<Rigidbody>(); // 玉を生成
                        rb_ball.transform.parent = PosParent.transform; // 車両をグレネードの親に設定

                        // 点滅フラグをONにする
                        isBlink = true;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {

                        // 点滅フラグをOFFにする
                        isBlink = false;

                        BombStock--;

                        if (rb_ball != null)
                        {
                            rb_ball.isKinematic = false;
                            rb_ball.AddForce(throwDirection + carRigid.velocity, ForceMode.Impulse); // カーソルの方向に力を一度加える
                            audioSource.PlayOneShot(clip[0]);
                            //GrenadeScript grenade = rb_ball.GetComponent<GrenadeScript>(); // 爆発するスクリプトの取得
                            //grenade.enabled = true; // 爆発スクリプトをアクティブに変更    
                        }
                    }

                }


                if (BOMBMAX > BombStock)
                {
                    if (count >= INTERVAL)
                    {
                        BombStock++;                   
                        audioSource.PlayOneShot(clip[1]);
                        count = 0f;
                    }
                }
     

                count += Time.deltaTime;
                //Debug.Log(BombStock);
            }

        }
    }

    public int Count()
    {
        return BombStock;
    }

    public bool GetIsBlink()
    {
        return isBlink;
    }
    

}
