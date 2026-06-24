using UnityEngine;

public abstract class EnemyHealth : MonoBehaviour, IEnemy
{
    float maxHealth;
    public float CurrentHealth { get; private set; }

    public EnemyDataSO Data { get; private set; }
    public bool IsDead { get; private set; }
    protected bool isAttackable = true;
    System.Action dieAnimation;

    [SerializeField] AnimationDestroyEvent destroyEvent;    // 敵が死亡したときのアニメーションイベント

    public virtual bool CheckAttackable()
    {
        // 継承しない場合は常に攻撃可能とする
        return true;
    }
    public void Initialize(EnemyDataSO enemyData) { }
    public void Initialize(EnemyDataSO enemyData, float enemyStatusMultiplier)
    {
        Data = enemyData;
        maxHealth = enemyData.MaxHealth * enemyStatusMultiplier;    // 難易度に応じて敵の最大体力を調整
        CurrentHealth = maxHealth;
        Debug.Log($"Enemy initialized with max health: {maxHealth}");
        IsDead = false;
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

        CalulateHealth(damage);

        return true;
    }

    void CalulateHealth(float damage)
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
}