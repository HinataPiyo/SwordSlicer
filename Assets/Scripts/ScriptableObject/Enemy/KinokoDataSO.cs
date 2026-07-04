using UnityEngine;

[CreateAssetMenu(fileName = "KinokoDataSO", menuName = "EnemyData/KinokoDataSO")]
public class KinokoDataSO : EnemyDataSO
{
    [Header("キノコが埋まってる時間"), SerializeField] float fillDuration = 5f;
    [Header("胞子を放つまでの時間"), SerializeField] float breedDelay = 2f;
    [Header("繁殖アニメーションの時間"), SerializeField] float breedAnimationDuration = 1f;
    [Header("胞子のパーティクル"), SerializeField] GameObject sporeParticle;
    [Header("繁殖する数"), SerializeField] int spawnCount = 3;
    [Header("繁殖するキノコ"), SerializeField] KinokoController spawnKinokoPrefab;

    public float FillDuration => fillDuration;
    public float BreedDelay => breedDelay;
    public float BreedAnimationDuration => breedAnimationDuration;
    public GameObject SporeParticle => sporeParticle;
    public int SpawnCount => spawnCount;
    public KinokoController SpawnKinokoPrefab => spawnKinokoPrefab;
}