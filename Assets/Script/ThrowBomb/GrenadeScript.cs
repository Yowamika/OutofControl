using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    // オブジェクト生成スクリプト
    ObjectGenerator generator;
    // オブジェクト分離スクリプト
    CubeSprit spritter;
   
    // Start is called before the first frame update
    public void Start()
    {
        GameObject director = GameObject.Find("Director");
        // ディレクタ―オブジェクトを取得
        generator = director.GetComponent<ObjectGenerator>();
        // オブジェクト分離スクリプト取得
        spritter = director.GetComponent<CubeSprit>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 当たり判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Object") || collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("Fragment"))
        {
            GameObject root = null;
            // collisionは親
            root = collision.transform.gameObject;
            
            // ディレクターから範囲内のオブジェクトを取得する
            Cube[] cubes = generator.GetCubeInRange(4.5f, this.transform.position);
            
            // もらったオブジェクトを爆発させる
            foreach (var c in cubes)
            {
                c.ExplodeMe(2000f, this.transform.position, 30f);
            }
            if(cubes.Length > 0)
            {
                //// オブジェクトの分離処理
                spritter.CheckSplit(root);
            }
            Destroy(this.gameObject);
        }
    }

}
