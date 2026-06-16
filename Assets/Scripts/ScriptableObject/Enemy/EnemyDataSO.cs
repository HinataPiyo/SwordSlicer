using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [SerializeField] string enemyName;
    [SerializeField] int maxHealth;
    [Header("ボーダーラインに到達するまでの時間"), SerializeField] float reachDuration;

    public string EnemyName => enemyName;
    public int MaxHealth => maxHealth;
    public float ReachDuration => reachDuration;
}