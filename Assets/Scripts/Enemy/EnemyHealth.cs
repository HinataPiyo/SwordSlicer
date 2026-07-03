using UnityEngine;

public abstract class EnemyHealth : MonoBehaviour, IEnemy
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
    System.Action dieAnimation;
    protected Vector2 attackPoint;

    [SerializeField] AnimationDestroyEvent destroyEvent;    // 敵が死亡したときのアニメーションイベント

    public virtual bool CheckAttackable(Vector2 attackPoint)
    {
        this.attackPoint = attackPoint;
        // 継承しない場合は常に攻撃可能とする
        return true;
    }
    public void Initialize(EnemyDataSO enemyData) { }
    public virtual void Initialize(EnemyDataSO enemyData, float enemyStatusMultiplier)
    {
        Data = enemyData;
        maxHealth = enemyData.MaxHealth * enemyStatusMultiplier;    // 難易度に応じて敵の最大体力を調整
        CurrentHealth = maxHealth;
        Debug.Log($"Enemy initialized with max health: {maxHealth}");
        IsDead = false;
        ConvertData();
    }

    public void RegisterDestroy(System.Action onRemove)
    {
        destroyEvent.RegisterDestroy(() => onRemove.Invoke());    // アニメーションイベントに死亡処理を登録
    }

    public void RegisterDieAnimation(System.Action dieAnimation)
    {
        this.dieAnimation += dieAnimation;    // 死亡アニメーションを登録
    }

    public void Tick() { }    // Update関数
    
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
    void Die()
    {
        dieAnimation.Invoke();      // 死亡アニメーションを再生
        IsDead = true;
        Debug.Log("Enemy died");
    }

    protected virtual void ConvertData() {}
}