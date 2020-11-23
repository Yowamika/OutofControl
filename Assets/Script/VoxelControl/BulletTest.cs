using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class BulletTest : MonoBehaviour
{

	[SerializeField] private float power = 50f;
	[SerializeField] private float deleteTime = 10f;

	// ディレクターオブジェクト
	ObjectGenerator generator;

	private Rigidbody rigid;
	private Ray ray;
	private SphereCollider sCollider;
    
	bool destroyflg;
	void Awake()
	{
		//　Rigidbodyを取得し速度を0に初期化
		rigid = GetComponent<Rigidbody>();
		// SphereColliderを取得
		sCollider = GetComponent<SphereCollider>();
		// ディレクタ―オブジェクトを取得
		generator = GameObject.Find("Director").GetComponent<ObjectGenerator>();
	}

	//　弾がアクティブになった時
	void OnEnable()
	{
        
		//　カメラからクリックした位置にレイを飛ばす
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		//　弾を発射してから指定した時間が経過したら自動で削除
		Destroy(this.gameObject, deleteTime);
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Object") || col.gameObject.CompareTag("Block") || col.gameObject.CompareTag("Fragment"))
		{
			// ディレクターから範囲内のオブジェクトを取得する
			Cube[] cubes = generator.GetCubeInRange(3.0f, this.transform.position);
			// もらったオブジェクトを爆発させる
			foreach (var c in cubes)
			{
				c.ExplodeMe(1000f, this.transform.position, 10f);
			}
			Destroy(this.gameObject);
		}

	}
	//　弾が存在していればレイの方向に力を加える
	void FixedUpdate()
	{
		rigid.AddForce(ray.direction * power, ForceMode.Force);
	}
}
