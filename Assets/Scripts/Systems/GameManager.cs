using UnityEngine;

public class GameManager : MonoBehaviour, IGameOver, IGameStop
{
    public const string HAS_PLAYED_ONCE_KEY = "HasPlayedOnce";

    public static bool IsGameOver { get; private set; } = false;
    public static bool IsGameStop { get; private set; } = false;
    public static System.Action OnGameOver;

    void Awake()
    {
        Application.targetFrameRate = 120;
        OnGameOver += () => GameOver();   
        IsGameOver = false;
        IsGameStop = false;
    }

    public void GameOver()
    {
        IsGameOver = true;
        PlayerPrefs.SetInt(HAS_PLAYED_ONCE_KEY, 1);
        PlayerPrefs.Save();
    }

    public void GameStop()
    {
        IsGameStop = true;
    }

    public void ResetStop()
    {
        IsGameStop = false;
    }
}
