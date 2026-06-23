using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [SerializeField] string enemyName;
    [SerializeField] int maxHealth;
    [Header("ボーダーラインに到達するまでの時間"), SerializeField] float reachDuration;
    [Header("敵を倒した時に得る通貨"), SerializeField] int currencyReward;

    public string EnemyName => enemyName;
    public int MaxHealth => maxHealth;
    public float ReachDuration => reachDuration;
    public int CurrencyReward => currencyReward;
}