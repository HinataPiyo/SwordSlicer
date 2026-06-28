using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public static bool IsGameOver { get; private set; } = false;
    [SerializeField] Transform swordAreaCenter;
    [SerializeField] Vector2 swordArea;

    [SerializeField] CurrencyUI currencyUI;

    public static System.Action OnGameOver;

    public void GetSwordArea(out Vector2 center, out Vector2 size)
    {
        center = swordAreaCenter.position;
        size = swordArea;
    }

    void Awake()
    {
        if(I == null) I = this;
        Application.targetFrameRate = 120;
        OnGameOver += () => GameOver();   
        IsGameOver = false;
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        IsGameOver = true;
    }

    void OnDrawGizmos()
    {
        if(swordAreaCenter == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(swordAreaCenter.position, swordArea);
    }
}
