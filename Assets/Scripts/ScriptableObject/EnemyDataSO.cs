using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [SerializeField] EnemyController enemyPrefab;
    [SerializeField] string enemyName;
    [SerializeField] int maxHealth;
    [Header("ボーダーラインに到達するまでの時間"), SerializeField] float reachDuration;

    public EnemyController EnemyPrefab => enemyPrefab;
    public string EnemyName => enemyName;
    public int MaxHealth => maxHealth;
    public float ReachDuration => reachDuration;
}