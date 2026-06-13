using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SwordStockUI : MonoBehaviour
{
    public enum AnimationType
    {
        None,
        Add,
        Remove
    }

    UIDocument uiDocs;

    ProgressBar stockIntervalBar;
    List<VisualElement> stockIcons = new List<VisualElement>();

    void Awake()
    {
        uiDocs = GetComponent<UIDocument>();
        var r = uiDocs.rootVisualElement;
        stockIntervalBar = r.Q<ProgressBar>("sword-stock-interval-progressbar");
        stockIcons = r.Q<VisualElement>("sword-stock-list").Query<VisualElement>("image").ToList();

        stockIntervalBar.value = 0f;
        stockIntervalBar.highValue = 1f;
        stockIntervalBar.title = "0%";

        for(int i = 0; i < stockIcons.Count; i++)
        {
            StockIconUpdate(null, i);
        }
    }

    /// <summary>
    /// ストックアイコンを更新する
    /// </summary>
    void StockIconUpdate(Sprite sprite, int targetIcon)
    {
        var stockIcon = stockIcons[targetIcon];
        if(sprite == null)
        {
            // ストックがない場合はアイコンを非表示にする
            stockIcon.style.backgroundImage = null;
            return;
        }

        // ストックがある場合はアイコンを表示する
        stockIcon.style.backgroundImage = new StyleBackground(sprite);
    }

    /// <summary>
    /// 剣を生成する時間を可視化したプログレスバーを更新する
    /// </summary>
    public void StockIntervalBarUpdate(float progress)
    {
        // プログレスバーの値は0~1の範囲で設定する必要があるため、Clamp01で値を制限する
        stockIntervalBar.value = Mathf.Clamp01(progress);
        stockIntervalBar.title = $"{progress * 100f:0}%";       // 100f:0は小数点以下を表示しないようにするための書式指定子
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="swordStock"></param>
    /// <param name="animationType">Add : swordStockの最後尾をAnimationする。 Removeは先頭はdequeueAnimationを行い、戦闘以外はAddAniamtionをする</param>
    public void UpdateIcons(Queue<SwordDataSO> swordStock, AnimationType animationType)
    {
        for(int i = 0; i < stockIcons.Count; i++)
        {
            if(i < swordStock.Count)
            {
                Sprite icon = swordStock.ToArray()[i].Icon;
                StockIconUpdate(icon, i);       // アイコンを更新

                if(animationType == AnimationType.Add && i == swordStock.Count - 1)
                {
                    StartCoroutine(ChangeStockIconStyle(i, animationType));     // アイコンのスタイルを変更
                }
                else if(animationType == AnimationType.Remove && i == 0)
                {
                    StartCoroutine(ChangeStockIconStyle(i, animationType));     // アイコンのスタイルを変更
                }
            }
            else
            {
                StockIconUpdate(null, i);
            }
        }
    }

    /// <summary>
    /// StockIconのスタイルを変更する
    /// </summary>
    IEnumerator ChangeStockIconStyle(int targetIcon, AnimationType animationType)
    {
        var stockIcon = stockIcons[targetIcon];     // 対象となるアイコン
        Debug.Log($"Changing style of Stock Icon {targetIcon} with AnimationType {animationType}");
        stockIcon.RemoveFromClassList("dequeue");
        stockIcon.RemoveFromClassList("on");
        stockIcon.RemoveFromClassList("add");

        switch(animationType)
        {
            case AnimationType.Add:
                stockIcon.AddToClassList("add");
                yield return null;      // 1フレーム待機してからonクラスを追加することで、addクラスのアニメーションが再生されるようにする
                stockIcon.AddToClassList("on");
                yield break;
            case AnimationType.Remove:
                stockIcon.AddToClassList("on");
                stockIcon.AddToClassList("dequeue");

                yield return new WaitForSeconds(0.2f);     // dequeueアニメーションの時間
                yield return ChangeStockIconStyle(targetIcon, AnimationType.Add);     // スタイルをリセット
                yield break;
        }
    }

}