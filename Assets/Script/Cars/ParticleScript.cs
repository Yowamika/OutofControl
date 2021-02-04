using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public ParticleSystem explosion;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Object") || collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("Fragment")) //Objectタグの付いたゲームオブジェクトと衝突したか判別
        {
            ParticleSystem exp = Instantiate(explosion, this.transform.position, Quaternion.identity); //パーティクル用ゲームオブジェクト生成
            Destroy(exp, 4f);
            Destroy(this.gameObject); //衝突したゲームオブジェクトを削除
        }
    }
}
