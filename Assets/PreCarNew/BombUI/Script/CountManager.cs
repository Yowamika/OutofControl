using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountManager : MonoBehaviour
{
    // 爆弾の情報を持ってくるため
    GrenadeThrow bomb;

    public GameObject car;

    // 爆弾UIのパネル
    public BombsCount bombPanel;

    // Start is called before the first frame update
    void Start()
    {
        // 情報を持ってくる
        bomb = car.GetComponent<GrenadeThrow>();
    }

    // Update is called once per frame
    void Update()
    {
        // 爆弾のカウントを格納
        int count = bomb.Count();

        // パネルのライフに反映
        bombPanel.UpdateLife(count);
    }
}
