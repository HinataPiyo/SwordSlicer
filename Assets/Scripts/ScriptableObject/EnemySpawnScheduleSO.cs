using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnScheduleSO", menuName = "Config/EnemySpawnScheduleSO")]
public class EnemySpawnScheduleSO : ScriptableObject
{
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