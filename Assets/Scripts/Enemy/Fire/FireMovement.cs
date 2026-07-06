using UnityEngine;

public class FireMovement : EnemyMovement<FireDataSO>
{
    FireController fireController;

    void Awake()
    {
        fireController = GetComponent<FireController>();
    }

    /// <summary>
    /// 敵の位置を計算する関数
    /// </summary>
    protected override Vector2 EvaluatePosition()
    {
        if (!TryGetTypedData(out var data))
        {
            return base.EvaluatePosition();
        }

        float baseX = Mathf.Lerp(startPosition.x, targetPosition.x, progress);
        float baseY = Mathf.Lerp(startPosition.y, targetPosition.y, progress);
        float wobble = Mathf.Sin(progress * Mathf.PI * 2f * data.WaveCycles) * data.WaveAmplitude;
        float wobbleDirection = fireController != null && fireController.isFlip ? -1f : 1f;

        float newX = baseX;
        float newY = baseY + wobble * wobbleDirection;

        return new Vector2(newX, newY);
    }
}