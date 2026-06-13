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

    void Update()
    {
        // 現在の剣が次に取ることができる状態かどうかを確認して、ボタンのインタラクト可能状態を更新する
        CheckTakeButtonInteractable();
    }

    /// <summary>
    /// 剣を取るボタンのインタラクト可能状態を更新する
    /// </summary>
    void CheckTakeButtonInteractable()
    {
        // 現在の剣が存在しない場合は、ストックがあるかどうかでボタンの状態を決める
        if(swordSpawnCtrl.CurrentSword == null)
        {
            take_button.SetEnabled(swordSpawnCtrl.HasStock);     // 現在の剣が存在しない場合は、ストックがあるかどうかでボタンの状態を決める
        }
        else
        {
            // 現在の剣が次に取ることができる状態かどうかを確認して、ボタンのインタラクト可能状態を更新する
            if(swordSpawnCtrl.CurrentSword.IsNextTakeSword())
            {
                take_button.SetEnabled(true);
                swordSpawnCtrl.RemoveCurrentSword();     // 現在の剣を削除する
                return;
            }

            take_button.SetEnabled(false);
        }
    }
}