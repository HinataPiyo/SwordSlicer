using UnityEngine;

public class FrogMovement : EnemyMovement<FrogDataSO>
{
    float elapsedIdleTime;
    public bool IsAnimation { get; private set; }    // アニメーション中かどうかのフラグ
    Vector3 targetPos;

    protected override void UpdateMovement()
    {
        if (!TryGetTypedData(out var data)) return;

        elapsedIdleTime += Time.deltaTime;
        if(elapsedIdleTime < data.JumpCooldown) return;
        // ジャンプ中
        StartAnimation(data);
        UpdateScale();
        Jump(data);
    }

    /// <summary>
    /// 敵がジャンプする処理
    /// </summary>
    void Jump(FrogDataSO data)
    {
        float distanceToTarget = Vector2.Distance(transform.position, targetPos);       // ターゲットまでの距離を計算
        // １秒で到達するようにする
        transform.position = Vector2.MoveTowards(transform.position, targetPos, data.JumpHeight * Time.deltaTime);
        
        // ターゲットに近づいたらクールダウンに入る
        if(distanceToTarget < 0.1f)
        {
            elapsedIdleTime = 0f;    // クールダウンをリセット
            IsAnimation = false;     // アニメーションフラグをリセット
        }
    }

    /// <summary>
    /// 敵のジャンプアニメーションを開始する処理
    /// </summary>
    void StartAnimation(FrogDataSO data)
    {
        if (IsAnimation) return;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        targetPos = transform.position + (Vector3)(direction * data.JumpHeight);
        changeAnimation.Invoke(data.JumpAnimationName);
        IsAnimation = true;
    }
    
}