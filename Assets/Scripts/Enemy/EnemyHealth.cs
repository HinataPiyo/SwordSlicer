using UnityEngine;

public abstract class EnemyHealth : MonoBehaviour
{
    protected float maxHealth;
    public event System.Action<float> OnHealthChanged;    // 敵の体力が変化したときのイベント
    float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        protected set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth / maxHealth);    // 体力の割合を通知
        }
    }

    public EnemyDataSO Data { get; private set; }
    public bool IsDead { get; private set; }
    protected bool isAttackable = true;
    protected Vector2 attackPoint;

    public event System.Action dieAnimation;
    [SerializeField] AnimationDestroyEvent destroyEvent;    // 敵が死亡したときのアニメーションイベント
    public void RegisterDestroyEvent(System.Action onRemove) => destroyEvent.OnDestroyEvent += onRemove;

    public virtual bool CheckAttackable(Vector2 attackPoint)
    {
        this.attackPoint = attackPoint;
        // 継承しない場合は常に攻撃可能とする
        return true;
    }

    public virtual void Initialize(EnemyDataSO enemyData, float enemyStatusMultiplier)
    {
        Data = enemyData;
        maxHealth = enemyData.MaxHealth * enemyStatusMultiplier;    // 難易度に応じて敵の最大体力を調整
        CurrentHealth = maxHealth;
        IsDead = false;
        ResolveTypedData();
    }
    
    /// <summary>
    /// 敵がダメージを受ける処理
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="isCritical">クリティカルかどうか</param>
    /// <param name="damagePosition">ダメージを受けた位置</param>
    /// <returns>ダメージを受けたかどうか</returns>
    public virtual bool TakeDamage(float damage, bool isCritical, Vector2 damagePosition)
    {
        if(IsDead) return false;    // 死亡している場合はダメージを受けない
        
        if(isCritical) WorldCanvasManager.I.ShowCriticalHitText(damage, damagePosition);    // クリティカルヒットのテキストを表示
        else WorldCanvasManager.I.ShowDamageText(damage, damagePosition);    // ダメージテキストを表示

        CalculateHealth(damage);

        return true;
    }

    /// <summary>
    /// 敵の体力を計算する処理
    /// </summary>
    void CalculateHealth(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 敵が死亡する処理
    /// </summary>
    protected virtual void Die()
    {
        dieAnimation?.Invoke();      // 死亡アニメーションを再生
        IsDead = true;
    }

    /// <summary>
    /// 敵の種類ごとのデータを変換して保存するフック。
    /// </summary>
    protected virtual void ResolveTypedData() {}
}

/// <summary>
/// 敵の種類ごとのデータを変換して保存するためのジェネリック基底クラス。
/// </summary>
/// <typeparam name="TData"> 敵の種類ごとのデータ型</typeparam>
public abstract class EnemyHealth<TData> : EnemyHealth where TData : EnemyDataSO
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