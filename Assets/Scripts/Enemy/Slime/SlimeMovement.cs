using UnityEngine;

public class SlimeMovement : EnemyMovement
{
    protected override void UpdateMovement()
    {
        MoveByProgressToTarget();
        UpdateScale();
    }
}