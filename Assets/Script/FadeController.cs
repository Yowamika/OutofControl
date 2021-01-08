using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //パネルのイメージを操作するのに必要


public class FadeController : MonoBehaviour
{

    float fadeSpeed = 0.07f;        //透明度が変わるスピードを管理
    float red, green, blue, alfa;   //パネルの色、不透明度を管理

    public bool isFadeOut = false;  //フェードアウト処理の開始、完了を管理するフラグ
    public bool isFadeIn = false;   //フェードイン処理の開始、完了を管理するフラグ
    private bool alfaOne = false;

    Image fadeImage;                

    // 車ぶち込んで
    [SerializeField]
    GameObject car;

    // カメラぶち込んで
    [SerializeField]
    new GameObject camera;

    public Transform target;

    //Car.CarController carController;
    BuggyControl buggyControl;

    CameraController cameraController;

    //Car.CarUserControl userController;


    public bool GetAlfa()
    {
        return alfaOne;
    }

    void Start()
    {
        fadeImage = GetComponent<Image>();
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = 0.0f;
        //carController = car.GetComponent<Car.CarController>();
        buggyControl = car.GetComponent<BuggyControl>();
        cameraController = camera.GetComponent<CameraController>();
        //userController = car.GetComponent<Car.CarUserControl>();
    }

    void Update()
    {
        // リスタートが押されたら
        if (buggyControl.GetRestarted())
        {
            // フェードアウトの状態に
            isFadeOut = true;
            // リスタート状態ではなくする
            //carController.SetRestarted(false);
            buggyControl.SetRestarted(false);
            // 時間を止める
            Time.timeScale = 0;
        }

        // 暗くなる
        if (isFadeOut)
        {
            StartFadeOut();
        }

        // 再開
        if (isFadeIn)
        {
            
            StartFadeIn();
        }

       

    }

    void StartFadeIn()
    {
       
        alfa -= fadeSpeed;                //a)不透明度を徐々に下げる
        SetAlpha();                      //b)変更した不透明度パネルに反映する
        if (alfa <= 0)
        {                    //c)完全に透明になったら処理を抜ける
            isFadeIn = false;
            fadeImage.enabled = false;    //d)パネルの表示をオフにする
            alfaOne = false;
            Time.timeScale = 1;
        }
    }

    void StartFadeOut()
    {
        fadeImage.enabled = true;  // a)パネルの表示をオンにする
        alfa += fadeSpeed;         // b)不透明度を徐々にあげる
        SetAlpha();               // c)変更した透明度をパネルに反映する
        if (alfa >= 1)
        {             // d)完全に不透明になったら処理を抜ける

            // 車のリセット
            //carController.SetSpeed(new Vector3(0f, 0f, 0f));
            buggyControl.SetSpeed(new Vector3(0f, 0f, 0f));
            alfaOne = true;

            // カメラのリセット
            float distance = 5.2f;
            Vector3 offset = new Vector3(0f, 0f, -distance);
            cameraController.transform.position = target.TransformPoint(offset);
            cameraController.transform.LookAt(target, Vector3.up);

            // フェードの切り替え
            isFadeOut = false;
            isFadeIn = true;
        }
    }

    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
}
