using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour, IEnemy
{
    protected float progress;
    protected Vector2 targetPosition;    // ボーダーラインの位置
    protected Vector2 startPosition;
    protected System.Action<string> changeAnimation;
    public EnemyDataSO Data { get; private set; }


    public void Initialize(EnemyDataSO enemyDataSO)
    {
        Data = enemyDataSO;
    }

    /// <summary>
    /// 敵の出現と同時に初期化するためのオーバーロードされたInitialize関数
    /// </summary>
    public void Initialize(EnemyDataSO enemyDataSO, Vector2 borderLinePos, System.Action<string> changeAnimation)
    {
        Initialize(enemyDataSO);

        transform.localScale = Vector3.zero;    // 出現時は小さくする
        startPosition = transform.position;     // 出現位置を保存
        targetPosition = borderLinePos;

        this.changeAnimation = changeAnimation; // アニメーション変更のコールバックを保存
        ConvertData();    // 敵の種類ごとのデータを変換して保存
    }

    /// <summary>
    /// 敵の移動処理を更新する関数
    /// </summary>
    public void Tick()
    {
        // 出現からボーダーラインに到達するまでの時間を計算
        progress += Time.deltaTime / Data.ReachDuration;

        if (Mathf.Abs(transform.position.y - targetPosition.y) < 0.1f)
        {
            Debug.Log("Enemy reached the border line!");
            return;
        }
    
        UpdateMovement();
    }

    /// <summary>
    /// 敵の出現アニメーションを更新する関数
    /// </summary>
    protected void UpdateScale()
    {
        // 出現からボーダーラインに到達するまでの距離を計算して、0から1の値を得る
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        float totalDistance = Vector2.Distance(startPosition, targetPosition);
        float scaleProgress = 1f - (distanceToTarget / totalDistance);
        // イージング：序盤は速く、後半は遅くなるようにスムーズに調整
        scaleProgress = Mathf.Sqrt(scaleProgress);
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, scaleProgress);
    }

    /// <summary>
    /// 敵の移動処理を更新する関数
    /// </summary>
    protected abstract void UpdateMovement();
    protected abstract void ConvertData();
}