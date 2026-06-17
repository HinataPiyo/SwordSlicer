using UnityEngine;

public class SwordAttack : MonoBehaviour, ISword
{
    [SerializeField] LayerMask enemyLayer;    // 敵のレイヤー 
    const float MIN_ATTACK_INTERVAL = 0.01f;
    const float DEFAULT_ATTACK_INTERVAL = 0.1f;
    const float ROTATE_AMOUNT_MULTIPLIER = 0.01f;

    SwordControl swordControl;
    float attackInterval;
    float elapsedTime = 0f;
    bool isAttacking = false;
    public SwordDataSO Data { get; private set; }

    void Awake()
    {
        swordControl = GetComponent<SwordControl>();
    }

    public void Initialize(SwordDataSO data)
    {
        Data = data;
    }

    /// <summary>
    /// 攻撃間隔を回転量に応じて変化させる
    /// </summary>
    float GetAttackInterval()
    {
        // 回転量に応じて攻撃間隔を短くする
        attackInterval = StatContext.I.SwordAttackInterval(swordControl.RotateAmount);
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
        health.TakeDamage(StatContext.I.GetDamageAmount(Data, out bool isClitical), isClitical, transform.position);
        AudioManager.I.PlaySE("SwordAttack");
        Reset();
    }

    EnemyHealth GetEnemiesInRange()
    {
        Collider2D hitColliders = Physics2D.OverlapCircle(transform.position, StatContext.I.SwordAttackRange(), enemyLayer);
        EnemyHealth enemyHealth = hitColliders?.GetComponent<EnemyHealth>();
        return enemyHealth;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(StatContext.I == null)
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, StatContext.I.SwordAttackRange());
        }
    }
}