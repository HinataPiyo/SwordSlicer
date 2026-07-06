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
    UpgradeEntry entry;
    bool disableAdjustLevel;

    /// <summary>
    /// アップグレード要素のUIを初期化する
    /// </summary>
    /// <param name="root">生成されたTemplateのRoot</param>
    public void Initialize(VisualElement root, UpgradeEntry entry)
    {
        this.entry = entry;
        statName = root.Q<Label>("stat-name");
        currentValue = root.Q<Label>("stat-value");
        currentLevel = root.Q<Label>("level-value");
        price = root.Q<Label>("price-value");
        upgradeButton = root.Q<Button>("release-button");
        downButton = root.Q<Button>("left-button");
        upButton = root.Q<Button>("right-button");

        // 攻撃力は基本的にレベル制限がないためここで分岐させる
        disableAdjustLevel = entry.levelProperty.UpgradeType == UpgradeType.SwordStrength 
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


        // 解放
        upgradeButton.clicked += () =>
        {
            int upgradeCost = entry.price();
            // 通貨が足りない場合は処理を中断する
            if (!SpendCurrency(upgradeCost))
            {
                CheckIntaractableButtons();     // ボタンの状態を更新して、押せない状態にする
                return;
            }

            entry.levelProperty.ReleaseUp();
            entry.levelProperty.LevelUp();
            CheckIntaractableButtons();
            UpgradePanelUI.OnUpgradePurchased?.Invoke();   // 通貨変動が発生するため他要素の押下可否も更新する

            Load();
            ServiceLocator.Get<IAudioService>().PlaySE("ReleaseButton");
        };

        // レベルダウン
        downButton.clicked += () =>
        {
            entry.levelProperty.LevelDown();
            Load();
            ServiceLocator.Get<IAudioService>().PlaySE("LevelCountDown");
        };

        // レベルアップ
        upButton.clicked += () =>
        {
            entry.levelProperty.LevelUp();
            Load();
            ServiceLocator.Get<IAudioService>().PlaySE("LevelCountUp");
        };

        statName.text = entry.statName;     // ステータスの名前
        Load();     // 更新

        CheckIntaractableButtons();
    }

    void Load()
    {
        currentLevel.text = entry.levelProperty.CurrentLevel.ToString();
        currentValue.text = entry.currentValue();
        UpdateReleaseIcons(entry.levelProperty.ReleaseLevel, entry.levelProperty.CurrentLevel);
    }

    public void CheckIntaractableButtons()
    {
        bool isIntaractable = !CheckLevelMax() && HasEnoughCurrency(entry.price());
        upgradeButton.SetEnabled(isIntaractable);
        currentLevel.text = entry.levelProperty.CurrentLevel.ToString();
        Debug.Log(entry.statName + ":" + entry.levelProperty.CurrentLevel + " / " + entry.currentValue());
        currentValue.text = entry.currentValue();
    }

    /// <summary>
    /// レベルが最大かどうかをチェックして、UIを更新する
    /// </summary>
    bool CheckLevelMax()
    {
        bool isMaxLevel = entry.levelProperty.IsReleaseMax();     // 解放レベルが最大かどうか

        upgradeButton.text = isMaxLevel ? "最大" : (disableAdjustLevel ? "強化" : "解放");        // ボタンのテキストも変更
        price.text = isMaxLevel ? "-" : entry.price().ToString("#,###");        // 価格も最大の場合は表示しない

        return isMaxLevel;
    }

    /// <summary>
    /// 通貨が足りているかどうかをチェックして、UIを更新する
    /// </summary>
    bool HasEnoughCurrency(int price)
    {
        bool hasEnough = CurrencyManager.Currency >= price;
        return hasEnough;
    }

    /// <summary>
    /// 通貨を消費する処理
    /// </summary>
    bool SpendCurrency(int price)
    {
        return CurrencyManager.SpendCurrency(price);
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