using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectGameModePanel : UIModuleBase
{
    public enum GameMode { Normal, }

    public class GameModeEntry
    {
        public GameMode Mode { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        public string SceneName { get; private set; }
        public System.Action OnClick { get; private set; }

        public void Initialize(GameMode mode, string displayName, string description, string sceneName, System.Action onClick)
        {
            Mode = mode;
            DisplayName = displayName;
            Description = description;
            SceneName = sceneName;
            OnClick = onClick;
        }
    }

    [SerializeField] VisualTreeAsset temp_GameModeEntry;
    VisualElement gameModeEntryContainer;

    public override void BindNavigation(ShowPanelController controller)
    {
        controller.BindBackButton(Root.Q<VisualElement>("BackButton").Q<Button>());
    }

    protected override void Initialize()
    {
        gameModeEntryContainer = Root.Q<VisualElement>("game-mode-entry-container");
        gameModeEntryContainer.Clear();
        List<GameModeEntry> gameModeEntries = new List<GameModeEntry>();

        // ゲームモードのエントリーを作成
        GameModeEntry normalModeEntry = new GameModeEntry();
        normalModeEntry.Initialize(
            GameMode.Normal,
            "ノーマル",
            "基本的なゲームモードです。",
            "GameScene",
            () => {
                Debug.Log("Normal Mode Selected");
                // シーン遷移などの処理をここに書く
                UnityEngine.SceneManagement.SceneManager.LoadScene(normalModeEntry.SceneName);
            }
        );
    
        gameModeEntries.Add(normalModeEntry);

        // UI生成
        foreach(var entry in gameModeEntries)
        {
            var element = temp_GameModeEntry.Instantiate();
            Button selectButton = element.Q<Button>();
            selectButton.clicked += () => entry.OnClick?.Invoke();

            gameModeEntryContainer.Add(element);
        }
    }
}