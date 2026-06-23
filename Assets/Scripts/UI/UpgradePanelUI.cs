using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradePanelUI : UIModuleBase
{
    [SerializeField] VisualTreeAsset temp_upgradeElement;
    ScrollView scrollView;
    VisualElement swordOriginalData;
    static readonly Vector2 DataPanelOffset = new Vector2(100, -50);
    List<VisualElement> swordIcons = new List<VisualElement>();
    List<UpgradeElementUI> upgradeElements = new List<UpgradeElementUI>();


    /// <summary>
    /// ShowPanelControllerのBindBackButtonを呼び出して
    /// </summary>
    public override void BindNavigation(ShowPanelController controller)
    {
        controller.BindBackButton(Root.Q<VisualElement>("BackButton").Q<Button>());
    }

    protected override void Initialize()
    {
        swordIcons = Root.Q("sword-icon-container").Query<VisualElement>("upgrade-sword-icon-root").ToList();
        scrollView = Root.Q<ScrollView>();
        swordOriginalData = Root.Q("SwordOriginalData");
        swordOriginalData.style.display = DisplayStyle.None;     // 元の剣のデータは表示しない
        scrollView.Clear();
        upgradeElements.Clear();

        List<UpgradeEntry> entries = StatContext.I.GetUpgradeEntries();
        for(int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var element = temp_upgradeElement.Instantiate();
            
            UpgradeElementUI upgradeElementUI = new UpgradeElementUI();
            upgradeElementUI.Initialize(element, entry);

            scrollView.Add(element);
            upgradeElements.Add(upgradeElementUI);
        }

        var swordDatas = StatContext.I.GetSwordData();
        for(int i = 0; i < swordIcons.Count; i++)
        {
            if(i >= swordDatas.Length)
                continue;

            // クロージャ対策
            int index = i;
            VisualElement swordIcon = swordIcons[index];
            BattleSettingConfig.SwordDataByType swordData = swordDatas[index];

            // イベント発火登録
            swordIcon.RegisterCallback<PointerEnterEvent>(evt => ShowSwordData(swordIcon, swordData));
            swordIcon.RegisterCallback<PointerLeaveEvent>(evt => HideSwordData());

            // テキスト設定
            Sprite icon = swordData.swordDataSO.Icon;
            VisualElement image = swordIcon.Q("image");
            Label type_name = swordIcon.Q<Label>("sword-rarity-name");

            image.style.backgroundImage = new StyleBackground(icon);
            type_name.text = swordData.GetTypeName();
        }
    }

    /// <summary>
    /// 剣のアイコンにマウスオーバーしたときに、剣の元のデータを表示する
    /// </summary>
    void ShowSwordData(VisualElement icon, BattleSettingConfig.SwordDataByType data)
    {
        swordOriginalData.style.display = DisplayStyle.Flex;
        Vector2 localPos = swordOriginalData.parent.WorldToLocal(icon.worldBound.center);
        swordOriginalData.style.left = localPos.x - DataPanelOffset.x;
        swordOriginalData.style.top = localPos.y - DataPanelOffset.y;

        Label name = swordOriginalData.Q<Label>("name-value");
        Label effect = swordOriginalData.Q<Label>("effect-value");

        name.text = data.GetSwordName();
        effect.text = data.GetEffectDescription();
    }

    void HideSwordData() => swordOriginalData.style.display = DisplayStyle.None;
}