using System.Collections.Generic;
using UnityEngine;

public partial class EnemySpawnController : MonoBehaviour
{
    [SerializeField] float spawnRange;
    [SerializeField] Transform borderLine;
    [SerializeField] float borderLineRange;     // ボーダーラインの範囲
    [SerializeField] float spawnInterval = 1f;    // 敵の出現間隔
    [SerializeField] int maxEnemyCount = 10;    // 最大出現数

    [SerializeField] GameObject[] enemyPrefab;
    [SerializeField] int spawnIndex;    // テスト

    float elapsedTime;
    List<EnemyController> enemies = new List<EnemyController>();

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= spawnInterval && enemies.Count < maxEnemyCount)
        {
            SpawnEnemy();
            elapsedTime = 0f;
        }
    }

    /// <summary>
    /// 敵を出現させる処理
    /// </summary>
    public void SpawnEnemy()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(transform.position.x - spawnRange / 2, transform.position.x + spawnRange / 2),
            transform.position.y
        );

        EnemyController enemy = Instantiate(enemyPrefab[spawnIndex], spawnPos, Quaternion.identity).GetComponent<EnemyController>();
        enemy.Initialize(GetTrgetPosition(), -(enemies.Count + 1));
        enemy.RegisterDestroy(() => RemoveEnemy(enemy));    // 敵が削除されるときにリストからも削除する
        enemies.Add(enemy);
    }

    /// <summary>
    /// 敵が削除されるときの処理
    /// </summary>
    public void RemoveEnemy(EnemyController enemy)
    {
        Destroy(enemy.gameObject);    // 敵オブジェクトを削除
        enemies.Remove(enemy);
    }

    /// <summary>
    /// ボーダーラインの範囲内でランダムな位置を取得する処理
    /// </summary>
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