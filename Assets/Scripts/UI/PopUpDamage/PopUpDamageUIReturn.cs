using UnityEngine;

public class PopUpDamageUIReturn : MonoBehaviour
{
    [SerializeField] PopUpDamageText damageText;

    // Animation側で設定
    public void ReturnToPool()
    {
        WorldCanvasManager.I.ReturnDamageTextToPool(damageText);
    }
}