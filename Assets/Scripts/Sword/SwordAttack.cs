using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float strength = 1f;
    [SerializeField] float attackInterval;

    const float MIN_ATTACK_INTERVAL = 0.01f;
    const float DEFAULT_ATTACK_INTERVAL = 0.1f;
    const float ROTATE_AMOUNT_MULTIPLIER = 0.01f;

    SwordControl swordControl;
    float elapsedTime = 0f;
    bool isAttacking = false;

    public float AttackRange => attackRange;

    void Awake()
    {
        swordControl = GetComponent<SwordControl>();
    }

    /// <summary>
    /// 攻撃間隔を回転量に応じて変化させる
    /// </summary>
    float GetAttackInterval()
    {
        // 回転量に応じて攻撃間隔を短くする
        attackInterval = DEFAULT_ATTACK_INTERVAL - (swordControl.RotateAmount * ROTATE_AMOUNT_MULTIPLIER);
        return Mathf.Max(MIN_ATTACK_INTERVAL, attackInterval);
    }

    void Update()
    {
        if(isAttacking)
        {
            Attack();
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= GetAttackInterval())
            {
                isAttacking = true;
            }
        }
    }
    
    /// <summary>
    /// 攻撃終了後、攻撃状態をリセットする
    /// </summary>
    void Reset()
    {
        isAttacking = false;
        elapsedTime = 0f;
    }

    /// <summary>
    /// 攻撃範囲内の敵にダメージを与える
    /// </summary>
    void Attack()
    {
        EnemyHealth health = GetEnemiesInRange();

        if(health == null) return;

        // 攻撃範囲内の敵にダメージを与える
        health.TakeDamage(strength, transform.position);
        AudioManager.I.PlaySE("SwordAttack");
        Reset();
    }

    EnemyHealth GetEnemiesInRange()
    {
        Collider2D hitColliders = Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer);
        EnemyHealth enemyHealth = hitColliders?.GetComponent<EnemyHealth>();
        return enemyHealth;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}