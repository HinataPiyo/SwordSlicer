using UnityEngine;

public class FireController : EnemyController<FireDataSO>
{
    public bool isFlip { get; private set; }

    public void Initialize(Vector2 targetPosition, Vector2 startPosition, float enemyStatusMultiplier, bool isFlip = false)
    {
        base.Initialize(targetPosition, startPosition, enemyStatusMultiplier);
        this.isFlip = isFlip;
    }
}