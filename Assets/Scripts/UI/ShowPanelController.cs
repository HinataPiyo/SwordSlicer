using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IShowPanel {
    public VisualElement Root { get; }
    public void SetModule(VisualElement moduleRoot);
    public void BindNavigation(ShowPanelController controller);     // パネルのナビゲーション（ボタンとコントローラーの紐付け）を行う
}
public class ShowPanelController : MonoBehaviour
{
    UIDocument uiDoc;
    [field: SerializeField] public UpgradePanelUI UpgradePanel { get; private set; }
    [field: SerializeField] public SelectButtonPanel SelectButtonPanel { get; private set; }
    [field: SerializeField] public SelectGameModePanel SelectGameModePanel { get; private set; }
    [field: SerializeField] public SaveAndLoadPanel SaveAndLoadPanel { get; private set; }

    const string UPGRADE_PANEL_NAME = "UpgradePanel";
    const string SELECT_BUTTON_PANEL_NAME = "SelectButtonPanel";
    const string SELECT_GAME_MODE_PANEL_NAME = "SelectGameModePanel";
    const string SAVE_AND_LOAD_PANEL_NAME = "SaveAndLoadPanel";
    Stack<IShowPanel> showPanelStack = new Stack<IShowPanel>();

    void Start()
    {
        uiDoc = GetComponent<UIDocument>();
        var r = uiDoc.rootVisualElement;
        UpgradePanel.SetModule(SearchModule(UPGRADE_PANEL_NAME));
        SelectButtonPanel.SetModule(SearchModule(SELECT_BUTTON_PANEL_NAME));
        SelectGameModePanel.SetModule(SearchModule(SELECT_GAME_MODE_PANEL_NAME));
        SaveAndLoadPanel.SetModule(SearchModule(SAVE_AND_LOAD_PANEL_NAME));

        // 起動直後は全パネルを非表示にして、Startで初期パネルだけ表示する。
        UpgradePanel.Root.style.display = DisplayStyle.None;
        SelectButtonPanel.Root.style.display = DisplayStyle.None;
        SelectGameModePanel.Root.style.display = DisplayStyle.None;
        SaveAndLoadPanel.Root.style.display = DisplayStyle.None;

        UpgradePanel.BindNavigation(this);
        SelectButtonPanel.BindNavigation(this);
        SelectGameModePanel.BindNavigation(this);
        SaveAndLoadPanel.BindNavigation(this);

        ShowPanel(SelectButtonPanel);
    }

    VisualElement SearchModule(string name)
    {
        var element = uiDoc.rootVisualElement.Q(name);
        if(element == null)
        {
            Debug.LogError($"ShowPanelController: Failed to find module with name '{name}'.");
        }
        return element;
    }


    /// <summary>
    /// 戻るボタンをコントローラーに紐付ける。
    /// これにより、どのパネルの戻るボタンも同じ動作（現在のパネルを非表示にして前のパネルを表示）になる。
    /// </summary>
    public void BindBackButton(Button button)
    {
        if(button == null)
            return;

        button.clicked += HidePanel;
    }

    /// <summary>
    /// 次へボタンをコントローラーに紐付ける。
    /// これにより、どのパネルの次へボタンも同じ動作（指定されたパネルを表示）になる。
    /// </summary>
    public void BindNextButton(Button button, IShowPanel nextPanel)
    {
        if(button == null || nextPanel == null)
            return;

        button.clicked += () => ShowPanel(nextPanel);
    }

    /// <summary>
    /// 指定されたパネルを表示する。
    /// 現在表示されているパネルはスタックに積まれ、非表示になる。
    /// </summary>
    void ShowPanel(IShowPanel panel)
    {
        if(panel == null)
        {
            Debug.LogWarning("ShowPanelController: panel is null.");
            return;
        }

        // すでに表示されているパネルが同じ場合は何もしない
        if(showPanelStack.Count > 0 && showPanelStack.Peek() == panel)
            return;

        if(showPanelStack.Count > 0)
        {
            showPanelStack.Peek().Root.style.display = DisplayStyle.None;    // 現在のパネルを非表示にする
        }

        panel.Root.style.display = DisplayStyle.Flex;   // 新しいパネルを表示する
        showPanelStack.Push(panel);    // 新しいパネルをスタックに積む

        ServiceLocator.Get<IAudioService>().PlaySE("NormalButton");
    }

    /// <summary>
    /// 現在のパネルを非表示にして、前のパネルを表示する。
    /// スタックから現在のパネルを取り出して非表示にし、次にスタックのトップにある前のパネルを表示する。
    /// </summary>
    void HidePanel()
    {
        if(showPanelStack.Count == 0)
        {
            Debug.LogWarning("ShowPanelController: No panel to hide.");
            return;
        }

        showPanelStack.Pop().Root.style.display = DisplayStyle.None;    // 現在のパネルを非表示にする

        if(showPanelStack.Count > 0)
        {
            showPanelStack.Peek().Root.style.display = DisplayStyle.Flex;    // 前のパネルを表示する
        }

        ServiceLocator.Get<IAudioService>().PlaySE("BackButton");
    }
}