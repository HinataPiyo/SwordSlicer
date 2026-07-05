using UnityEngine;
using UnityEngine.UIElements;

public class SaveAndLoadPanel : UIModuleBase
{
    Label body;
    Button saveButton;
    Button loadButton;

    protected override void Initialize()
    {
        body = Root.Q<VisualElement>("body").Q<Label>();
        saveButton = Root.Q<VisualElement>("SaveButton").Q<Button>();
        loadButton = Root.Q<VisualElement>("LoadButton").Q<Button>();

        saveButton.clicked += OnSaveButtonClicked;
        loadButton.clicked += OnLoadButtonClicked;
        SetSaveStatus();
    }

    public override void BindNavigation(ShowPanelController controller)
    {
        controller.BindBackButton(Root.Q<VisualElement>("BackButton").Q<Button>());
        // Save＆Loadボタンが押されたら、SelectButtonPanelに戻るように設定する
        controller.BindNextButton(saveButton, controller.SelectButtonPanel);
        controller.BindNextButton(loadButton, controller.SelectButtonPanel);
    }

    void OnSaveButtonClicked()
    {
        ServiceLocator.Get<ISave>().Save();
    }

    void OnLoadButtonClicked()
    {
        ServiceLocator.Get<ILoad>().Load();
    }

    /// <summary>
    /// セーブデータの有無に応じて、セーブボタンとロードボタンの表示状態を切り替える。
    /// セーブデータが存在する場合はロードボタンを有効化し、存在しない場合はロードボタンを無効化する。
    /// </summary>
    void SetSaveStatus()
    {
        bool hasSaveData = ServiceLocator.Get<ISave>().HaSaveData();
        saveButton.SetEnabled(true); // セーブボタンは常に有効化する
        loadButton.SetEnabled(hasSaveData); // ロードボタンはセーブデータの有無に応じて有効化/無効化する
        if(hasSaveData)
        {
            body.text = "セーブデータが存在します。\nロードボタンを押すと、\nセーブデータが読み込まれます。";
        }
        else
        {
            body.text = "セーブデータが存在しません。";
        }
    }
    
}