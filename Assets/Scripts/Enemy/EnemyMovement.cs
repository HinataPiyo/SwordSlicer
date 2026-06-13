using UnityEngine;

public class EnemyMovement : MonoBehaviour, IEnemy
{
    float progress;
    Vector2 targetPosition;    // ボーダーラインの位置
    Vector2 startPosition;
    public EnemyDataSO Data { get; private set; }


    public void Initialize(EnemyDataSO enemyDataSO)
    {
        Data = enemyDataSO;
    }

    /// <summary>
    /// 敵の出現と同時に初期化するためのオーバーロードされたInitialize関数
    /// </summary>
    public void Initialize(EnemyDataSO enemyDataSO, Vector2 borderLinePos)
    {
        Initialize(enemyDataSO);

        transform.localScale = Vector3.zero;    // 出現時は小さくする
        startPosition = transform.position;     // 出現位置を保存
        targetPosition = borderLinePos;
    }

    /// <summary>
    /// 敵の移動処理を更新する関数
    /// </summary>
    public void Tick()
    {
        // 出現からボーダーラインに到達するまでの時間を計算
        progress += Time.deltaTime / Data.ReachDuration;

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            Debug.Log("Enemy reached the border line!");
            return;
        }

        UpdateScale();
        UpdateMovement();
    }

    /// <summary>
    /// 敵の出現アニメーションを更新する関数
    /// </summary>
    void UpdateScale()
    {
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);    // 徐々に大きくする
    }

    /// <summary>
    /// 敵の移動処理を更新する関数
    /// </summary>
    void UpdateMovement()
    {
        transform.position = Vector2.MoveTowards(startPosition, targetPosition, progress); 
    }
}