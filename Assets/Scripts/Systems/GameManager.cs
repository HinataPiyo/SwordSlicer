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

    /// <summary>
    /// 通貨を追加する処理
    /// </summary>
    public void AddCurrency(int amount)
    {
        CurrencyManager.AddCurrency(amount);
        currencyUI.UpdateCurrency(CurrencyManager.Currency);    // 通貨の値を更新する
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(swordAreaCenter.position, swordArea);
    }
}
