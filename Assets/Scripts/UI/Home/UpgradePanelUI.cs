using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradePanelUI : UIModuleBase
{
    [SerializeField] VisualTreeAsset temp_upgradeElement;
    ScrollView scrollView;
    VisualElement swordOriginalData;
    static readonly Vector2 DataPanelOffset = new Vector2(100, -50);
    List<SwordIconUI> swordIcons = new List<SwordIconUI>();
    List<UpgradeElementUI> upgradeElements = new List<UpgradeElementUI>();

    public static System.Action OnUpgradePurchased;   // 強化購入が成立したときに呼ばれるイベント

    void Awake()
    {
        BattleSettingConfig.OnLoadLevelProperties += Initialize;
        OnUpgradePurchased += () => CheckUpgradeButtonIntaractable();
        ServiceLocator.Get<ILoad>().OnLoad += () => CheckUpgradeButtonIntaractable();       // ロード完了時に強化ボタンが押せるか確認する
    }
    
    /// <summary>
    /// ShowPanelControllerのBindBackButtonを呼び出して
    /// </summary>
    public override void BindNavigation(IPanelNavigationController controller)
    {
        controller.BindBackButton(Root.Q<VisualElement>("BackButton").Q<Button>());
    }

    void CheckUpgradeButtonIntaractable()
    {
        foreach(var element in upgradeElements)
        {
            element.CheckIntaractableButtons();
        }
    }

    protected override void Initialize()
    {
        // -- 初期化
        var Icons = Root.Q("sword-icon-container").Query<VisualElement>("upgrade-sword-icon-root").ToList();

        scrollView = Root.Q<ScrollView>();
        swordOriginalData = Root.Q("SwordOriginalData");
        swordOriginalData.style.display = DisplayStyle.None;     // 元の剣のデータは表示しない
        scrollView.Clear();
        upgradeElements.Clear();

        List<UpgradeEntry> entries = ServiceLocator.Get<IStateService>().UpgradeEntries();
        for(int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var element = temp_upgradeElement.Instantiate();
            
            UpgradeElementUI upgradeElementUI = new UpgradeElementUI();
            upgradeElementUI.Initialize(element, entry);

            scrollView.Add(element);
            upgradeElements.Add(upgradeElementUI);
        }

        // -- データ反映
        var swordDatas = ServiceLocator.Get<IStateService>().SwordDatas();

        for(int i = 0; i < Icons.Count; i++)
        {
            if(i >= swordDatas.Length)
                continue;

            int index = i;  // クロージャのためにローカル変数に格納する
            SwordIconUI swordIconUI = new SwordIconUI();
            swordIconUI.Initialize(Icons[index], swordDatas[index]);

            swordIconUI.RegisterEnterCallback(() => ShowSwordData(Icons[index], swordDatas[index]));
            swordIconUI.RegisterLeaveCallback(() => HideSwordData());
            
            swordIcons.Add(swordIconUI);
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

public class SwordIconUI
{
    VisualElement root;
    VisualElement image;
    Label type_name;

    public void Initialize(VisualElement root)
    {
        this.root = root;
        image = root.Q("image");
        type_name = root.Q<Label>("sword-rarity-name");
    }

    public void Initialize(VisualElement root, BattleSettingConfig.SwordDataByType swordData)
    {
        Initialize(root);
        SetData(swordData);
    }

    public void SetCount(Sprite icon, int count)
    {
        image.style.backgroundImage = new StyleBackground(icon);
        type_name.text = count.ToString() + "回";
    }

    void SetData(BattleSettingConfig.SwordDataByType swordData)
    {
        Sprite icon = swordData.swordDataSO.Icon;
        image.style.backgroundImage = new StyleBackground(icon);
        type_name.text = swordData.GetTypeName();
    }

    public void RegisterEnterCallback(System.Action callback)
    {
        root.RegisterCallback<PointerEnterEvent>(evt => callback());
    }

    public void RegisterLeaveCallback(System.Action callback)
    {
        root.RegisterCallback<PointerLeaveEvent>(evt => callback());
    }
}