using UnityEngine;

[CreateAssetMenu(fileName = "DevilDataSO", menuName = "EnemyData/DevilDataSO")]
public class DevilDataSO : EnemyDataSO
{
    [SerializeField] FireController firePrefab;    // 火の玉を生成するためのプレハブ
    [SerializeField] float fireSpawnInterval = 10f;    // 火の玉を生成する間隔
    [SerializeField] int fireSpawnCount = 3;    // 火の玉を生成する数

    public FireController FirePrefab => firePrefab;
    public float FireSpawnInterval => fireSpawnInterval;
    public int FireSpawnCount => fireSpawnCount;
}