using UnityEngine;

public class EnemyHealth : MonoBehaviour, IEnemy
{
    float maxHealth;
    public float CurrentHealth { get; private set; }

    public EnemyDataSO Data { get; private set; }
    public bool IsDead { get; private set; }
    System.Action dieAnimation;

    [SerializeField] AnimationDestroyEvent destroyEvent;    // 敵が死亡したときのアニメーションイベント

    public void Initialize(EnemyDataSO enemyData)
    {
        Data = enemyData;
        maxHealth = Data.MaxHealth;
        CurrentHealth = maxHealth;
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
    public void TakeDamage(float damage)
    {
        if(IsDead) return;    // すでに死亡している場合はダメージを受けない

        maxHealth -= damage;
        Debug.Log($"Enemy took {damage} damage, current health: {maxHealth}");
        if (maxHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 敵が死亡する処理
    /// </summary>
    void Die()
    {
        dieAnimation.Invoke();
        IsDead = true;
    }
}