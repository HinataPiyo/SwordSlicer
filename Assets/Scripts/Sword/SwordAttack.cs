using UnityEngine;

public class SwordAttack : MonoBehaviour, ISword
{
    [SerializeField] LayerMask enemyLayer;    // 敵のレイヤー 
    [SerializeField] GameObject attackEffectPrefab;    // 攻撃エフェクトのプレハブ
    const float ROTATE_AMOUNT_MULTIPLIER = 0.01f;

    SwordControl swordControl;
    float attackInterval;
    float elapsedTime = 0f;
    bool isAttacking = false;
    int hitCount = 0;
    public BattleSettingConfig.SwordDataByType Data { get; private set; }

    void Awake()
    {
        swordControl = GetComponent<SwordControl>();
    }

    public void Initialize(BattleSettingConfig.SwordDataByType data)
    {
        Data = data;
    }

    /// <summary>
    /// 攻撃間隔を回転量に応じて変化させる
    /// </summary>
    float GetAttackInterval()
    {
        // 回転量に応じて攻撃間隔を短くする
        attackInterval = ServiceLocator.Get<IStateService>().SwordAttackInterval(swordControl.RotateAmount);
        return attackInterval;
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

        bool isAttackable = health.CheckAttackable();    // 敵が攻撃可能かどうかをチェック
        if(!isAttackable)
        {
            WorldCanvasManager.I.ShowAttackMissText(transform.position);    // 攻撃が当たらなかった場合はミスのテキストを表示する
            Reset();
            return;
        }

        float damage = ServiceLocator.Get<IStateService>().DamageAmount(Data, out bool isClitical);    // ダメージ量を取得

        // 攻撃範囲内の敵にダメージを与える
        bool isApplyDamage = health.TakeDamage(damage, isClitical, transform.position);
        if(isApplyDamage)
        {
            ServiceLocator.Get<IAudioService>().PlaySE("SwordAttack");      // 攻撃が当たった場合のみSEを再生する
            ServiceLocator.Get<ICameraShake>().Shake(0.2f, 0.5f);           // カメラを揺らす
            Instantiate(attackEffectPrefab, health.transform.position, Quaternion.identity);    // 攻撃エフェクトを生成する
            hitCount++;
        }

        ServiceLocator.Get<IResultService>().Data.SetMaxHitCount(hitCount);    // ヒット数をリザルトに反映する

        Reset();
    }

    EnemyHealth GetEnemiesInRange()
    {
        Collider2D hitColliders = Physics2D.OverlapCircle(transform.position, ServiceLocator.Get<IStateService>().SwordAttackRange(), enemyLayer);
        EnemyHealth enemyHealth = hitColliders?.GetComponent<EnemyHealth>();
        return enemyHealth;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(ServiceLocator.Get<IStateService>() == null)
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, ServiceLocator.Get<IStateService>().SwordAttackRange());
        }
    }
}