using UnityEngine;

public class KinokoMovement : EnemyMovement<KinokoDataSO>
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

    /// <summary>
    /// 敵の移動処理を更新する関数
    /// </summary>
    protected override void AdvanceProgress()
    {
        if (IsFilledSporeKinoko()) return;
        base.AdvanceProgress();
    }

    /// <summary>
    /// 敵の位置を計算する関数
    /// </summary>
    protected override Vector2 EvaluatePosition()
    {
        if (IsFilledSporeKinoko()) return transform.position;
        return base.EvaluatePosition();
    }

    /// <summary>
    /// 敵のスケールを計算する関数
    /// </summary>
    protected override float EvaluateScaleProgress()
    {
        return EvaluateScaleProgressBySpawnSource(ShouldUseSporeScalePath());
    }

    /// <summary>
    /// 胞子キノコで、スケール計算専用の経路を使う場合は、進行度を計算しない。
    /// </summary>
    bool IsFilledSporeKinoko()
    {
        return kinokoController.IsSpawnedBySpore && kinokoController.IsFilled;
    }

    /// <summary>
    /// 胞子キノコで、スケール計算専用の開始位置が設定されている場合は、スケール計算専用の経路を使う。
    /// </summary>
    bool ShouldUseSporeScalePath()
    {
        return kinokoController.IsSpawnedBySpore && hasScaleStartPosition;
    }

    /// <summary>
    /// スケール計算専用の経路を使う場合は、スケール計算専用の開始位置からの進行率を返す。
    /// </summary>
    /// <param name="useSporeScalePath">スケール計算専用の経路を使うかどうか</param>
    float EvaluateScaleProgressBySpawnSource(bool useSporeScalePath)
    {
        if (!useSporeScalePath)
        {
            return CalculatePathProgress(startPosition, targetPosition, transform.position);
        }

        // 胞子生成時は、同じxのスポーンライン開始点からの進行率で見た目サイズを決める。
        return CalculatePathProgress(scaleStartPosition, targetPosition, transform.position);
    }


}