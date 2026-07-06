using UnityEngine;

public class ZombieHealth : EnemyHealth<ZombieDataSO>
{
    ZombieMovement zombieMovement;

    float zmobieMaxHealth;

    void Awake()
    {
        zombieMovement = GetComponent<ZombieMovement>();
    }

    public override void Initialize(EnemyDataSO enemyData, float enemyStatusMultiplier)
    {
        base.Initialize(enemyData, enemyStatusMultiplier);

        if (!TryGetTypedData(out _))
        {
            enabled = false;
            return;
        }

        zmobieMaxHealth = maxHealth;
        zombieMovement.IsInvulnerable = false;
        OnHealthChanged -= HandleHealthChanged;
        OnHealthChanged += HandleHealthChanged;
    }

    public override bool CheckAttackable(Vector2 attackPoint)
    {
        if (zombieMovement.IsInvulnerable) return false;

        return base.CheckAttackable(attackPoint);
    }

    public override bool TakeDamage(float damage, bool isCritical, Vector2 damagePosition)
    {
        if (zombieMovement.IsInvulnerable) return false;

        return base.TakeDamage(damage, isCritical, damagePosition);
    }

    void HandleHealthChanged(float healthRate)
    {
        if (zombieMovement.IsInvulnerable) return;
        if (CurrentHealth <= 0f) return;

        // 体力が減少した割合を計算し、HealthDecreaseRate分減少した場合は無敵状態にする
        if (CalculateHealthRate())
        {
            zmobieMaxHealth = CurrentHealth;
            zombieMovement.IsInvulnerable = true;
        }
    }

    bool CalculateHealthRate()
    {
        if (!TryGetTypedData(out var data)) return false;
        if (zmobieMaxHealth <= 0f) return false;

        // 現在の体力がHealthDecreaseRate以下になったらtrueを返す
        return CurrentHealth / zmobieMaxHealth <= data.HealthDecreaseRate;
    }
}