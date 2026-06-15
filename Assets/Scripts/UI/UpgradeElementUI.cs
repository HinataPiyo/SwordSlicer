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

        statName.text = entry.statName;
        currentValue.text = entry.currentValue;
        currentLevel.text = entry.currentLevel.ToString();
        price.text = entry.price.ToString("#,###");
    }
}

public class UpgradeEntry
{
    public string statName;
    public string currentValue;
    public int currentLevel;
    public int price;
}