using UnityEngine;

public class EnemyHealth : MonoBehaviour, IEnemy
{
    float maxHealth;
    public float CurrentHealth { get; private set; }

    public EnemyDataSO Data { get; private set; }
    System.Action onRemove;    // 敵が削除されるときのコールバック

    public void Initialize(EnemyDataSO enemyData)
    {
        Data = enemyData;
        maxHealth = Data.MaxHealth;
        CurrentHealth = maxHealth;
    }

    public void RegisterDestroy(System.Action onRemove)
    {
        this.onRemove += onRemove;
    }

    /// <summary>
    /// 敵がダメージを受ける処理
    /// </summary>
    public void TakeDamage(float damage)
    {
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
        Destroy(gameObject);
    }
}