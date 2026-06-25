using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeElementUI
{
    Label statName;
    Label currentValue;
    Label currentLevel;
    Label price;
    Button upgradeButton;
    Button downButton;
    Button upButton;
    List<VisualElement> releaseIcons = new List<VisualElement>();

    /// <summary>
    /// アップグレード要素のUIを初期化する
    /// </summary>
    /// <param name="root">生成されたTemplateのRoot</param>
    public void Initialize(VisualElement root, UpgradeEntry entry)
    {
        statName = root.Q<Label>("stat-name");
        currentValue = root.Q<Label>("stat-value");
        currentLevel = root.Q<Label>("level-value");
        price = root.Q<Label>("price-value");
        upgradeButton = root.Q<Button>("release-button");
        downButton = root.Q<Button>("left-button");
        upButton = root.Q<Button>("right-button");

        // 攻撃力は基本的にレベル制限がないためここで分岐させる
        bool disableAdjustLevel = entry.levelProperty.UpgradeType == UpgradeType.SwordStrength 
        || entry.levelProperty.UpgradeType == UpgradeType.SwordCreateInterval
        || entry.levelProperty.UpgradeType == UpgradeType.SwordStock
        || entry.levelProperty.UpgradeType == UpgradeType.CriticalRate
        || entry.levelProperty.UpgradeType == UpgradeType.CriticalDamageMultiplier;

        VisualElement adjust_level_container = root.Q("adjust-level-container");

        // レベル制限がある場合はレベル調整UIを表示し、ない場合は非表示にする
        if(!disableAdjustLevel)
        {
            adjust_level_container.style.display = DisplayStyle.Flex;
            releaseIcons = root.Q("level-element-container").Query<VisualElement>("level-element").ToList();

            // 想定しているレベルの数だけ用意する
            for(int i = 0; i < releaseIcons.Count; i++)
            {
                if(i >= entry.levelProperty.MaxLevel - 1)
                {
                    releaseIcons[i].style.display = DisplayStyle.None;
                }
            }
        }
        else
        {
            adjust_level_container.style.display = DisplayStyle.None;
            upgradeButton.text = "強化";
            releaseIcons = new List<VisualElement>();
        }

        void Load()
        {
            currentLevel.text = entry.levelProperty.CurrentLevel.ToString();
            currentValue.text = entry.currentValue();
            CheckLevelMax(entry, disableAdjustLevel);           // レベルが最大かどうかをチェックしてUIを更新
            UpdateReleaseIcons(entry.levelProperty.ReleaseLevel, entry.levelProperty.CurrentLevel);
        }


        // 解放
        upgradeButton.clicked += () =>
        {

            entry.levelProperty.ReleaseUp();
            entry.levelProperty.LevelUp();

            Load();
            AudioManager.I.PlaySE("ReleaseButton");

            SpendCurrency(entry.price());
            HasEnoughCurrency(entry.price());
        };

        // レベルダウン
        downButton.clicked += () =>
        {
            entry.levelProperty.LevelDown();
            Load();
            AudioManager.I.PlaySE("LevelCountDown");
        };

        // レベルアップ
        upButton.clicked += () =>
        {
            entry.levelProperty.LevelUp();
            Load();
            AudioManager.I.PlaySE("LevelCountUp");
        };

        statName.text = entry.statName;     // ステータスの名前
        Load();     // 更新

        HasEnoughCurrency(entry.price());     // 通貨が足りているかどうかをチェックしてUIを更新
    }

    /// <summary>
    /// レベルが最大かどうかをチェックして、UIを更新する
    /// </summary>
    void CheckLevelMax(UpgradeEntry entry, bool disableAdjustLevel)
    {
        bool isMaxLevel = entry.levelProperty.IsReleaseMax();     // 解放レベルが最大かどうか
        upgradeButton.SetEnabled(!isMaxLevel);              // 最大レベルの場合は解放

        upgradeButton.text = isMaxLevel ? "最大" : (disableAdjustLevel ? "強化" : "解放");        // ボタンのテキストも変更
        price.text = isMaxLevel ? "-" : entry.price().ToString("#,###");        // 価格も最大の場合は表示しない
    }

    /// <summary>
    /// 通貨が足りているかどうかをチェックして、UIを更新する
    /// </summary>
    void HasEnoughCurrency(int price)
    {
        bool hasEnough = CurrencyManager.Currency >= price;
        upgradeButton.SetEnabled(hasEnough);
    }

    /// <summary>
    /// 通貨を消費する処理
    /// </summary>
    void SpendCurrency(int price)
    {
        CurrencyManager.SpendCurrency(price);
    }

    /// <summary>
    /// 解放アイコンの表示を更新する
    /// </summary>
    void UpdateReleaseIcons(int releaseLevel, int currentLevel)
    {
        for(int i = 0; i < releaseIcons.Count; i++)
        {
            if(i < releaseLevel)
            {
                releaseIcons[i].AddToClassList("release");
            }
            if(i < currentLevel)
            {
                releaseIcons[i].AddToClassList("enable");
            }
            if(i >= currentLevel && i < releaseLevel)
            {
                releaseIcons[i].RemoveFromClassList("enable");
            }
        }
    }
}

public class UpgradeEntry
{
    public string statName;
    public System.Func<string> currentValue;
    public LevelProperty levelProperty;
    public System.Func<int> price;
}