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

    [SerializeField] EnemySpawnScheduleSO spawnScheduleSO;

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
        GameModeEntry easyDifficultyEntry = new GameModeEntry();
        easyDifficultyEntry.Initialize(
            GameMode.Normal,
            "魔の森から離れた平原（イージー）",
            "敵が弱く、出現間隔も長い難易度。初心者向け。",
            "GameScene",
            () => {
                Debug.Log("Easy Mode Selected");
                spawnScheduleSO.SetDifficultyLevel(DifficultyLevel.Easy);
                UnityEngine.SceneManagement.SceneManager.LoadScene(easyDifficultyEntry.SceneName);
            }
        );


        GameModeEntry normalDifficultyEntry = new GameModeEntry();
        normalDifficultyEntry.Initialize(
            GameMode.Normal,
            "魔の森の入口（ノーマル）",
            "通常の難易度。徐々に強くなる敵を倒し続けるサバイバルモード。",
            "GameScene",
            () => {
                Debug.Log("Normal Mode Selected");
                spawnScheduleSO.SetDifficultyLevel(DifficultyLevel.Normal);
                UnityEngine.SceneManagement.SceneManager.LoadScene(normalDifficultyEntry.SceneName);
            }
        );
        
        GameModeEntry hardDifficultyEntry = new GameModeEntry();
        hardDifficultyEntry.Initialize(
            GameMode.Normal,
            "魔の森の中枢（ハード）",
            "敵がより強く、出現間隔も短くなる難易度。より高いスコアを目指す挑戦者向け。",
            "GameScene",
            () => {
                Debug.Log("Hard Mode Selected");
                spawnScheduleSO.SetDifficultyLevel(DifficultyLevel.Hard);
                UnityEngine.SceneManagement.SceneManager.LoadScene(hardDifficultyEntry.SceneName);
            }
        );

        GameModeEntry extremeDifficultyEntry = new GameModeEntry();
        extremeDifficultyEntry.Initialize(
            GameMode.Normal,
            "魔の森の奥地（エクストリーム）",
            "敵が非常に強く、出現間隔も極端に短くなる難易度。最強の挑戦者向け。",
            "GameScene",
            () => {
                Debug.Log("Extreme Mode Selected");
                spawnScheduleSO.SetDifficultyLevel(DifficultyLevel.Extreme);
                UnityEngine.SceneManagement.SceneManager.LoadScene(extremeDifficultyEntry.SceneName);
            }
        );

        gameModeEntries.Add(easyDifficultyEntry);
        gameModeEntries.Add(normalDifficultyEntry);
        gameModeEntries.Add(hardDifficultyEntry);
        gameModeEntries.Add(extremeDifficultyEntry);

        // UI生成
        foreach(var entry in gameModeEntries)
        {
            var element = temp_GameModeEntry.Instantiate();
            Label title = element.Q<Label>("title-value");
            Label description = element.Q<Label>("description-value");
            Button selectButton = element.Q<Button>();

            title.text = entry.DisplayName;
            description.text = entry.Description;
            selectButton.clicked += () => entry.OnClick?.Invoke();

            gameModeEntryContainer.Add(element);
        }
    }
}