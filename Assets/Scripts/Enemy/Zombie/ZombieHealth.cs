using UnityEngine;

public class ZombieHealth : EnemyHealth
{
    ZombieDataSO zombieData;
    ZombieMovement zombieMovement;

    float zmobieMaxHealth;

    void Awake()
    {
        zombieMovement = GetComponent<ZombieMovement>();
    }

    protected override void ConvertData()
    {
        if(Data is not ZombieDataSO)
        {
            Debug.LogError("ZombieHealth: Data is not ZombieDataSO");
            return;
        }

        zombieData = Data as ZombieDataSO;
    }

    public override void Initialize(EnemyDataSO enemyData, float enemyStatusMultiplier)
    {
        base.Initialize(enemyData, enemyStatusMultiplier);
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
            Debug.Log($"Zombie is now invulnerable. Current health: {CurrentHealth}, Max health: {zmobieMaxHealth}");
        }
    }

    bool CalculateHealthRate()
    {
        if (zmobieMaxHealth <= 0f) return false;

        // 現在の体力がHealthDecreaseRate以下になったらtrueを返す
        return CurrentHealth / zmobieMaxHealth <= zombieData.HealthDecreaseRate;
    }
}