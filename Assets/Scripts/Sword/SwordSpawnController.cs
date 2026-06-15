using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSpawnController : MonoBehaviour
{
    static readonly Vector3 SpawnPosOffset = new Vector3(0, 0.5f, 0);     // 剣の生成位置のオフセット
    [SerializeField] SwordControl sorwdControlPrefab;
    [SerializeField] SwordStockUI swordStockUI;

    float elapsedTime = 0f;     // 経過時間

    //Queue<T>はFIFO(First In First Out)のデータ構造で、Enqueueでデータを追加し、Dequeueで最初に追加されたデータを取り出すことができる
    Queue<SwordDataSO> swordStock = new Queue<SwordDataSO>();     // ストックされた剣のデータ
    public SwordControl CurrentSword { get; private set; }     // 現在生成されている剣の参照
    public bool HasStock => swordStock.Count > 0;     // ストックがあるかどうか
    public void RemoveCurrentSword() => CurrentSword = null;     // 現在の剣を削除する

    void Update()
    {
        if(!CanCreateData()) return;    // ストックが最大数に達している場合は新しいデータを生成しない

        elapsedTime += Time.deltaTime;
        swordStockUI.StockIntervalBarUpdate(elapsedTime / StatContext.I.GetStockInterval());     // プログレスバーを更新

        if(elapsedTime >= StatContext.I.GetStockInterval())
        {
            SwordDataSO newSwordData = StatContext.I.CreateSword();    // 新しい剣のデータを取得
            swordStock.Enqueue(newSwordData);     // ストックに剣のデータを追加
            swordStockUI.StockIntervalBarUpdate(0f);     // プログレスバーをリセット
            swordStockUI.UpdateIcons(swordStock.ToArray(), SwordStockUI.AnimationType.Add);     // ストックアイコンを更新
            elapsedTime = 0f;
        }
    }

    /// <summary>
    /// ストックから剣のデータを取り出して剣を生成する
    /// </summary>
    public void SpawnSorwd()
    {
        if(swordStock.Count == 0) return;    // ストックに剣のデータがない場合は何もしない
        StartCoroutine(SpawnSwordRoutine());
    }

    IEnumerator SpawnSwordRoutine()
    {
        SwordDataSO[] stockSnapshot = swordStock.ToArray();
        SwordDataSO newSwordData = swordStock.Dequeue();     // ストックから剣のデータを取り出す

        CurrentSword = Instantiate(sorwdControlPrefab, transform.position + SpawnPosOffset, Quaternion.identity);
        CurrentSword.Initialize(newSwordData);     // ストックから剣のデータを取り出して剣に設定

        swordStockUI.UpdateIcons(stockSnapshot, SwordStockUI.AnimationType.Remove);     // 削除前の見た目でアニメーションを再生する
        yield return new WaitForSeconds(SwordStockUI.RemoveAnimationDuration);
        swordStockUI.UpdateIcons(swordStock.ToArray(), SwordStockUI.AnimationType.Update);     // アニメーション後に現在のストック状態へ更新する
    }

    /// <summary>
    /// ストックが最大数に達しているかどうかを判定する
    /// </summary>
    bool CanCreateData()
    {
        return swordStock.Count < StatContext.I.GetCurrentMaxStock();
    }
}