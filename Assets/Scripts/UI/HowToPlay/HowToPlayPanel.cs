using UnityEngine;
using UnityEngine.UIElements;

public class HowToPlayPanel : MonoBehaviour
{
    UIDocument uiDoc;
    [SerializeField] HowToPlayPanelModel model;
    Button nextOrCloseButton;
    int currentStep = 0;

    void OnEnable()
    {
        InitializeUI();
    }

    void OnDisable()
    {
        // ボタンのクリックイベントを解除して、重複登録を防ぐ
        if(nextOrCloseButton != null)
        {
            nextOrCloseButton.clicked -= NextStep;
            nextOrCloseButton = null;
        }
    }

    void Awake()
    {
        bool hasSaveData = ServiceLocator.Get<ISave>().HaSaveData();

        // ゲームを一度もプレイしていない場合はHowToPlayPanelを表示する
        bool hasPlayedOnce = ServiceLocator.Get<ISave>().HasPlayedOnce();

        InitializeUI();

        // セーブデータが存在しない場合かつゲームを一度もプレイしていない場合はHowToPlayPanelを表示する
        if(!hasSaveData && !hasPlayedOnce)
        {
            OpenPanel();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void InitializeUI()
    {
        uiDoc = GetComponent<UIDocument>();
        nextOrCloseButton = uiDoc.rootVisualElement.Q<VisualElement>("NextOrCloseButton").Q<Button>();

        nextOrCloseButton.clicked -= NextStep;
        nextOrCloseButton.clicked += NextStep;
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        InitializeUI();
        currentStep = 0;
        nextOrCloseButton.text = "次へ";
        SetHowToPlayImage(model.howToPlayImages[currentStep]);
    }

    /// <summary>
    /// HowToPlayPanelModelの画像をUIに反映させる
    /// </summary>
    void SetHowToPlayImage(Sprite image)
    {
        model.howToPlayImage = image;
        model.step = "ステップ - " + (currentStep + 1).ToString();
    }

    /// <summary>
    /// 次のステップに進む、または閉じるボタンを押したときの処理
    /// </summary>
    void NextStep()
    {
        if(currentStep < model.howToPlayImages.Length - 1)
        {
            currentStep++;
            SetHowToPlayImage(model.howToPlayImages[currentStep]);
            nextOrCloseButton.text = "次へ";
            if(currentStep == model.howToPlayImages.Length - 1)
            {
                nextOrCloseButton.text = "閉じる";
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}