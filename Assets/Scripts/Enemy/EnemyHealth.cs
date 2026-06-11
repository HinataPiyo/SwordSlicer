using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 10f;
    public float CurrentHealth { get; private set; }

    void Awake()
    {
        CurrentHealth = maxHealth;
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