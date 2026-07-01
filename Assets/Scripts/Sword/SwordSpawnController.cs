using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSpawnController : MonoBehaviour, ISwordDraggingArea
{
    static readonly Vector3 SpawnPosOffset = new Vector3(0, 0.5f, 0);     // 剣の生成位置のオフセット
    [SerializeField] SwordControl sorwdControlPrefab;
    [SerializeField] SwordStockUI swordStockUI;
    [SerializeField] SwordDetailDataUI swordDetailDataUI;

    float elapsedTime = 0f;     // 経過時間

    [SerializeField] Transform swordAreaCenter;
    [SerializeField] Vector2 swordArea;

    //Queue<T>はFIFO(First In First Out)のデータ構造で、Enqueueでデータを追加し、Dequeueで最初に追加されたデータを取り出すことができる
    Queue<BattleSettingConfig.SwordDataByType> swordStock = new Queue<BattleSettingConfig.SwordDataByType>();     // ストックされた剣のデータ
    public SwordControl CurrentSword { get; private set; }     // 現在生成されている剣の参照
    public bool HasStock => swordStock.Count > 0;     // ストックがあるかどうか
    public void RemoveCurrentSword() => CurrentSword = null;     // 現在の剣を削除する

    void Update()
    {
        if(GameManager.IsGameOver) return;    // ゲームオーバー時は敵を出現させない
        
        swordDetailDataUI.UpdateData(CurrentSword);     // 剣の詳細データUIを更新
        
        if(!CanCreateData()) return;    // ストックが最大数に達している場合は新しいデータを生成しない

        elapsedTime += Time.deltaTime;
        swordStockUI.StockIntervalBarUpdate(elapsedTime / ServiceLocator.Get<IStateService>().StockInterval());     // プログレスバーを更新

        if(elapsedTime >= ServiceLocator.Get<IStateService>().StockInterval())
        {
            BattleSettingConfig.SwordDataByType newSwordData = ServiceLocator.Get<IStateService>().CreateSword();    // 新しい剣のデータを取得
            swordStock.Enqueue(newSwordData);     // ストックに剣のデータを追加
            swordStockUI.StockIntervalBarUpdate(0f);     // プログレスバーをリセット
            swordStockUI.UpdateIcons(swordStock.ToArray(), SwordStockUI.AnimationType.Add);     // ストックアイコンを更新
            elapsedTime = 0f;
        }
    }

    public void GetSwordArea(out Vector2 center, out Vector2 size)
    {
        center = swordAreaCenter.position;
        size = swordArea;
    }

    /// <summary>
    /// ストックから剣のデータを取り出して剣を生成する
    /// </summary>
    public void SpawnSorwd()
    {
        if(swordStock.Count == 0) return;    // ストックに剣のデータがない場合は何もしない
        CurrentSword = null;     // 現在の剣を削除する
        StartCoroutine(SpawnSwordRoutine());
    }

    IEnumerator SpawnSwordRoutine()
    {
        BattleSettingConfig.SwordDataByType[] stockSnapshot = swordStock.ToArray();
        BattleSettingConfig.SwordDataByType newSwordData = swordStock.Dequeue();     // ストックから剣のデータを取り出す

        CurrentSword = Instantiate(sorwdControlPrefab, transform.position + SpawnPosOffset, Quaternion.identity);
        CurrentSword.Initialize(newSwordData);     // ストックから剣のデータを取り出して剣に設定

        swordStockUI.UpdateIcons(stockSnapshot, SwordStockUI.AnimationType.Remove);     // 削除前の見た目でアニメーションを再生する
        yield return new WaitForSeconds(SwordStockUI.RemoveAnimationDuration);
        swordStockUI.UpdateIcons(swordStock.ToArray(), SwordStockUI.AnimationType.Update);     // アニメーション後に現在のストック状態へ更新する

        ServiceLocator.Get<IResultService>().Data.AddSwordCreateCount(newSwordData.type);     // 結果データに剣の生成数を追加
    }

    /// <summary>
    /// ストックが最大数に達しているかどうかを判定する
    /// </summary>
    bool CanCreateData()
    {
        return swordStock.Count < ServiceLocator.Get<IStateService>().CurrentMaxStock();
    }

    void OnDrawGizmos()
    {
        if(swordAreaCenter == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(swordAreaCenter.position, swordArea);
    }
}