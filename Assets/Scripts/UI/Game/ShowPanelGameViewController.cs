using UnityEngine;
using UnityEngine.UIElements;


public class ShowPanelGameViewController : ShowPanelNavigationControllerBase
{
    [field: SerializeField] public AudioSettingsPanel AudioSettingsPanel { get; private set; }
    [field: SerializeField] public RetreatPanel RetreatPanel { get; private set; }

    const string RESTREA_PANEL_NAME = "RetreatPanel";
    const string TO_AUDIO_SETTINGS_BUTTON_NAME = "ToAudioSettingsButton";
    const string TO_RETREAT_PANEL_BUTTON_NAME = "ToRetreatPanelButton";

    Button toAudioSettingsButton;
    Button toRetreatPanelButton;


    void Start()
    {
        InitializeDocument();

        AudioSettingsPanel.SetModule(SearchModule(ShowPanelHomeController.AUDIO_SETTINGS_PANEL_NAME));
        RetreatPanel.SetModule(SearchModule(RESTREA_PANEL_NAME));
        
        AudioSettingsPanel.Root.style.display = DisplayStyle.None;
        RetreatPanel.Root.style.display = DisplayStyle.None;
        
        AudioSettingsPanel.BindNavigation(this);
        RetreatPanel.BindNavigation(this);

        toAudioSettingsButton = UiDoc.rootVisualElement.Q<VisualElement>(TO_AUDIO_SETTINGS_BUTTON_NAME)?.Q<Button>();
        toRetreatPanelButton = UiDoc.rootVisualElement.Q<VisualElement>(TO_RETREAT_PANEL_BUTTON_NAME)?.Q<Button>();
        toAudioSettingsButton.clicked += OnToAudioSettingsButtonClicked;
        toRetreatPanelButton.clicked += OnToRetreaPanelButtonClicked;
    }

    void OnToAudioSettingsButtonClicked()
    {
        ShowPanel(AudioSettingsPanel);
    }

    void OnToRetreaPanelButtonClicked()
    {
        // ゲーム停止
        ServiceLocator.Get<IGameStop>().GameStop();
        ShowPanel(RetreatPanel);
    }
}