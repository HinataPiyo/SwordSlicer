using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectGameModePanel : UIModuleBase
{
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
        foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
        {
            var entry = GameModeSetting.GetGameModeEntry(difficulty, spawnScheduleSO);
            gameModeEntries.Add(entry);
        }

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

public static class GameModeSetting
{
    public static GameModeEntry GetGameModeEntry(DifficultyLevel difficulty, EnemySpawnScheduleSO spawnScheduleSO)
    {
        GameModeEntry entry = new GameModeEntry();

        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                entry.Initialize(
                    GameMode.Normal,
                    $"{GetDifficultyTitle(DifficultyLevel.Easy)}（イージー）",
                    "敵が弱く、出現間隔も長い難易度。初心者向け。",
                    "GameScene",
                    () => {
                        Debug.Log("Easy Mode Selected");
                        spawnScheduleSO.SetDifficultyLevel(DifficultyLevel.Easy);
                        UnityEngine.SceneManagement.SceneManager.LoadScene(entry.SceneName);
                    });
                break;
            case DifficultyLevel.Normal:
                entry.Initialize(
                    GameMode.Normal,
                    $"{GetDifficultyTitle(DifficultyLevel.Normal)}（ノーマル）",
                    "通常の難易度。徐々に強くなる敵を倒し続けるサバイバルモード。",
                    "GameScene",
                    () => {
                        Debug.Log("Normal Mode Selected");
                        spawnScheduleSO.SetDifficultyLevel(DifficultyLevel.Normal);
                        UnityEngine.SceneManagement.SceneManager.LoadScene(entry.SceneName);
                    });
                break;
            case DifficultyLevel.Hard:
                entry.Initialize(
                    GameMode.Normal,
                    $"{GetDifficultyTitle(DifficultyLevel.Hard)}（ハード）",
                    "敵がより強く、出現間隔も短くなる難易度。中級者向け。",
                    "GameScene",
                    () => {
                        Debug.Log("Hard Mode Selected");
                        spawnScheduleSO.SetDifficultyLevel(DifficultyLevel.Hard);
                        UnityEngine.SceneManagement.SceneManager.LoadScene(entry.SceneName);
                    });
                break;
            case DifficultyLevel.Extreme:
                entry.Initialize(
                    GameMode.Normal,
                    $"{GetDifficultyTitle(DifficultyLevel.Extreme)}（エクストリーム）",
                    "敵が非常に強く、出現間隔がより短くなる難易度。上級者向け。",
                    "GameScene",
                    () => {
                        Debug.Log("Extreme Mode Selected");
                        spawnScheduleSO.SetDifficultyLevel(DifficultyLevel.Extreme);
                        UnityEngine.SceneManagement.SceneManager.LoadScene(entry.SceneName);
                    });
                break;
            case DifficultyLevel.Nightmare:
                entry.Initialize(
                    GameMode.Normal,
                    $"{GetDifficultyTitle(DifficultyLevel.Nightmare)}（ナイトメア）",
                    "敵が最強で、出現間隔も最短になる難易度。究極の挑戦者向け。",
                    "GameScene",
                    () => {
                        Debug.Log("Nightmare Mode Selected");
                        spawnScheduleSO.SetDifficultyLevel(DifficultyLevel.Nightmare);
                        UnityEngine.SceneManagement.SceneManager.LoadScene(entry.SceneName);
                    });
                break;
        }

        return entry;
    }

    public static string GetDifficultyTitle(DifficultyLevel difficulty)
    {
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                return "魔の森から離れた平原";
            case DifficultyLevel.Normal:
                return "魔の森の入口";
            case DifficultyLevel.Hard:
                return "魔の森の中枢";
            case DifficultyLevel.Extreme:
                return "魔の森の奥地";
            case DifficultyLevel.Nightmare:
                return "魔の森の最深部";
            default:
                return "不明な難易度";
        }
    }
}