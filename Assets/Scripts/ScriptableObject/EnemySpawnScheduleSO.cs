using System.Collections.Generic;
using UnityEngine;

public enum DifficultyLevel { Easy, Normal, Hard, Extreme }
[CreateAssetMenu(fileName = "EnemySpawnScheduleSO", menuName = "Config/EnemySpawnScheduleSO")]
public class EnemySpawnScheduleSO : ScriptableObject
{
    [SerializeField] DifficultyLevel difficultyLevel = DifficultyLevel.Normal;
    [SerializeField] float spawnInterval = 8f; // 敵の出現間隔
    [SerializeField] float difficultyIncreaseInterval = 30f; // 難易度が上がる間隔
    [SerializeField] float enemyStatusMultiplier = 1.1f;
    [SerializeField] float spawnIntervalMultiplier = 0.9f;
    [SerializeField] float enemyTypeUnlockInterval = 20f;       // 敵の種類がアンロックされる間隔
    const float MinDifficultyIncreaseInterval = 5f; // 最小難易度上昇間隔

    static readonly Dictionary<DifficultyLevel, float> EnemyStatusMultipliersByDifficulty = new Dictionary<DifficultyLevel, float>
    {
        { DifficultyLevel.Easy, 0.8f },
        { DifficultyLevel.Normal, 1f },
        { DifficultyLevel.Hard, 1.2f },
        { DifficultyLevel.Extreme, 1.5f }
    };

    static readonly Dictionary<DifficultyLevel, float> DifficultyIncreaseMultiplierByDifficulty = new Dictionary<DifficultyLevel, float>
    {
        { DifficultyLevel.Easy, 1.2f },
        { DifficultyLevel.Normal, 1f },
        { DifficultyLevel.Hard, 0.8f },
        { DifficultyLevel.Extreme, 0.6f }
    };

    static readonly float minSpawnInterval = 0.5f; // 最小出現間隔

    /// <summary>
    /// 難易度に応じて敵を倒した時に得る通貨の量を計算する
    /// </summary>
    public int CurrencyRewardByDifficulty(int reward)
    {
        return Mathf.RoundToInt(reward * EnemyStatusMultipliersByDifficulty[difficultyLevel]);
    }

    public void SetDifficultyLevel(DifficultyLevel level) => difficultyLevel = level;
    public DifficultyLevel GetDifficultyLevel() => difficultyLevel;
    public string GetDifficultyLevelText()
    {
        switch (difficultyLevel)
        {
            case DifficultyLevel.Easy: return "イージー";
            case DifficultyLevel.Normal: return "ノーマル";
            case DifficultyLevel.Hard: return "ハード";
            case DifficultyLevel.Extreme: return "エクストリーム";
            default: return "Unknown";
        }
    }

    /// <summary>
    /// 難易度に応じて敵のステータスを強化する倍率を計算する
    /// </summary>
    float DifficultyIncreaseIntervalByDifficulty()
    {
        return difficultyIncreaseInterval * DifficultyIncreaseMultiplierByDifficulty[difficultyLevel];
    }

    public float SpawnInterval(float elapsedTime)
    {
        // 難易度が上がるごとに出現間隔を短くする
        float interval = spawnInterval * Mathf.Pow(spawnIntervalMultiplier, elapsedTime / DifficultyIncreaseIntervalByDifficulty());
        return Mathf.Max(interval, minSpawnInterval); // 最小出現間隔を超えないようにする
    }

    /// <summary>
    /// 難易度に応じて敵のステータスを強化する倍率を計算する
    /// </summary>
    public float EnemyStatusMultiplier(float elapsedTime)
    {
        // 難易度が上がるごとに敵のステータスを強化する\
        float multiplier = EnemyStatusMultipliersByDifficulty[difficultyLevel] * Mathf.Pow(enemyStatusMultiplier, elapsedTime / DifficultyIncreaseIntervalByDifficulty());
        return multiplier;
    }

    /// <summary>
    /// 難易度に応じて敵の種類がアンロックされる間隔を計算する
    /// </summary>
    public float EnemyTypeUnlockInterval()
    {
        float interval = enemyTypeUnlockInterval * DifficultyIncreaseMultiplierByDifficulty[difficultyLevel];
        Debug.Log($"Difficulty Level: <color=yellow>{difficultyLevel}</color>, EnemyTypeUnlockInterval: <color=yellow>{interval}</color>");
        return Mathf.Max(interval, MinDifficultyIncreaseInterval); // 最小アンロック間隔を超えないようにする
    }
    [SerializeField] Entry[] entries;
    public Entry[] Entries => entries;


    [System.Serializable]
    public class Entry
    {
        public GameObject enemyPrefab;
        [Range(0.01f, 1f)] public float probability;// 出現する確率
    }
}