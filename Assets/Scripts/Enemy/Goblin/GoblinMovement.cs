using UnityEngine;

public class GoblinMovement : GoblinMovementBase<GoblinDataSO>
{
}

public class GoblinMovementBase<TData> : EnemyMovement<TData> where TData : GoblinDataSO
{
    float elapsedIdleTime;
    bool flipX = false;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void UpdateMovement()
    {
        if (!TryGetTypedData(out var data)) return;

        elapsedIdleTime += Time.deltaTime;
        if (elapsedIdleTime < data.MoveCooldown) return;
        Move(data);
        Reset();
    }

    void Move(TData data)
    {
        changeAnimation.Invoke(data.MoveAnimationName);
        flipX = !flipX;    // 次回の移動時に反転
        spriteRenderer.flipX = flipX;    // スプライトの反転を切り替え
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        transform.position += (Vector3)moveDirection * data.MoveDistance;       // 一瞬で移動する

        UpdateScale();
    }

    void Reset()
    {
        elapsedIdleTime = 0f;    // クールダウンをリセット
    }
}