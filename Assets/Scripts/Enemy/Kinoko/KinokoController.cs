using UnityEngine;

public class KinokoController : EnemyController<KinokoDataSO>
{
    public bool IsSpawnedBySpore { get; private set; } = false;    // このキノコが胞子によって生成されたかどうかを示すフラグ
    public bool IsFilled { get; private set; } = false;    // このキノコが埋まった状態かどうかを示すフラグ
    public bool IsBreed { get; private set; } = false;    // このキノコが繁殖したかどうかを示すフラグ
    float spawnElapsedTime = 0f;    // このキノコが生成されてからの経過時間
    bool hasTriggeredBreedAnimation = false;

    public override void Initialize(Vector2 borderLinePos, Vector2 startPos, float enemyStatusMultiplier)
    {
        Initialize(borderLinePos, startPos, enemyStatusMultiplier, false);    // デフォルトでは胞子による生成ではない
    }

    public void Initialize(Vector2 borderLinePos, Vector2 startPos, float enemyStatusMultiplier, bool isSpawnedBySpore)
    {
        IsSpawnedBySpore = isSpawnedBySpore;    // このキノコが胞子によって生成されたかどうかを設定
        spawnElapsedTime = 0f;
        hasTriggeredBreedAnimation = false;

        // 胞子生成個体は最初から「埋まっている」「繁殖済み」の状態で開始する。
        // 非胞子個体だけ繁殖タイマーを進める。
        IsFilled = IsSpawnedBySpore;
        IsBreed = IsSpawnedBySpore;

        if (!TryGetData(out var data))
        {
            enabled = false;
            return;
        }

        base.Initialize(borderLinePos, startPos, enemyStatusMultiplier);

        if(IsSpawnedBySpore)
        {
            ChangeAnimationState("Fill_IN");
        }

    }

    public void SetScaleStartPosition(Vector2 startPos)
    {
        // Movement へスケール専用の経路開始点を渡す。
        // 位置移動の開始点(startPos)とは分離して扱う。
        if (movement is KinokoMovement kinokoMovement)
        {
            kinokoMovement.SetScaleStartPosition(startPos);
        }
    }

    protected override void Update()
    {
        base.Update();

        if(health.IsDead) return;

        if (!TryGetData(out var data)) return;

        if (IsFilled && IsSpawnedBySpore)       // このキノコが胞子によって生成され、埋まった状態の場合
        {
            spawnElapsedTime += Time.deltaTime;
            if (spawnElapsedTime >= data.FillDuration)
            {
                // 一定時間経過で埋まり解除して移動可能状態へ遷移。
                IsFilled = false;    // 埋まり解除して移動できる状態にする
                ChangeAnimationState("Fill_OUT");
            }
        }
        else if(!IsSpawnedBySpore && !IsBreed)      // このキノコが胞子によって生成されておらず、まだ繁殖していない場合
        {
            spawnElapsedTime += Time.deltaTime;
            if(spawnElapsedTime >= data.BreedDelay)
            {
                if (!hasTriggeredBreedAnimation)
                {
                    ChangeAnimationState("Breed");
                    hasTriggeredBreedAnimation = true;
                }

                if(spawnElapsedTime >= data.BreedDelay + data.BreedAnimationDuration)
                {
                    Instantiate(data.SporeParticle, transform.position, Quaternion.identity);    // 胞子のパーティクルを生成
                    // 繁殖処理を行う
                    for (int i = 0; i < data.SpawnCount; i++)
                    {
                        ServiceLocator.Get<ISpawnKinoko>().SpawnEnemyBySpore(this);
                    }
                    IsBreed = true;    // 繁殖済みにする
                }
            }
        }
    }
}