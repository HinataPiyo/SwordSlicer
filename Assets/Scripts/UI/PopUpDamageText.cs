using TMPro;
using UnityEngine;

public class PopUpDamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText;

    public void SetDamageUI(float damage)
    {
        damageText.text = damage.ToString("F0");    // 小数点以下を表示しない
    }
}