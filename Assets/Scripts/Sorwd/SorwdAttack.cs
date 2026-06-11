using System.Collections.Generic;
using UnityEngine;

public class SorwdAttack : MonoBehaviour
{
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float strength = 1f;
    [SerializeField] float attackInterval;

    const float MIN_ATTACK_INTERVAL = 0.01f;
    const float DEFAULT_ATTACK_INTERVAL = 0.1f;
    const float ROTATE_AMOUNT_MULTIPLIER = 0.01f;

    SorwdControl sorwdControl;
    float elapsedTime = 0f;
    bool isAttacking = false;

    void Awake()
    {
        sorwdControl = GetComponent<SorwdControl>();
    }

    /// <summary>
    /// 攻撃間隔を回転量に応じて変化させる
    /// </summary>
    float GetAttackInterval()
    {
        // 回転量に応じて攻撃間隔を短くする
        attackInterval = DEFAULT_ATTACK_INTERVAL - (sorwdControl.RotateAmount * ROTATE_AMOUNT_MULTIPLIER);
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
        List<EnemyHealth> enemies = GetEnemiesInRange();
        if(enemies == null) return;

        foreach (EnemyHealth enemy in enemies)
        {
            Debug.Log($"AttackInterval: {GetAttackInterval()}s, RotateAmount: {sorwdControl.RotateAmount}");
            WorldCanvasManager.I.ShowDamageText(strength, transform.position);    // ダメージテキストを表示
            enemy.TakeDamage(strength);
            Reset();
        }
    }

    List<EnemyHealth> GetEnemiesInRange()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        if(hitColliders.Length <= 0) return null;

        List<EnemyHealth> enemies = new List<EnemyHealth>();
        foreach (Collider2D collider in hitColliders)
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null) enemies.Add(enemyHealth);
        }

        return enemies;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}