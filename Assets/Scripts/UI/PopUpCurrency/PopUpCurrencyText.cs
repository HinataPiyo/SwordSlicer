using TMPro;
using UnityEngine;

public class PopUpCurrencyText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currencyText;

    public void SetCurrencyUI(int currency)
    {
        currencyText.text = "￥" + currency.ToString("#,##0");    // 通貨の増加量を表示する
    }
}