using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class paintButtonScript : MonoBehaviour
{
    enum MODE
    {
        FORM,
        PAINT,
        STAMP,
        BEDROCKSIZE
    }

    private MODE mode = MODE.FORM;
    private MODE prevMode;

    public Text text;

    public paintScript paint;
    public cubeJointScript form;
    public stampScript stamp;
    public BedrockScript bedrock;

    public Dropdown materialDropdown;
    public Dropdown shapeDropdown;

    public Dropdown stampDropdown;

    public Text operation;

    // Start is called before the first frame update
    void Start()
    {
        ChangeShapeDropdown(materialDropdown);
    }

    // Update is called once per frame
    void Update()
    {
        switch (mode)
        {
            case MODE.FORM:
                if(!bedrock.panel.activeSelf)
                    form.Form();
                operation.text = "左クリック：置く\nShift+左クリック：消す(原点のは無理)\nドロップダウンで置くブロックとマテリアル変更";
                break;
            case MODE.PAINT:
                if (!bedrock.panel.activeSelf)
                    paint.Paint();
                operation.text = "左クリック：色変え\nドロップダウンで選択したブロックの色を変更可能\nドロップダウンでマテリアル変更";
                break;
            case MODE.STAMP:
                if (!bedrock.panel.activeSelf)
                    stamp.Stamp();
                operation.text = "左クリック：置く　右クリック：回転\nブロックが回転しないのは仕様\nドロップダウンで置くスタンプ変更\nBoxesData/Stamps内を参照する";
                break;
            case MODE.BEDROCKSIZE:
                if (!bedrock.panel.activeSelf)
                    bedrock.ChangeSize();
                operation.text = "左下らへんのInputFieldで大きさ変更\nTabキーでInputField切替\n再度サイズ変更ボタンを押すと終了";
                break;
        }
    }

    public void ClickPaintButton()
    {
        switch(mode)
        {
            case MODE.FORM:
                text.text = "いろぬり";
                mode = MODE.PAINT;
                break;
            case MODE.PAINT:
                text.text = "すたんぷ";
                materialDropdown.gameObject.SetActive(false);
                shapeDropdown.gameObject.SetActive(false);
                stampDropdown.gameObject.SetActive(true);
                mode = MODE.STAMP;
                break;
            case MODE.STAMP:
                text.text = "はいち";
                materialDropdown.gameObject.SetActive(true);
                shapeDropdown.gameObject.SetActive(true);
                stampDropdown.gameObject.SetActive(false);
                mode = MODE.FORM;
                break;
            default:
                // さいずかえモードの時ここ　なにもおこりません
                break;
        }
    }

    public void ChangeShapeDropdown(Dropdown change)
    {
        // 引数で押されたドロップダウンを持って来れる　あとはvalueで数字取得したり
        materialDropdown.ClearOptions();
        materialDropdown.AddOptions(form.holder.materialTextList[change.value]);
        materialDropdown.value = 0;
    }

    public void SetPrevMode()
    {
        prevMode = mode;
        mode = MODE.BEDROCKSIZE;
        text.text = "さいずかえ";
        materialDropdown.gameObject.SetActive(false);
        shapeDropdown.gameObject.SetActive(false);
        stampDropdown.gameObject.SetActive(false);
    }

    public void EndBedrockSizeMode()
    {
        mode = --prevMode;
        if ((int)mode == -1)
            mode = MODE.STAMP;
        materialDropdown.gameObject.SetActive(true);
        shapeDropdown.gameObject.SetActive(true);
        stampDropdown.gameObject.SetActive(true);
        ClickPaintButton();
    }
}
