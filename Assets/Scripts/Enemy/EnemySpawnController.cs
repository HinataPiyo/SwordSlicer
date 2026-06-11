using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] float spawnRange;
    [SerializeField] Transform borderLine;
    [SerializeField] float borderLineRange;     // ボーダーラインの範囲

    [SerializeField] GameObject enemyPrefab;        // テスト

    void Start()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(transform.position.x - spawnRange / 2, transform.position.x + spawnRange / 2),
            transform.position.y
        );
        GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        obj.GetComponent<EnemyMovement>().Initialize(GetTrgetPosition());     // ボーダーラインの位置を渡す
    }

    Vector2 GetTrgetPosition()
    {
        return new Vector2(
            Random.Range(borderLine.position.x - borderLineRange / 2, borderLine.position.x + borderLineRange / 2),
            borderLine.position.y
        );
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange, 0.1f, 0));    // 出現範囲を表示
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(borderLine.position, new Vector3(borderLineRange, 0.1f, 0));    // ボーダーラインの範囲を表示
    }
}