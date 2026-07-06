using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ShowPanelNavigationControllerBase : MonoBehaviour, IPanelNavigationController
{
    readonly Stack<IShowPanel> showPanelStack = new Stack<IShowPanel>();
    protected UIDocument UiDoc { get; private set; }

    /// <summary>
    /// UIDocumentを初期化する。Start()で呼び出すこと。
    /// </summary>
    protected void InitializeDocument()
    {
        UiDoc = GetComponent<UIDocument>();
        if(UiDoc == null)
        {
            Debug.LogError($"{GetType().Name}: UIDocument component was not found.");
        }
    }

    /// <summary>
    /// 指定された名前のモジュールを検索する。UIDocumentが初期化されていない場合はnullを返す。
    /// </summary>
    /// <param name="name">検索するモジュールの名前</param>
    protected VisualElement SearchModule(string name)
    {
        if(UiDoc == null)
        {
            Debug.LogError($"{GetType().Name}: UiDoc is null. Call InitializeDocument() first.");
            return null;
        }

        var element = UiDoc.rootVisualElement.Q(name);
        if(element == null)
        {
            Debug.LogError($"{GetType().Name}: Failed to find module with name '{name}'.");
        }
        return element;
    }

    /// <summary>
    /// ShowPanelControllerのBindBackButtonを呼び出して、ボタンが押されたときに現在のパネルを非表示にする。
    /// </summary>
    /// <param name="button">戻るボタン</param>
    public void BindBackButton(Button button)
    {
        if(button == null)
            return;

        button.clicked += HidePanel;
    }

    /// <summary>
    /// ShowPanelControllerのBindNextButtonを呼び出して、ボタンが押されたときに指定されたパネルを表示する。
    /// </summary>
    /// <param name="button">次へボタン</param>
    /// <param name="nextPanel">表示するパネル</param>
    public void BindNextButton(Button button, IShowPanel nextPanel)
    {
        if(button == null || nextPanel == null)
            return;

        button.clicked += () => ShowPanel(nextPanel);
    }

    /// <summary>
    /// 指定されたパネルを表示する。すでに表示されている場合は何もしない。
    /// </summary>
    /// <param name="panel">表示するパネル</param>
    protected void ShowPanel(IShowPanel panel)
    {
        if(panel == null)
        {
            Debug.LogWarning($"{GetType().Name}: panel is null.");
            return;
        }

        if(showPanelStack.Count > 0 && showPanelStack.Peek() == panel)
            return;

        if(showPanelStack.Count > 0)
        {
            showPanelStack.Peek().Root.style.display = DisplayStyle.None;
        }

        panel.Root.style.display = DisplayStyle.Flex;
        showPanelStack.Push(panel);

        ServiceLocator.Get<IAudioService>().PlaySE("NormalButton");
    }

    /// <summary>
    /// 現在表示されているパネルを非表示にする。すでに非表示の場合は何もしない。
    /// </summary>
    protected void HidePanel()
    {
        if(showPanelStack.Count == 0)
        {
            Debug.LogWarning($"{GetType().Name}: No panel to hide.");
            return;
        }

        showPanelStack.Pop().Root.style.display = DisplayStyle.None;

        if(showPanelStack.Count > 0)
        {
            showPanelStack.Peek().Root.style.display = DisplayStyle.Flex;
        }

        ServiceLocator.Get<IAudioService>().PlaySE("BackButton");
    }
}
