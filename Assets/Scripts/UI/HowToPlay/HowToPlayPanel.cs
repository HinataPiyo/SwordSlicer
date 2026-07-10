using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HowToPlayPanel : MonoBehaviour
{
    UIDocument uiDoc;
    [SerializeField] HowToPlayPanelModel model;
    Button nextOrCloseButton;
    int currentStep = 0;

    void Awake()
    {
        bool hasSaveData = ServiceLocator.Get<ISave>().HaSaveData();
        uiDoc = GetComponent<UIDocument>();
        nextOrCloseButton = uiDoc.rootVisualElement.Q<VisualElement>("NextOrCloseButton").Q<Button>();
        nextOrCloseButton.clicked += NextStep;

        // ゲームを一度もプレイしていない場合はHowToPlayPanelを表示する
        bool hasPlayedOnce = ServiceLocator.Get<ISave>().HasPlayedOnce();

        // セーブデータが存在しない場合かつゲームを一度もプレイしていない場合はHowToPlayPanelを表示する
        if(!hasSaveData && !hasPlayedOnce)
        {
            gameObject.SetActive(true);
            SetHowToPlayImage(model.howToPlayImages[currentStep]);
        }
        else
        {
            gameObject.SetActive(false);
        }
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