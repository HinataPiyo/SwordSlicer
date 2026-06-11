using UnityEngine;

public class WorldCanvasManager : MonoBehaviour
{
    public static WorldCanvasManager I { get; private set; }

    [SerializeField] PopUpDamageText popUpDamageTextPrefab;

    void Awake()
    {
        if(I == null) I = this;
    }

    public void ShowDamageText(float damage, Vector2 position)
    {
        PopUpDamageText popUp = Instantiate(popUpDamageTextPrefab, position, Quaternion.identity, transform);
        popUp.SetDamageUI(damage);
    }
}