using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public static bool IsGameOver { get; private set; } = false;
    public static System.Action OnGameOver;

    void Awake()
    {
        if(I == null) I = this;
        Application.targetFrameRate = 120;
        OnGameOver += () => GameOver();   
        IsGameOver = false;
    }

    void GameOver()
    {
        IsGameOver = true;
    }
}
