using UnityEngine;
using UnityEngine.UIElements;

public class TakeSwordButton : MonoBehaviour
{
    UIDocument uiDocs;

    Button take_button;

    [SerializeField] SwordSpawnController swordSpawnCtrl;

    void Awake()
    {
        uiDocs = GetComponent<UIDocument>();
        take_button = uiDocs.rootVisualElement.Q<VisualElement>("sword-take-button").Q<Button>();

        take_button.clicked += () => swordSpawnCtrl.SpawnSorwd();
    }
}