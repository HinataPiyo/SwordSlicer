using UnityEngine;

public class KinokoMovement : EnemyMovement
{
    KinokoController kinokoController;
    bool hasScaleStartPosition;
    Vector2 scaleStartPosition;

    void Awake()
    {
        kinokoController = GetComponent<KinokoController>();
    }

    public void SetScaleStartPosition(Vector2 startPos)
    {
        // 胞子キノコのみ、移動経路とは別の「スケール計算専用の開始位置」を使う。
        scaleStartPosition = startPos;
        hasScaleStartPosition = true;
    }

    protected override void UpdateMovement()
    {
        bool useSporeScalePath = ShouldUseSporeScalePath();     // 胞子キノコで、スケール計算専用の開始位置が設定されている場合は、スケール計算専用の経路を使う。
        if(kinokoController.IsSpawnedBySpore && kinokoController.IsFilled)    // このキノコが胞子によって生成され、埋まった状態の場合は移動しない
        {
            // 埋まった状態のときは移動しないが、スケールはスケール計算専用の経路で更新する。
            UpdateScaleBySpawnSource(useSporeScalePath);
            return;
        }

        MoveByProgressToTarget();
        UpdateScaleBySpawnSource(useSporeScalePath);
    }

    // 胞子キノコで、スケール計算専用の開始位置が設定されている場合は、スケール計算専用の経路を使う。
    bool ShouldUseSporeScalePath()
    {
        return kinokoController.IsSpawnedBySpore && hasScaleStartPosition;
    }

    // スケール計算専用の経路を使う場合は、スケール計算専用の開始位置からの進行率でスケールを更新する。
    void UpdateScaleBySpawnSource(bool useSporeScalePath)
    {
        if (!useSporeScalePath)
        {
            UpdateScale();
            return;
        }

        // 胞子生成時は、同じxのスポーンライン開始点からの進行率で見た目サイズを決める。
        float scaleProgress = CalculatePathProgress(scaleStartPosition, targetPosition, transform.position);
        ApplyScaleFromProgress(scaleProgress);
    }


}