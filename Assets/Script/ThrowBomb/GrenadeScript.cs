using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    ObjectGenerator generator;
   
    // Start is called before the first frame update
    public void Start()
    {
        // ディレクタ―オブジェクトを取得
        generator = GameObject.Find("Director").GetComponent<ObjectGenerator>();
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
            // ディレクターから範囲内のオブジェクトを取得する
            Cube[] cubes = generator.GetCubeInRange(3.0f, this.transform.position);
            // もらったオブジェクトを爆発させる
            foreach (var c in cubes)
            {
                c.ExplodeMe(1000f, this.transform.position, 15f);
            }
            Destroy(this.gameObject);
        }
    }

}
