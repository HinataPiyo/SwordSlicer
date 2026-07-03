using UnityEngine;

[CreateAssetMenu(fileName = "ZombieDataSO", menuName = "EnemyData/ZombieDataSO")]
public class ZombieDataSO : EnemyDataSO
{
    [Header("攻撃を通さない時間"), SerializeField] float invulnerableTime = 1f;
    [Header("Hpの減少割合"), SerializeField] float healthDecreaseRate = 0.3f;       // 体力が30%減少したかどうか

    public float InvulnerableTime => invulnerableTime;
    public float HealthDecreaseRate => healthDecreaseRate;
}