using System.Collections.Generic;
using UnityEngine;

public partial class EnemySpawnController : MonoBehaviour, ISpawnKinoko, ISpawnFire
{
    [SerializeField] float spawnRange;
    [SerializeField] Transform borderLine;
    [SerializeField] float borderLineRange;     // ボーダーラインの範囲

    [SerializeField] EnemySpawnScheduleSO spawnSchedule;    // 敵の出現スケジュール
    [SerializeField] ElapsedUI elapsedUI;    // 経過時間を表示するUI

    float spawnElapsedTime;
    float unlockElapsedTime;
    List<EnemyController> enemies = new List<EnemyController>();
    List<EnemySpawnScheduleSO.Entry> unlockedEntries = new List<EnemySpawnScheduleSO.Entry>();

    public string GetDifficultyLevelText() => spawnSchedule.GetDifficultyLevelText();
    public DifficultyLevel GetDifficultyLevel() => spawnSchedule.GetDifficultyLevel();

    [SerializeField] float testUnlockElapsedTime = 300f;    // テスト用のアンロック経過時間

    void Awake()
    {
        unlockElapsedTime = testUnlockElapsedTime;
        unlockedEntries.Add(spawnSchedule.Entries[0]);    // 最初の敵の種類はアンロック済みにする
    }

    void Update()
    {
        if(GameManager.IsGameOver) return;    // ゲームオーバー時は敵を出現させない
        
        elapsedUI.UpdateElapsedTime(unlockElapsedTime);    // 経過時間をUIに反映する
        var deltaTime = Time.deltaTime;

        unlockElapsedTime += deltaTime;
        spawnElapsedTime += deltaTime;

        if(spawnElapsedTime >= spawnSchedule.SpawnInterval(unlockElapsedTime))
        {
            spawnElapsedTime = 0f;
            SpawnEnemy();
        }

        CheckUnlockEnemyType();
        ServiceLocator.Get<IResultService>().Data.SetDefenseTime(unlockElapsedTime);    // 経過時間をリザルトに反映する
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
            if (unlockElapsedTime >= spawnSchedule.EnemyTypeUnlockInterval() * unlockedEntries.Count) unlockedEntries.Add(spawnSchedule.Entries[i]);
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

        return spawnSchedule.Entries[0].enemyPrefab;
    }

    /// <summary>
    /// 敵を出現させる処理
    /// </summary>
    void SpawnEnemy()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(transform.position.x - spawnRange / 2, transform.position.x + spawnRange / 2),
            transform.position.y
        );
        Vector2 startPos = new Vector2(spawnPos.x, transform.position.y);

        EnemyController enemy = Instantiate(ChooseSpawnEnemy(), spawnPos, Quaternion.identity).GetComponent<EnemyController>();
        enemy.Initialize(GetTargetPosition(), startPos, spawnSchedule.EnemyStatusMultiplier(unlockElapsedTime));
        enemy.RegisterDestroyEvent(() => RemoveEnemy(enemy));    // 敵が削除されるときにリストからも削除する
        enemies.Add(enemy);
    }

    /// <summary>
    /// 敵を胞子によって出現させる処理
    /// </summary>
    /// <param name="enemy">キノコの親</param>
    public void SpawnEnemyBySpore(KinokoController enemy)
    {
        if (!enemy.TryGetEnemyData(out var parentData)) return;

        float offsetY = 0.2f;    // キノコの親から少し上に出現させるためのオフセット
        float randomX = Random.Range(enemy.transform.position.x - spawnRange / 2, enemy.transform.position.x + spawnRange / 2);
        float randomY = Random.Range(enemy.transform.position.y - offsetY / 2, enemy.transform.position.y + offsetY / 2);
        Vector2 spawnPos = new Vector2(randomX, randomY);
        Vector2 scaleStartPos = new Vector2(spawnPos.x, transform.position.y);

        KinokoController kinoko = Instantiate(parentData.SpawnKinokoPrefab, spawnPos, Quaternion.identity);
        kinoko.Initialize(GetTargetPosition(), spawnPos, spawnSchedule.EnemyStatusMultiplier(unlockElapsedTime), true);    // このキノコが胞子によって生成されたことを示すフラグをtrueに設定
        kinoko.SetScaleStartPosition(scaleStartPos);
        kinoko.RegisterDestroyEvent(() => RemoveEnemy(kinoko));    // 敵が削除されるときにリストからも削除する
        enemies.Add(kinoko);
    }

    public void SpawnEnemyFire(FireController _fire, bool isFlip)
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(transform.position.x - spawnRange / 2, transform.position.x + spawnRange / 2),
            transform.position.y
        );
        Vector2 startPos = new Vector2(spawnPos.x, transform.position.y);

        FireController fire = Instantiate(_fire, spawnPos, Quaternion.identity);
        fire.Initialize(GetTargetPosition(), startPos, spawnSchedule.EnemyStatusMultiplier(unlockElapsedTime), isFlip);
        fire.RegisterDestroyEvent(() => RemoveEnemy(fire));    // 敵が削除されるときにリストからも削除する
        enemies.Add(fire);
    }

    /// <summary>
    /// 敵が削除されるときの処理
    /// </summary>
    public void RemoveEnemy(EnemyController enemy)
    {
        Destroy(enemy.gameObject);    // 敵オブジェクトを削除
        enemies.Remove(enemy);
        int reward = spawnSchedule.CurrencyRewardByDifficulty(enemy.Data.CurrencyReward);    // 敵を倒したときの報酬を取得
        CurrencyManager.AddCurrency(reward);    // 敵を倒したときの報酬を追加

        // リザルト登録
        ServiceLocator.Get<IResultService>().Data.AddEnemyKillCount();    // 敵を倒したときのキル数を追加
        ServiceLocator.Get<IResultService>().Data.AddGetCurrency(reward);    // 敵を倒したと
        WorldCanvasManager.I.ShowCurrencyText(reward, enemy.transform.position);    // 敵を倒したときの報酬をUIに表示
    }

    /// <summary>
    /// ボーダーラインの範囲内でランダムな位置を取得する処理
    /// </summary>
    Vector2 GetTargetPosition()
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