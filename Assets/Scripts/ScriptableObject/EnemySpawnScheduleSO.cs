using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnScheduleSO", menuName = "Config/EnemySpawnScheduleSO")]
public class EnemySpawnScheduleSO : ScriptableObject
{
    [SerializeField] float spawnInterval = 5f; // 敵の出現間隔
    [SerializeField] float difficultyIncreaseInterval = 15f; // 難易度が上がる間隔
    [SerializeField] float enemyStatusMultiplier = 1.1f;
    [SerializeField] float spawnIntervalMultiplier = 0.9f;

    static readonly float minSpawnInterval = 0.5f; // 最小出現間隔

    public float SpawnInterval(float elapsedTime)
    {
        // 難易度が上がるごとに出現間隔を短くする
        float interval = spawnInterval * Mathf.Pow(spawnIntervalMultiplier, elapsedTime / difficultyIncreaseInterval);
        return Mathf.Max(interval, minSpawnInterval);
    }

    public float EnemyStatusMultiplier(float elapsedTime)
    {
        // 難易度が上がるごとに敵のステータスを強化する
        return Mathf.Pow(enemyStatusMultiplier, elapsedTime / difficultyIncreaseInterval);
    }
    [SerializeField] Entry[] entries;
    public Entry[] Entries => entries;


    [System.Serializable]
    public class Entry
    {
        public GameObject enemyPrefab;
        public float enemyTypeUnlockInterval;       // 敵の種類がアンロックされる間隔
        public int maxSpawnCount;                   // 最大出現数
        [Range(0.01f, 1f)] public float probability;// 出現する確率
    }
}