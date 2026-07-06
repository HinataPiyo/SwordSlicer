using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ResultService : MonoBehaviour, IResultService
{
    [SerializeField] BattleSettingConfig battleSettingConfig;
    [SerializeField] EnemySpawnController enemySpawnCTRL;

    public class Entry
    {
        public class SwordData
        {
            public SwordType type;
            public int createCount { get; private set; } = 0;

            public void AddCreateCount() => createCount++;
        }

        public float defenseTime { get; private set; }
        public int enemyKillCount { get; private set; }
        public int maxHitCount { get; private set; }
        public int totalGetCurrency { get; private set; }
        public List<SwordData> swordDatas { get; private set; } = new List<SwordData>()
        {
            new SwordData() { type = SwordType.Normal },
            new SwordData() { type = SwordType.Advanced },
            new SwordData() { type = SwordType.Ultimate },
        };

        public void SetDefenseTime(float time) => defenseTime = time;
        public void AddEnemyKillCount() => enemyKillCount++;
        public void SetMaxHitCount(int count)
        {
            // セットされたカウントが現在の最大ヒット数より大きい場合のみ更新する
            if(count > maxHitCount)
                maxHitCount = count;
        }

        public void AddGetCurrency(int currency) => totalGetCurrency += currency;

        public void AddSwordCreateCount(SwordType type)
        {
            var swordData = swordDatas.Find(s => s.type == type);
            if(swordData != null)
                swordData.AddCreateCount();
        }
    }

    public Entry Data { get; private set; } = new Entry();

    UIDocument uiDoc;
    VisualElement root;

    Label difficultyText;
    Label difficultyTitle;
    Label defanseTimeText;
    Label enemyKillCountText;
    Label maxHitCountText;
    Label totalGetCurrencyText;
    List<SwordIconUI> swordIcons = new List<SwordIconUI>();

    Button homeBackButton;
    Button retryButton;


    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement.Q<VisualElement>("ResultPanel");

        root.style.display = DisplayStyle.None;
        root.AddToClassList("result-panel__hide");
        GameManager.OnGameOver += () => ShowResult();
    }

    void OnEnable()
    {
        var r = uiDoc.rootVisualElement;
        difficultyTitle = r.Q<VisualElement>("stage-title-container").Q<Label>("difficulty-title");
        difficultyText = r.Q<VisualElement>("stage-title-container").Q<Label>("difficulty-text");
        defanseTimeText = r.Q<VisualElement>("defense-time-container").Q<Label>("time-value");
        enemyKillCountText = r.Q<VisualElement>("enemy-kill-count-container").Q<Label>("count-value");
        maxHitCountText = r.Q<VisualElement>("max-hit-count-container").Q<Label>("count-value");
        totalGetCurrencyText = r.Q<VisualElement>("get-currency-amount-container").Q<Label>("value");
        homeBackButton = r.Q<VisualElement>("home-back-button").Q<Button>();
        retryButton = r.Q<VisualElement>("retry-button").Q<Button>();
        var icons = r.Q<VisualElement>("sword-create-count-container").Query<VisualElement>("upgrade-sword-icon-root").ToList();
        var swordDatas = ServiceLocator.Get<IStateService>().SwordDatas();

        for(int i = 0; i < icons.Count; i++)
        {
            if(i >= swordDatas.Length)
                continue;

            int index = i;  // クロージャのためにローカル変数に格納する
            SwordIconUI swordIconUI = new SwordIconUI();
            swordIconUI.Initialize(icons[index]);

            swordIcons.Add(swordIconUI);
        }

        homeBackButton.clicked += HomeBackButtonClicked;
        retryButton.clicked += RetryButtonClicked;
    }

    public void ShowResult()
    {
        root.style.display = DisplayStyle.Flex;
        root.RemoveFromClassList("result-panel__hide");
        root.AddToClassList("result-panel__show");
        
        difficultyTitle.text = GameModeSetting.GetDifficultyTitle(enemySpawnCTRL.GetDifficultyLevel());
        difficultyText.text = enemySpawnCTRL.GetDifficultyLevelText();
        defanseTimeText.text = Data.defenseTime.ToString("F1") + "秒";
        enemyKillCountText.text = Data.enemyKillCount.ToString() + "体";
        maxHitCountText.text = Data.maxHitCount.ToString() + "Hit";
        totalGetCurrencyText.text = "￥" + Data.totalGetCurrency.ToString("#,##0");

        for(int i = 0; i < swordIcons.Count; i++)
        {
            if(i >= Data.swordDatas.Count)
                continue;

            Sprite icon = battleSettingConfig.GetSwordIcon(Data.swordDatas[i].type);
            swordIcons[i].SetCount(icon, Data.swordDatas[i].createCount);
        }
    }

    /// <summary>
    /// ホームボタンが押されたときの処理
    /// </summary>
    void HomeBackButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// リトライボタンが押されたときの処理
    /// </summary>
    void RetryButtonClicked()
    {
        SceneManager.LoadScene(1);
    }


}