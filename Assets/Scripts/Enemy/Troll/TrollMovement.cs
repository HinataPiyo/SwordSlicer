using UnityEngine;

public class TrollMovement : EnemyMovement
{
    protected override void UpdateMovement()
    {
        transform.position = Vector2.MoveTowards(startPosition, targetPosition, progress);
        UpdateScale();
    }

    protected override void ConvertData() { }
}