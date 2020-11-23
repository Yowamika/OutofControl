// ボックスのオブジェクト
// 2020/09/18
// 佐竹晴登

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // ディレクターオブジェクト
    ObjectGenerator DirectorObject;
    // 破片の当たり判定
    BoxCollider bCol;
    // 物理
    Rigidbody rigid;
    // メッシュレンダラー
    MeshRenderer mesh;
    // 破片となった時の大きさ
    const float FragmentScale = 80f;
    // 削除命令が出てから消えるまでの時間
    const float DestroyTime = 5.0f;
    //// オブジェクトが吹き飛ぶ威力
    //const float ExpPower = 300.0f;

    // ---------------------------------------------------
    // このオブジェクトが有効になった時
    private void OnEnable()
    {
        DirectorObject = GameObject.Find("Director").GetComponent<ObjectGenerator>();
        bCol = GetComponent<BoxCollider>();
        rigid = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
    }
    // ---------------------------------------------------
    // アップデート関数
    public void UpdateMe()
    {

    }
    // ------------------------------------------------------
    // 自身を爆発させる関数
    // power  爆発する力
    // center 爆発する中心点
    // radius 爆発半径
    public void ExplodeMe(float power, Vector3 center, float radius)
    {
        // 先に自身を崩れさせる
        CollapseMe();
        // 爆発処理をする
        this.rigid.AddExplosionForce(power, center, radius);
        // 軽量化処理
        // 接しているオブジェクトを取得
        //Cube[] cubes = DirectorObject.GirthCheck(this);
        //// そのオブジェクトが可視状態かを取得
        //foreach(var c in cubes)
        //{
        //    if(!c.GetVisable())
        //    {
        //        DirectorObject.CheckVisible(c);
        //    }
            
        //}
           
    }
    // ------------------------------------------------------
    // 自身を崩れさせる関数
    private void CollapseMe()
    {
        // 親から独立させて物理挙動をONにする
        // 親がいる場合は崩れる処理
        if (this.transform.parent != null)
        {
            // 親の繋がりを絶つ
            this.transform.parent = null;
            // 詰まらないように破片の大きさを80%にする
            this.transform.localScale = new Vector3(FragmentScale, FragmentScale, FragmentScale);
            // 物理挙動をONにする
            rigid.isKinematic = false;
            // 可視化する
            mesh.enabled = true;
            // 削除申請を出しておく（消されるのは数秒後
            Destroy(this.gameObject, DestroyTime);
        }
    }

    
    // 可視状態かどうかを取得
    public bool GetVisable()
    {
        return mesh.enabled;
    }
    // 可視不可視を切り替える
    // value true = 表示する false = 表示しない
    public void SetVisable(bool value)
    {
        mesh.enabled = value;
    }
}