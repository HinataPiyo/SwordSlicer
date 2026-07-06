using UnityEngine;
using UnityEngine.UIElements;

public class SaveAndLoadPanel : UIModuleBase
{
    Label body;
    Button saveButton;
    Button loadButton;
    string _saveButtonOriginalText;
    bool _isConfirmingOverwrite;

    protected override void Initialize()
    {
        body = Root.Q<VisualElement>("body").Q<Label>();
        saveButton = Root.Q<VisualElement>("SaveButton").Q<Button>();
        loadButton = Root.Q<VisualElement>("LoadButton").Q<Button>();

        _saveButtonOriginalText = saveButton.text;

        saveButton.clicked += OnSaveButtonClicked;
        loadButton.clicked += OnLoadButtonClicked;
        SetSaveStatus();
    }

    public override void BindNavigation(IPanelNavigationController controller)
    {
        var backButton = Root.Q<VisualElement>("BackButton").Q<Button>();
        // 戻るボタン押下時に確認状態をリセットする
        backButton.clicked += SetSaveStatus;
        controller.BindBackButton(backButton);

        if(controller is IHomePanelNavigationController homeController)
        {
            // Loadボタンは従来通りSelectButtonPanelへ遷移
            controller.BindNextButton(loadButton, homeController.SelectButtonPanel);
        }
        // Saveボタンは確認フローがあるため自動遷移させない（BackButtonで戻る）
    }

    void OnSaveButtonClicked()
    {
        bool hasSaveData = ServiceLocator.Get<ISave>().HaSaveData();

        if (hasSaveData && !_isConfirmingOverwrite)
        {
            // 上書き確認モードへ移行
            _isConfirmingOverwrite = true;
            body.text = "既存のセーブデータを上書きします。\nよろしいですか？\n（もう一度押すと上書き保存されます）";
            saveButton.text = "上書き保存する";
            loadButton.SetEnabled(false);
        }
        else
        {
            // 新規 or 確認済み → 保存実行
            ServiceLocator.Get<ISave>().Save();
            SetSaveStatus();
        }
    }

    void OnLoadButtonClicked()
    {
        ServiceLocator.Get<ILoad>().Load();
    }

    /// <summary>
    /// セーブデータの有無に応じて、セーブボタンとロードボタンの表示状態を切り替える。
    /// 上書き確認状態もリセットする。
    /// </summary>
    void SetSaveStatus()
    {
        _isConfirmingOverwrite = false;
        saveButton.text = _saveButtonOriginalText;

        bool hasSaveData = ServiceLocator.Get<ISave>().HaSaveData();
        saveButton.SetEnabled(true);
        loadButton.SetEnabled(hasSaveData);
        if (hasSaveData)
        {
            body.text = "セーブデータが存在します。\nロードボタンを押すと、\nセーブデータが読み込まれます。";
        }
        else
        {
            body.text = "セーブデータが存在しません。";
        }
    }
}