using UnityEngine;

public class ZombieController : EnemyController<ZombieDataSO>
{
    ZombieMovement zombieMovement;
    const string ANIMATOR_PARAM_INVULNERABLE = "Invulnerable";

    protected override void Awake()
    {
        base.Awake();
        zombieMovement = movement as ZombieMovement;
        if (zombieMovement == null) return;

        zombieMovement.OnInvulnerableChanged += HandleInvulnerableChanged;
    }

    /// <summary>
    /// 敵が削除されるときのコールバックを登録する
    /// </summary>
    void OnDestroy()
    {
        if (zombieMovement == null) return;

        zombieMovement.OnInvulnerableChanged -= HandleInvulnerableChanged;
    }

    void HandleInvulnerableChanged(bool isInvulnerable)
    {
        ChangeAnimationState(ANIMATOR_PARAM_INVULNERABLE, isInvulnerable);
    }
}