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
    // メッシュレンダラー
    MeshRenderer mesh;
    // レンダラー
    Renderer renderer;
    // ジョイント情報
    FixedJoint joint;
    // 破片となった時の大きさ
    const float FragmentScale = 80f;
    // 削除命令が出てから消えるまでの時間
    const float DestroyTime = 5.0f;
    // 置き換え先のRigid付きPrefab
    [SerializeField]
    GameObject NewCube;
    // ---------------------------------------------------
    // このオブジェクトが有効になった時
    private void OnEnable()
    {
        // 各コンポーネント取得
        DirectorObject = GameObject.Find("Director").GetComponent<ObjectGenerator>();
        bCol  = this.GetComponent<BoxCollider>();
        mesh = this.GetComponent<MeshRenderer>();
        renderer  = this.GetComponent<Renderer>();
        joint = this.GetComponent<FixedJoint>();
    }
    // ---------------------------------------------------
    // 最初のフレーム
    private void Start()
    {
       // ジョイントにenableプロパティは存在しない
       // 削除　後　再生成が唯一の手
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
        
        // 親がいる場合は崩れる処理
        if (this.transform.parent != null)
        {
            //// 親の繋がりを絶つ
            //this.transform.parent = null;
            //// 詰まらないように破片の大きさを80%にする
            //this.transform.localScale = new Vector3(FragmentScale, FragmentScale, FragmentScale);
            //// 可視化する
            //mesh.enabled = true;
            //// 削除申請を出しておく（消されるのは数秒後
            //Destroy(this.gameObject, DestroyTime);
            
            // 代わりとなるオブジェクトを生成
            GameObject go = Instantiate(NewCube,this.transform.position, Quaternion.identity);
            // レイキャストを無視するレイヤー
            go.transform.localScale = new Vector3(FragmentScale, FragmentScale, FragmentScale);
            // 爆発処理をする
            go.GetComponent<Rigidbody>().AddExplosionForce(power, center, radius);
            go.GetComponent<Renderer>().material = renderer.material;
            this.transform.parent = null;
            // レイを無視するレイヤーに変更
            this.gameObject.layer = 9;
            Destroy(go, DestroyTime);
            Destroy(this.gameObject);
        }
        
        // 軽量化処理
        // 接しているオブジェクトを取得
           
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