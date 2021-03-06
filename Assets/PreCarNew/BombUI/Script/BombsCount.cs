﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombsCount : MonoBehaviour
{

    public float speed = 1.5f;


    public Image[] images;

    private Image image;
    private float time;

    // 爆弾の情報を持ってくるため
    GrenadeThrow bomb;
    public GameObject car;

    private const float EXPANSION = 1.2f;
   
    // Start is called before the first frame update
    void Start()
    {
        // 情報を持ってくる
        bomb = car.GetComponent<GrenadeThrow>();
    }

    public void UpdateLife(int life)
    {
        if(0 < life)
        {
            for (int i = 0; i < 3; i++)
            {
                // 未使用
                images[i].color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

                // 使用後は灰色ボムに
                if (i + 1 > life)
                    images[i].color = new Color(0.0f, 0.0f, 0.0f, 0.25f);
                images[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                // 点滅フラグが立ったら点滅ボムに
                if (bomb.GetIsBlink())
                {
                    time += Time.deltaTime * 5.0f * speed;
                    images[life - 1].color = new Color(0.0f, 0.0f, 0.0f, Mathf.Sin(time) * 0.5f + 0.5f);

                    images[life - 1].transform.localScale = new Vector3(EXPANSION, EXPANSION, 1.0f);
                }
                else
                    images[life - 1].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

    }

}
