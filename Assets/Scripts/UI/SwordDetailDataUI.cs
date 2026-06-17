using UnityEngine;
using UnityEngine.UIElements;

public class SwordDetailDataUI : MonoBehaviour
{
    UIDocument uiDoc;
    Label rotateForceValue;
    Label moveSpeedValue;
    Label turnAmountValue;

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();

        var r = uiDoc.rootVisualElement;
        rotateForceValue = r.Q<Label>("rotate-amount-value");
        moveSpeedValue = r.Q<Label>("move-speed-value");
        turnAmountValue = r.Q<Label>("turn-amount-value");

        Reset();
    }

    void Reset()
    {
        rotateForceValue.text = "0.0";
        moveSpeedValue.text = "0.0";
        turnAmountValue.text = "0.0";
    }

    public void UpdateData(SwordControl sword)
    {
        if(sword == null)
        {
            return;
        }

        UpdateRotateAmount(sword.RotateAmount);
        UpdateMoveSpeed(sword.Speed);
        UpdateTurnAmount(sword.GetTurnEffect());
    }

    // 回転力
    void UpdateRotateAmount(float value)
    {
        rotateForceValue.text = value.ToString("F1");
    }

    // 移動速度
    void UpdateMoveSpeed(float value)
    {
        moveSpeedValue.text = value.ToString("F1");
    }

    // 回転による横移動量
    void UpdateTurnAmount(float value)
    {
        turnAmountValue.text = (value * 10000).ToString("F1");
    }



}