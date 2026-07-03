using UnityEngine;

public class ZombieController : EnemyController
{
    protected override void Awake()
    {
        base.Awake();
        ZombieMovement zombieMovement = movement as ZombieMovement;
        zombieMovement.OnInvulnerableChanged += (isInvulnerable) =>
        {
            ChangeAnimationState("Invulnerable", isInvulnerable);
        };
            
    }
}