using TMPro;
using UnityEngine;

public class CriticalHitText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI criticalHitText;

    public void SetCriticalDamageUI(float damage)
    {
        criticalHitText.text = damage.ToString("F0");    // 小数点以下を表示しない
    }
}