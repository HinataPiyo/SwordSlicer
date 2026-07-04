using UnityEngine;

public class TrollMovement : EnemyMovement
{
    protected override void UpdateMovement()
    {
        MoveByProgressToTarget();
        UpdateScale();
    }

    protected override void ConvertData() { }
}