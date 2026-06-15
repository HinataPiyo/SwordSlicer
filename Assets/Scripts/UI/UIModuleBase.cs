using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIModuleBase : MonoBehaviour, IShowPanel
{
    public VisualElement Root { get; private set; }

    public void SetModule(VisualElement moduleRoot)
    {
        Root = moduleRoot;
        Initialize();
    }

    protected abstract void Initialize();
    public abstract void BindNavigation(ShowPanelController controller);
}