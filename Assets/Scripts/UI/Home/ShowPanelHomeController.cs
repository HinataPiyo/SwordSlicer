using UnityEngine;
using UnityEngine.UIElements;

public interface IShowPanel {
    public VisualElement Root { get; }
    public void SetModule(VisualElement moduleRoot);
    public void BindNavigation(IPanelNavigationController controller);     // パネルのナビゲーション（ボタンとコントローラーの紐付け）を行う
}

public interface IPanelNavigationController
{
    public void BindBackButton(Button button);
    public void BindNextButton(Button button, IShowPanel nextPanel);
}

public interface IHomePanelNavigationController : IPanelNavigationController
{
    public UpgradePanelUI UpgradePanel { get; }
    public SelectButtonPanel SelectButtonPanel { get; }
    public SelectGameModePanel SelectGameModePanel { get; }
    public SaveAndLoadPanel SaveAndLoadPanel { get; }
    public AudioSettingsPanel AudioSettingsPanel { get; }
}

public class ShowPanelHomeController : ShowPanelNavigationControllerBase, IHomePanelNavigationController
{
    [field: SerializeField] public UpgradePanelUI UpgradePanel { get; private set; }
    [field: SerializeField] public SelectButtonPanel SelectButtonPanel { get; private set; }
    [field: SerializeField] public SelectGameModePanel SelectGameModePanel { get; private set; }
    [field: SerializeField] public SaveAndLoadPanel SaveAndLoadPanel { get; private set; }
    [field: SerializeField] public AudioSettingsPanel AudioSettingsPanel { get; private set; }

    const string UPGRADE_PANEL_NAME = "UpgradePanel";
    const string SELECT_BUTTON_PANEL_NAME = "SelectButtonPanel";
    const string SELECT_GAME_MODE_PANEL_NAME = "SelectGameModePanel";
    const string SAVE_AND_LOAD_PANEL_NAME = "SaveAndLoadPanel";
    public const string AUDIO_SETTINGS_PANEL_NAME = "AudioSettingsPanel";

    void Start()
    {
        InitializeDocument();
        UpgradePanel.SetModule(SearchModule(UPGRADE_PANEL_NAME));
        SelectButtonPanel.SetModule(SearchModule(SELECT_BUTTON_PANEL_NAME));
        SelectGameModePanel.SetModule(SearchModule(SELECT_GAME_MODE_PANEL_NAME));
        SaveAndLoadPanel.SetModule(SearchModule(SAVE_AND_LOAD_PANEL_NAME));
        AudioSettingsPanel.SetModule(SearchModule(AUDIO_SETTINGS_PANEL_NAME));

        // 起動直後は全パネルを非表示にして、Startで初期パネルだけ表示する。
        UpgradePanel.Root.style.display = DisplayStyle.None;
        SelectButtonPanel.Root.style.display = DisplayStyle.None;
        SelectGameModePanel.Root.style.display = DisplayStyle.None;
        SaveAndLoadPanel.Root.style.display = DisplayStyle.None;
        AudioSettingsPanel.Root.style.display = DisplayStyle.None;

        UpgradePanel.BindNavigation(this);
        SelectButtonPanel.BindNavigation(this);
        SelectGameModePanel.BindNavigation(this);
        SaveAndLoadPanel.BindNavigation(this);
        AudioSettingsPanel.BindNavigation(this);

        ShowPanel(SelectButtonPanel);
    }
}