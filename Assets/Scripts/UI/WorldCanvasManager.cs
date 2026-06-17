using UnityEngine;

public class WorldCanvasManager : MonoBehaviour
{
    public static WorldCanvasManager I { get; private set; }

    [SerializeField] PopUpDamageText popUpDamageTextPrefab;
    [SerializeField] GameObject attackMissTextPrefab;

    void Awake()
    {
        if(I == null) I = this;
    }

    public void ShowDamageText(float damage, Vector2 position)
    {
        PopUpDamageText popUp = Instantiate(popUpDamageTextPrefab, position, Quaternion.identity, transform);
        popUp.SetDamageUI(damage);
    }

    public void ShowAttackMissText(Vector2 position)
    {
        Instantiate(attackMissTextPrefab, position, Quaternion.identity, transform);
    }
}