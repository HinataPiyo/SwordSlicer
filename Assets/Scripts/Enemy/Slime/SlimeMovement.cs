using UnityEngine;

public class SlimeMovement : EnemyMovement
{
    protected override void UpdateMovement()
    {
        transform.position = Vector2.MoveTowards(startPosition, targetPosition, progress);
        UpdateScale();
    }
}