using UnityEngine;

public class DevilMovement : EnemyMovement
{
    protected override void UpdateMovement()
    {
        // 移動しない
        transform.localScale = new Vector3(1f, 1f, 1f);    // スケールは常に1にする
    }
}