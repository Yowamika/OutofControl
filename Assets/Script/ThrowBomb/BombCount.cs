using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombCount : MonoBehaviour
{
    GrenadeThrow bomb;
    public Text counter;
    public GameObject car;
    // Start is called before the first frame update
    void Start()
    {
        bomb = car.GetComponent<GrenadeThrow>();
    }

    // Update is called once per frame
    void Update()
    {
       //Text BombText = counter.GetComponent<Text>();
       counter.text = "爆弾残数：" + bomb.Count().ToString();
    }
}
