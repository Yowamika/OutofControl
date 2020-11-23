using UnityEngine;
using System.Collections;

public class ShotTest : MonoBehaviour
{

	//　弾のプレハブ
	[SerializeField] private GameObject bullet;
    [SerializeField] private GameObject b2;
	//　レンズからのオフセット値
	[SerializeField] private float offset;
    
    float wait_time = 1.0f;
    float wait;
	// Update is called once per frame
	void Update()
	{
       if(Input.GetMouseButtonDown(1) && wait >= wait_time)
        {
            //　カメラのレンズの中心を求める
            var centerOfLens = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane + offset));
            //　カメラのレンズの中心から弾を飛ばす
            var bulletObj = Instantiate(b2, centerOfLens, Quaternion.identity) as GameObject;
            wait = 0.0f;
        }

		if (Input.GetButton("Fire1") && wait >= wait_time)
		{
			//　カメラのレンズの中心を求める
			var centerOfLens = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane + offset));
			//　カメラのレンズの中心から弾を飛ばす
			var bulletObj = Instantiate(bullet, centerOfLens, Quaternion.identity) as GameObject;
            wait = 0.0f;
		}
        wait += Time.deltaTime;
        if(wait > wait_time)
            wait = wait_time;
	}
}