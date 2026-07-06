using UnityEngine.UIElements;

public class RetreatPanel : UIModuleBase
{
    Button retreatButton;
    
    protected override void Initialize()
    {
        retreatButton = Root.Q<VisualElement>("RetreatButton").Q<Button>();
        
        retreatButton.clicked += OnRetreaButtonClicked;
    }

    void OnRetreaButtonClicked()
    {
        Root.style.display = DisplayStyle.None;

        // ゲーム終了
        ServiceLocator.Get<IGameOver>().GameOver();
        GameManager.OnGameOver?.Invoke();
    }

    public override void BindNavigation(IPanelNavigationController controller)
    {
        controller.BindBackButton(Root.Q<Button>("BackButton"));
    }
}