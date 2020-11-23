using UnityEngine;
using System.Collections;

public class CursorRayCast : MonoBehaviour
{

    // カーソルに使用するテクスチャ
    [SerializeField]
    private Texture2D cursor = null;

    void Start()
    {
        // カーソルを画像に変更
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
    }

    void Update()
    {
        // マウスの左クリックでアクション
        if (Input.GetButtonDown("Fire1"))
        {
            Shot();
        }
    }

    // 爆弾投げる
    void Shot()
    {
        float distance = 100; // 飛ばす&表示するRayの長さ
        float duration = 3;   // 表示期間（秒）

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, duration, false);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Default")))
        {
            Debug.Log(hit.point);
        }
    }

    
}