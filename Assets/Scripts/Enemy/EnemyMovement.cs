using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float duration = 10f;      // 敵がボーダーラインに到達するまでの時間
    float progress;
    Vector2 targetPosition;    // ボーダーラインの位置
    Vector2 startPosition;

    public void Initialize(Vector2 borderLinePos)
    {
        transform.localScale = Vector3.zero;    // 出現時は小さくする
        startPosition = transform.position;     // 出現位置を保存
        targetPosition = borderLinePos;
    }

    void Update()
    {
        // 出現からボーダーラインに到達するまでの時間を計算
        progress += Time.deltaTime / duration;

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            Debug.Log("Enemy reached the border line!");
            return;
        }

        UpdateScale();
        UpdateMovement();
    }

    void UpdateScale()
    {
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);    // 徐々に大きくする
    }

    void UpdateMovement()
    {
        transform.position = Vector2.MoveTowards(startPosition, targetPosition, progress); 
    }
}