using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour, IEnemy
{
    // startPosition -> targetPosition を 0..1 で表した進行度。
    // 0: 開始地点, 1: ボーダーライン到達。
    protected float progress;
    protected Vector2 targetPosition;    // ボーダーラインの位置
    protected Vector2 startPosition;
    protected System.Action<string> changeAnimation;
    public EnemyDataSO Data { get; private set; }

    const float ReachedBorderLineThreshold = 0.1f;


    public void Initialize(EnemyDataSO enemyDataSO)
    {
        Data = enemyDataSO;
    }

    /// <summary>
    /// 敵の出現と同時に初期化するためのオーバーロードされたInitialize関数
    /// </summary>
    public virtual void Initialize(EnemyDataSO enemyDataSO, Vector2 borderLinePos, Vector2 startPos, System.Action<string> changeAnimation)
    {
        Initialize(enemyDataSO);

        // 位置とスケールの計算基準になる経路を保存。
        startPosition = startPos;     // 出現位置を保存
        targetPosition = borderLinePos;

        // 生成済みオブジェクトの現在座標から進行度を逆算し、見た目の連続性を保つ。
        progress = GetCurrentPathProgress();
        UpdateScale();

        this.changeAnimation = changeAnimation; // アニメーション変更のコールバックを保存
        ConvertData();    // 敵の種類ごとのデータを変換して保存
    }

    /// <summary>
    /// 敵の移動処理を更新する関数
    /// </summary>
    public void Tick()
    {
        if(GameManager.IsGameOver) return;    // ゲームオーバー時は敵を出現させない

        if (HasReachedBorderLine())
        {
            ReachedBorderLine();
            return;
        }
    
        UpdateMovement();
    }

    /// <summary>
    /// ReachDuration に応じて進行度を進め、開始位置からボーダーラインまで線形移動する
    /// </summary>
    protected void MoveByProgressToTarget()
    {
        // ReachDuration 秒で progress が 0 -> 1 へ進む。
        progress += Time.deltaTime / Data.ReachDuration;

        // 経路上の位置は Lerp(start, target, progress) で決定する。
        transform.position = Vector2.Lerp(startPosition, targetPosition, Mathf.Clamp01(progress));
    }

    bool HasReachedBorderLine()
    {
        return Mathf.Abs(transform.position.y - targetPosition.y) < ReachedBorderLineThreshold;
    }

    float GetCurrentPathProgress()
    {
        return CalculatePathProgress(startPosition, targetPosition, transform.position);
    }

    protected float CalculatePathProgress(Vector2 pathStart, Vector2 pathEnd, Vector2 currentPos)
    {
        Vector2 path = pathEnd - pathStart;
        float pathLengthSq = path.sqrMagnitude;
        if (pathLengthSq <= Mathf.Epsilon) return 1f;

        Vector2 fromStartToCurrent = currentPos - pathStart;
        // 射影係数 t = dot(current-start, end-start) / |end-start|^2
        // t が 0..1 の範囲なら経路上、範囲外は Clamp で端点に寄せる。
        float t = Vector2.Dot(fromStartToCurrent, path) / pathLengthSq;
        return Mathf.Clamp01(t);
    }

    protected void ApplyScaleFromProgress(float scaleProgress)
    {
        // sqrt で序盤を大きく、終盤を緩やかにするイージング。
        scaleProgress = Mathf.Sqrt(Mathf.Clamp01(scaleProgress));
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, scaleProgress);
    }

    void ReachedBorderLine()
    {
        GameManager.OnGameOver?.Invoke();
        Debug.Log("Enemy reached the border line!");
    }

    /// <summary>
    /// 敵の出現アニメーションを更新する関数
    /// </summary>
    protected void UpdateScale()
    {
        // 現在位置から求めた進行度をそのままスケール進行度として使う。
        float scaleProgress = GetCurrentPathProgress();
        ApplyScaleFromProgress(scaleProgress);
    }

    /// <summary>
    /// 敵の移動処理を更新する関数
    /// </summary>
    protected abstract void UpdateMovement();
    protected virtual void ConvertData() {}
}