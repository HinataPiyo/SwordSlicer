using Unity.VisualScripting;
using UnityEngine;

public class GoblinMovement : EnemyMovement
{
    GoblinDataSO convertData;
    float elapsedIdleTime;
    bool flipX = false;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void ConvertData()
    {
        // GoblinDataSOにキャストして、Goblin専用のデータを取得
        GoblinDataSO goblinData = Data as GoblinDataSO;
        if (goblinData != null)
        {
            convertData = goblinData;
        }
        else
        {
            Debug.LogError("GoblinMovement: Data is not of type GoblinDataSO");
        }
    }

    protected override void UpdateMovement()
    {
        elapsedIdleTime += Time.deltaTime;
        if (elapsedIdleTime < convertData.MoveCooldown) return;
        Move();
        Reset();
    }

    void Move()
    {
        changeAnimation.Invoke(convertData.MoveAnimationName);
        flipX = !flipX;    // 次回の移動時に反転
        spriteRenderer.flipX = flipX;    // スプライトの反転を切り替え
        transform.position += Vector3.down * convertData.MoveDistance;       // 一瞬で移動する

        UpdateScale();
    }

    void Reset()
    {
        elapsedIdleTime = 0f;    // クールダウンをリセット
    }
}