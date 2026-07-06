using UnityEngine;

public class DevilController : EnemyController<DevilDataSO>
{
    [SerializeField] Vector2 offsetPos = new Vector2(0, 0.5f);
    float fireSpawnTimer = 0f;    // 火の玉を生成するタイマー
    const string ANIMATION_PARAMETER_SPAWN = "Spawn";

    public override void Initialize(Vector2 borderLinePos, Vector2 startPos, float enemyStatusMultiplier)
    {
        base.Initialize(borderLinePos, startPos, enemyStatusMultiplier);
        transform.position = startPos + offsetPos;

        ChangeAnimationState(ANIMATION_PARAMETER_SPAWN);

        if (!TryGetData(out var data))
        {
            enabled = false;
            return;
        }

        fireSpawnTimer = data.FireSpawnInterval;    // 初回の火の玉生成を遅らせるため、タイマーを初期化
    }

    protected override void Update()
    {
        base.Update();

        if(health.IsDead) return;    // 死亡している場合は火の玉を生成しない

        if (!TryGetData(out var data)) return;

        fireSpawnTimer += Time.deltaTime;
        if(fireSpawnTimer >= data.FireSpawnInterval)
        {
            for(int i = 0; i < data.FireSpawnCount; i++)
            {
                bool isFlip = Random.value > 0.5f; // ランダムで火の玉の生成方向を決定
                ServiceLocator.Get<ISpawnFire>().SpawnEnemyFire(data.FirePrefab, isFlip);
            }

            fireSpawnTimer = 0f;
        }
    }
}