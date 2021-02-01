using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class paintButtonScript : MonoBehaviour
{
    enum MODE
    {
        FORM,
        TONS,
        PAINT,
        STAMP,
        BEDROCKSIZE
    }

    private MODE mode = MODE.FORM;
    private MODE prevMode;

    [SerializeField]
    private Text text = null;      // 自分のテキスト

    [SerializeField]
    private paintScript paint = null;
    [SerializeField]
    private cubeJointScript form = null;
    [SerializeField]
    private TonsScript tons = null;
    [SerializeField]
    private stampScript stamp = null;
    [SerializeField]
    private BedrockScript bedrock = null;

    [SerializeField]
    private Dropdown materialDropdown = null;
    [SerializeField]
    private Dropdown shapeDropdown = null;

    [SerializeField]
    private Dropdown stampDropdown = null;

    [SerializeField]
    private Text operation = null;

    // Start is called before the first frame update
    void Start()
    {
        ChangeShapeDropdown();
        operation.text = "左クリック：置く\nShift+左クリック：消す(原点のは無理)\nドロップダウンで置くブロックとマテリアル変更";
    }

    // Update is called once per frame
    void Update()
    {
        bool flag = (!bedrock.panel.activeSelf && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject());
        // パネル出てない、UIに触れてない　でupdate
        switch (mode)
        {
            case MODE.FORM:
                if(flag)
                    form.Form();
                break;
            case MODE.TONS:
                if (flag)
                    tons.Tons();
                break;
            case MODE.PAINT:
                if(flag)
                    paint.Paint();
                break;
            case MODE.STAMP:
                if(flag)
                    stamp.Stamp();
                break;
            case MODE.BEDROCKSIZE:
                bedrock.ChangeSize();
                break;
        }
    }

    /// <summary>
    /// 左上のボタンを押したとき　モード変える
    /// サイズ変更モードの時はボタン押してもなんも起こらない
    /// </summary>
    public void ClickPaintButton()
    {
        switch(mode)
        {
            case MODE.FORM:
                text.text = "いっぱいおく";
                shapeDropdown.gameObject.SetActive(false);
                ChangeShapeDropdown(true);
                mode = MODE.TONS;
                operation.text = "左クリックで始点を決定\nもう一回左クリックでいっぱい置ける\n右クリックで始点とり直し";
                break;
            case MODE.TONS:
                text.text = "いろぬり";
                shapeDropdown.gameObject.SetActive(true);
                ChangeShapeDropdown();
                tons.Reset();
                mode = MODE.PAINT;
                operation.text = "左クリック：色変え\nドロップダウンで選択したブロックの色を変更可能\nドロップダウンでマテリアル変更";
                break;
            case MODE.PAINT:
                text.text = "すたんぷ";
                materialDropdown.gameObject.SetActive(false);
                shapeDropdown.gameObject.SetActive(false);
                stampDropdown.gameObject.SetActive(true);
                mode = MODE.STAMP;
                operation.text = "左クリック：置く　右クリック：回転\nブロックが回転しないのは仕様\nドロップダウンで置くスタンプ変更\nBoxesData/Stamps内を参照する";
                break;
            case MODE.STAMP:
                text.text = "はいち";
                materialDropdown.gameObject.SetActive(true);
                shapeDropdown.gameObject.SetActive(true);
                stampDropdown.gameObject.SetActive(false);
                mode = MODE.FORM;
                operation.text = "左クリック：置く\nShift+左クリック：消す(原点のは無理)\nドロップダウンで置くブロックとマテリアル変更";
                break;
            default:
                // さいずかえモードの時ここ　なにもおこりません
                break;
        }
    }

    /// <summary>
    /// ドロップダウン用　置く形の方のドロップダウンを変えた時にマテリアルのドロップダウンも変更する
    /// </summary>
    /// <param name="flag">trueだと0(正方形)のマテリアルドロップダウンに変更する</param>
    public void ChangeShapeDropdown(bool flag = false)
    {
        // 引数で押されたドロップダウンを持って来れる　あとはvalueで数字取得したり
        materialDropdown.ClearOptions();
        if(flag)
        {
            materialDropdown.AddOptions(form.holder.materialTextList[0]);
        }
        else
        {
            materialDropdown.AddOptions(form.holder.materialTextList[shapeDropdown.value]);
        }
        materialDropdown.value = 0;
    }

    /// <summary>
    /// サイズ変更モードに移行
    /// </summary>
    public void SetPrevMode()
    {
        // サイズ変更モードに移行　いろいろ非表示にする
        prevMode = mode;
        mode = MODE.BEDROCKSIZE;
        text.text = "さいずかえ";
        operation.text = "左下らへんのInputFieldで大きさ変更\nTabキーでInputField切替\n再度サイズ変更ボタンを押すと終了";

        materialDropdown.gameObject.SetActive(false);
        shapeDropdown.gameObject.SetActive(false);
        stampDropdown.gameObject.SetActive(false);
    }

    /// <summary>
    /// サイズ変更モードから戻る
    /// </summary>
    public void EndBedrockSizeMode()
    {
        // サイズ変更モード→元のモードに戻る　元のモードのUI表示状況は別関数で設定
        mode = --prevMode;
        if ((int)mode == -1)
            mode = MODE.STAMP;
        materialDropdown.gameObject.SetActive(true);
        shapeDropdown.gameObject.SetActive(true);
        stampDropdown.gameObject.SetActive(true);
        ClickPaintButton();
    }
}
