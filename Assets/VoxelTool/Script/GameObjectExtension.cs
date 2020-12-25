using UnityEngine;
using System.Collections;

/// <summary>
/// GameObjectの拡張クラス
/// </summary>
public static class GameObjectExtension
{

    /// <summary>
    /// レイヤーを設定する
    /// </summary>
    /// <param name="needSetChildrens">子にもレイヤー設定を行うか</param>
    public static void SetLayer(this GameObject gameObject, int layerNo, bool needSetChildrens = true)
    {
        if (gameObject == null)
        {
            return;
        }
        gameObject.layer = layerNo;

        //子に設定する必要がない場合はここで終了
        if (!needSetChildrens)
        {
            return;
        }

        //子のレイヤーにも設定する
        foreach (Transform childTransform in gameObject.transform)
        {
            SetLayer(childTransform.gameObject, layerNo, needSetChildrens);
        }
    }

    /// <summary>
    /// マテリアル設定
    /// </summary>
    /// <param name="needSetChildrens">子にもマテリアル設定を行うか</param>
    public static void SetMaterial(this GameObject gameObject, Material setMaterial, bool needSetChildrens = true)
    {
        if (gameObject == null)
        {
            return;
        }

        //レンダラーがあればそのマテリアルを変更
        if (gameObject.GetComponent<Renderer>())
        {
            gameObject.GetComponent<Renderer>().material = setMaterial;
        }

        //子に設定する必要がない場合はここで終了
        if (!needSetChildrens)
        {
            return;
        }

        //子のマテリアルにも設定する
        foreach (Transform childTransform in gameObject.transform)
        {
            SetMaterial(childTransform.gameObject, setMaterial, needSetChildrens);
        }

    }


}