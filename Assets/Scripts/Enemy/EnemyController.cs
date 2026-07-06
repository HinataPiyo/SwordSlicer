using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    [SerializeField] EnemyDataSO enemyDataSO;

    protected EnemyMovement movement;
    protected EnemyHealth health;

    Animator animator;
    const string ANIMATOR_PARAM_DEAD = "Die";

    public EnemyDataSO Data => enemyDataSO;


    protected virtual void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
        if(animator == null) Debug.LogWarning("Animatorが見つかりません");
    }

    /// <summary>
    /// 敵が出現する際の初期化処理
    /// </summary>
    public virtual void Initialize(Vector2 borderLinePos, Vector2 startPos, float enemyStatusMultiplier)
    {
        movement.Initialize(enemyDataSO, borderLinePos, startPos, ChangeAnimationState);
        health.Initialize(enemyDataSO, enemyStatusMultiplier);

        RegisterDieAnimation();     // 死亡アニメーションを登録
    }

    protected virtual void Update()
    {
        if(health.IsDead) return;    // 死亡している場合は移動しない

        // 敵の移動とその他の処理を更新
        movement.Tick();
    }

    /// <summary>
    /// 敵のアニメーションを変更する(Trigger)
    /// </summary>
    protected void ChangeAnimationState(string stateName)
    {
        if (animator == null) return;

        animator.SetTrigger(stateName);
    }

    /// <summary>
    /// 敵のアニメーションを変更する(Bool)
    /// </summary>
    protected void ChangeAnimationState(string stateName, bool isLooping)
    {
        if (animator == null) return;

        animator.SetBool(stateName, isLooping);
    }

    /// <summary>
    /// 敵が削除されるときのコールバックを登録する
    /// </summary>
    public void RegisterDestroyEvent(System.Action onRemove) => health.RegisterDestroyEvent(onRemove);
    void RegisterDieAnimation() => health.dieAnimation += () => ChangeAnimationState(ANIMATOR_PARAM_DEAD);    // 死亡アニメーションを登録
}

public abstract class EnemyController<TData> : EnemyController where TData : EnemyDataSO
{
    TData cachedData;

    /// <summary>
    /// Data を TData として取得する。成功時は内部キャッシュを返す。
    /// </summary>
    protected bool TryGetData(out TData data)
    {
        if (cachedData != null)
        {
            data = cachedData;
            return true;
        }

        if (Data is TData typedData)
        {
            cachedData = typedData;
            data = cachedData;
            return true;
        }

        Debug.LogError($"{GetType().Name} requires {typeof(TData).Name} but got {Data?.GetType().Name ?? "null"}.", this);
        data = null;
        return false;
    }

    /// <summary>
    /// Data を TData として取得する。成功時は内部キャッシュを返す。
    /// </summary>
    public bool TryGetEnemyData(out TData data)
    {
        return TryGetData(out data);
    }

    /// <summary>
    /// Data を TData として取得する。成功時は内部キャッシュを返す。
    /// </summary>
    protected virtual void OnValidate()
    {
        if (Data == null) return;
        if (Data is TData) return;

        Debug.LogWarning($"{GetType().Name} expects {typeof(TData).Name}, but inspector has {Data.GetType().Name}.", this);
    }
}