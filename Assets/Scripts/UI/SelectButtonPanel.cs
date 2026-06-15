using UnityEngine;
using UnityEngine.UIElements;

public class SelectButtonPanel : UIModuleBase
{
    Button toUpgradePanelButton;
    Button toSelectGameModePanelButton;
    public override void BindNavigation(ShowPanelController controller)
    {
        controller.BindNextButton(toSelectGameModePanelButton, controller.SelectGameModePanel);
        controller.BindNextButton(toUpgradePanelButton, controller.UpgradePanel);
    }
    
    protected override void Initialize()
    {
        toUpgradePanelButton = Root.Q<VisualElement>("ToUpgradePanelButton").Q<Button>();
        toSelectGameModePanelButton = Root.Q<VisualElement>("ToSelectGameModePanelButton").Q<Button>();
    }
}