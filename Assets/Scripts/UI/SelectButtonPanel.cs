using UnityEngine;
using UnityEngine.UIElements;

public class SelectButtonPanel : UIModuleBase
{
    Button toUpgradePanelButton;
    Button toSelectGameModePanelButton;
    Button toSaveAndLoadButton;
    Button toAudioSettingsPanelButton;
    public override void BindNavigation(ShowPanelController controller)
    {
        controller.BindNextButton(toSelectGameModePanelButton, controller.SelectGameModePanel);
        controller.BindNextButton(toUpgradePanelButton, controller.UpgradePanel);
        controller.BindNextButton(toSaveAndLoadButton, controller.SaveAndLoadPanel);
        controller.BindNextButton(toAudioSettingsPanelButton, controller.AudioSettingsPanel);
    }
    
    protected override void Initialize()
    {
        toUpgradePanelButton = Root.Q<VisualElement>("ToUpgradePanelButton").Q<Button>();
        toSelectGameModePanelButton = Root.Q<VisualElement>("ToSelectGameModePanelButton").Q<Button>();
        toSaveAndLoadButton = Root.Q<VisualElement>("ToSaveAndLoadButton").Q<Button>();
        toAudioSettingsPanelButton = Root.Q<VisualElement>("ToAudioSettingsButton").Q<Button>();
    }
}