using UnityEngine;
using UnityEngine.UIElements;

public class SelectButtonPanel : UIModuleBase
{
    Button toUpgradePanelButton;
    Button toSelectGameModePanelButton;
    Button toSaveAndLoadButton;
    Button toAudioSettingsPanelButton;
    public override void BindNavigation(IPanelNavigationController controller)
    {
        if(controller is not IHomePanelNavigationController homeController)
        {
            Debug.LogWarning("SelectButtonPanel: IHomePanelNavigationController is required for home navigation.");
            return;
        }

        controller.BindNextButton(toSelectGameModePanelButton, homeController.SelectGameModePanel);
        controller.BindNextButton(toUpgradePanelButton, homeController.UpgradePanel);
        controller.BindNextButton(toSaveAndLoadButton, homeController.SaveAndLoadPanel);
        controller.BindNextButton(toAudioSettingsPanelButton, homeController.AudioSettingsPanel);
    }
    
    protected override void Initialize()
    {
        toUpgradePanelButton = Root.Q<VisualElement>("ToUpgradePanelButton").Q<Button>();
        toSelectGameModePanelButton = Root.Q<VisualElement>("ToSelectGameModePanelButton").Q<Button>();
        toSaveAndLoadButton = Root.Q<VisualElement>("ToSaveAndLoadButton").Q<Button>();
        toAudioSettingsPanelButton = Root.Q<VisualElement>("ToAudioSettingsButton").Q<Button>();
    }
}