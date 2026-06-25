using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager I { get; private set; }
    [SerializeField] Transform swordAreaCenter;
    [SerializeField] Vector2 swordArea;

    [SerializeField] CurrencyUI currencyUI;

    public void GetSwordArea(out Vector2 center, out Vector2 size)
    {
        center = swordAreaCenter.position;
        size = swordArea;
    }

    void Awake()
    {
        if(I == null) I = this;
        Application.targetFrameRate = 120;
    }

    void OnDrawGizmos()
    {
        if(swordAreaCenter == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(swordAreaCenter.position, swordArea);
    }
}
