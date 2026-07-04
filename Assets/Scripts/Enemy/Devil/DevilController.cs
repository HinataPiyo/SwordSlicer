using UnityEngine;

public class DevilController : EnemyController
{
    [SerializeField] Vector2 offsetPos = new Vector2(0, 0.5f);
    DevilDataSO convertData;
    float fireSpawnTimer = 0f;    // 火の玉を生成するタイマー

    public DevilDataSO ConvertDataSO()
    {
        DevilDataSO data = Data as DevilDataSO;
        if (data == null)
        {
            Debug.LogError("DevilDataSO is null");
            return null;
        }

        convertData = data;
        return convertData;
    }

    public override void Initialize(Vector2 borderLinePos, Vector2 startPos, float enemyStatusMultiplier)
    {
        base.Initialize(borderLinePos, startPos, enemyStatusMultiplier);
        transform.position = startPos + offsetPos;

        ChangeAnimationState("Spawn");

        if (convertData == null) ConvertDataSO();
    }

    protected override void Update()
    {
        base.Update();

        fireSpawnTimer += Time.deltaTime;
        if(fireSpawnTimer >= convertData.FireSpawnInterval)
        {
            for(int i = 0; i < convertData.FireSpawnCount; i++)
            {
                bool isFlip = Random.value > 0.5f; // ランダムで火の玉の生成方向を決定
                ServiceLocator.Get<ISpawnFire>().SpawnEnemyFire(convertData.FirePrefab, isFlip);
            }

            fireSpawnTimer = 0f;
        }
    }
}