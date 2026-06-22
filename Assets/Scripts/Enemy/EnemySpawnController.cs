using System.Collections.Generic;
using UnityEngine;

public partial class EnemySpawnController : MonoBehaviour
{
    [SerializeField] float spawnRange;
    [SerializeField] Transform borderLine;
    [SerializeField] float borderLineRange;     // ボーダーラインの範囲

    [SerializeField] EnemySpawnScheduleSO spawnSchedule;    // 敵の出現スケジュール
    [SerializeField] float spawnInterval;        // 敵の出現間隔

    float elapsedTime;
    float unlockElapsedTime;
    List<EnemyController> enemies = new List<EnemyController>();
    List<EnemySpawnScheduleSO.Entry> unlockedEntries = new List<EnemySpawnScheduleSO.Entry>();

    void Update()
    {
        var deltaTime = Time.deltaTime;

        unlockElapsedTime += deltaTime;
        elapsedTime += deltaTime;

        if(elapsedTime >= spawnInterval)
        {
            elapsedTime = 0f;
            SpawnEnemy();
        }

        CheckUnlockEnemyType();
    }

    /// <summary>
    /// 敵の種類がアンロックされるかどうかをチェックする処理
    /// </summary>
    void CheckUnlockEnemyType()
    {
        for (int i = 0; i < spawnSchedule.Entries.Length; i++)
        {
            // すでにアンロック済みの敵の種類はスキップする
            if(unlockedEntries.Contains(spawnSchedule.Entries[i])) continue;

            // 敵の種類がアンロックされる時間を過ぎていたら、アンロック済みリストに追加する
            if (unlockElapsedTime >= spawnSchedule.Entries[i].enemyTypeUnlockInterval) unlockedEntries.Add(spawnSchedule.Entries[i]);
        }
    }

    /// <summary>
    /// 敵の種類を確率に応じて決定する処理
    /// </summary>
    GameObject ChooseSpawnEnemy()
    {
        float totalWeight = 0f;

        // アンロック済みの敵の種類の確率の合計を計算する
        foreach (var enemy in unlockedEntries)
        {
            totalWeight += enemy.probability;
        }

        // 0からtotalWeightまでのランダムな値を生成する
        float randomValue = Random.Range(0f, totalWeight);

        float currentWeightSum = 0f;
        // ランダムな値がどの敵の種類に属するかを決定する
        foreach (var enemy in unlockedEntries)
        {
            // 現在の確率の合計に現在の敵の種類の確率を加算する
            currentWeightSum += enemy.probability;
            if (randomValue <= currentWeightSum)
            {
                return enemy.enemyPrefab;
            }
        }

        return unlockedEntries[unlockedEntries.Count - 1].enemyPrefab; // 万が一のため、最後の敵を返す
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

        EnemyController enemy = Instantiate(ChooseSpawnEnemy(), spawnPos, Quaternion.identity).GetComponent<EnemyController>();
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