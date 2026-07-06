using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour
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
        ResolveTypedData();    // 敵の種類ごとのデータを変換して保存
    }

    /// <summary>
    /// 敵の移動処理を更新する関数
    /// </summary>
    public void Tick()
    {
        if(GameManager.IsGameOver || GameManager.IsGameStop) return;    // ゲームオーバー時は敵を出現させない

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
        AdvanceProgress();
        transform.position = EvaluatePosition();
    }

    /// <summary>
    /// 敵の種類ごとのデータを変換して保存するフック。
    /// </summary>
    bool HasReachedBorderLine()
    {
        return Mathf.Abs(transform.position.y - targetPosition.y) < ReachedBorderLineThreshold;
    }

    /// <summary>
    /// 敵の種類ごとのデータを変換して保存するフック。
    /// </summary>
    float GetCurrentPathProgress()
    {
        return CalculatePathProgress(startPosition, targetPosition, transform.position);
    }

    /// <summary>
    /// 敵の種類ごとのデータを変換して保存するフック。
    /// </summary>
    /// <param name="pathStart"> 経路の開始位置</param>
    /// <param name="pathEnd"> 経路の終了位置</param>
    /// <param name="currentPos"> 現在の位置</param>
    /// <returns> 経路上の進行度（0..1）</returns>
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

    /// <summary>
    /// 敵の種類ごとのデータを変換して保存するフック。
    /// </summary>
    protected void ApplyScaleFromProgress(float scaleProgress)
    {
        // sqrt で序盤を大きく、終盤を緩やかにするイージング。
        scaleProgress = Mathf.Sqrt(Mathf.Clamp01(scaleProgress));
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, scaleProgress);
    }

    /// <summary>
    /// 敵がボーダーラインに到達したときの処理を行うフック。
    /// </summary>
    void ReachedBorderLine()
    {
        GameManager.OnGameOver?.Invoke();
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
    /// 敵の移動処理を更新する関数。
    /// 共通の移動フローを基底側で固定し、派生はフックだけを上書きして差分を実装する。
    /// </summary>
    protected virtual void UpdateMovement()
    {
        if (ShouldPauseMovement()) return;

        AdvanceProgress();
        transform.position = EvaluatePosition();
        ApplyScaleFromProgress(EvaluateScaleProgress());
        OnMoved();
    }

    /// <summary>
    /// 移動停止状態かどうかを返す。派生で停止条件を差し込む。
    /// </summary>
    protected virtual bool ShouldPauseMovement() => false;

    /// <summary>
    /// 進行度の進め方を差し替えるためのフック。
    /// </summary>
    protected virtual void AdvanceProgress()
    {
        if (Data == null) return;
        if (Data.ReachDuration <= Mathf.Epsilon)
        {
            progress = 1f;
            return;
        }

        progress += Time.deltaTime / Data.ReachDuration;
    }

    /// <summary>
    /// 進行度から位置を計算するフック。
    /// </summary>
    protected virtual Vector2 EvaluatePosition()
    {
        return Vector2.Lerp(startPosition, targetPosition, Mathf.Clamp01(progress));
    }

    /// <summary>
    /// スケール進行度を計算するフック。
    /// </summary>
    protected virtual float EvaluateScaleProgress()
    {
        return GetCurrentPathProgress();
    }

    /// <summary>
    /// 移動とスケール更新後に追加処理を行うためのフック。
    /// </summary>
    protected virtual void OnMoved() { }
    protected virtual void ResolveTypedData() {}
}

/// <summary>
/// 敵の種類ごとのデータを変換して保存するためのジェネリック基底クラス。
/// </summary>
/// <typeparam name="TData"> 敵の種類ごとのデータ型</typeparam>
public abstract class EnemyMovement<TData> : EnemyMovement where TData : EnemyDataSO
{
    protected TData TypedData { get; private set; }

    protected sealed override void ResolveTypedData()
    {
        if (Data is TData typedData)
        {
            TypedData = typedData;
            OnTypedDataReady(typedData);
            return;
        }

        TypedData = null;
        Debug.LogError($"{GetType().Name} requires {typeof(TData).Name} but got {Data?.GetType().Name ?? "null"}.", this);
    }

    protected bool TryGetTypedData(out TData data)
    {
        if (TypedData != null)
        {
            data = TypedData;
            return true;
        }

        if (Data is TData typedData)
        {
            TypedData = typedData;
            data = TypedData;
            return true;
        }

        Debug.LogError($"{GetType().Name} requires {typeof(TData).Name} but got {Data?.GetType().Name ?? "null"}.", this);
        data = null;
        return false;
    }

    protected virtual void OnTypedDataReady(TData data) { }
}