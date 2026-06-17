using UnityEngine;

public class FrogMovement : EnemyMovement
{
    FrogDataSO convertData;
    float elapsedIdleTime;
    public bool IsAnimation { get; private set; }    // アニメーション中かどうかのフラグ
    Vector3 targetPos;

    protected override void ConvertData()
    {
        if (Data is FrogDataSO frogData)
        {
            convertData = frogData;   // キャストしたデータを再度EnemyDataに格納
        }
        else
        {
            Debug.LogError("FrogMovement requires FrogDataSO");
        }
    }

    protected override void UpdateMovement()
    {
        elapsedIdleTime += Time.deltaTime;
        if(elapsedIdleTime < convertData.JumpCooldown) return;
        // ジャンプ中
        StartAnimation();
        UpdateScale();
        Jump();
    }

    /// <summary>
    /// 敵がジャンプする処理
    /// </summary>
    void Jump()
    {
        float distanceToTarget = Vector2.Distance(transform.position, targetPos);       // ターゲットまでの距離を計算
        // １秒で到達するようにする
        transform.position = Vector2.MoveTowards(transform.position, targetPos, convertData.JumpHeight * Time.deltaTime);
        
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
    void StartAnimation()
    {
        if (IsAnimation) return;
        targetPos = transform.position + Vector3.down * convertData.JumpHeight;
        changeAnimation.Invoke("Move");
        IsAnimation = true;
    }
    
}