using UnityEngine;

public class GameManager : MonoBehaviour, IGameOver, IGameStop
{
    public static bool IsGameOver { get; private set; } = false;
    public static bool IsGameStop { get; private set; } = false;
    public static System.Action OnGameOver;

    void Awake()
    {
        Application.targetFrameRate = 120;
        OnGameOver += () => GameOver();   
        IsGameOver = false;
    }

    public void GameOver()
    {
        IsGameOver = true;
    }

    public void GameStop()
    {
        IsGameStop = true;
    }
}
