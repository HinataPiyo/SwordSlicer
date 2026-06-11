using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] float spawnRange;
    [SerializeField] Transform borderLine;
    [SerializeField] float borderLineRange;     // ボーダーラインの範囲
    [SerializeField] float spawnInterval = 1f;    // 敵の出現間隔
    [SerializeField] int maxEnemyCount = 10;    // 最大出現数

    [SerializeField] GameObject enemyPrefab;        // テスト

    float elapsedTime;
    int currentEnemyCount;

    void Start()
    {
        
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= spawnInterval && currentEnemyCount < maxEnemyCount)
        {
            SpawnEnemy();
            elapsedTime = 0f;
            currentEnemyCount++;
        }
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