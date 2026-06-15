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
        releaseIcons = root.Q("level-element-container").Query<VisualElement>("level-element").ToList();

        void Load()
        {
            currentLevel.text = entry.levelProperty.CurrentLevel.ToString();
            currentValue.text = entry.currentValue();
            price.text = entry.price().ToString("#,###");
            Debug.Log($"ReleaseLevel: {entry.levelProperty.ReleaseLevel}, CurrentLevel: {entry.levelProperty.CurrentLevel}");
            UpdateReleaseIcons(entry.levelProperty.ReleaseLevel, entry.levelProperty.CurrentLevel);
        }

        // 解放
        upgradeButton.clicked += () =>
        {
            entry.levelProperty.ReleaseUp();
            Load();
        };

        // レベルダウン
        downButton.clicked += () =>
        {
            entry.levelProperty.LevelDown();
            Load();
        };

        // レベルアップ
        upButton.clicked += () =>
        {
            entry.levelProperty.LevelUp();
            Load();
        };

        statName.text = entry.statName;     // ステータスの名前
        Load();     // 更新
    }

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