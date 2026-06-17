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
            bool hasStock = swordSpawnCtrl.HasStock;
            take_button.SetEnabled(hasStock);     // 現在の剣が存在しない場合は、ストックがあるかどうかでボタンの状態を決める
            take_button.pickingMode = hasStock ? PickingMode.Position : PickingMode.Ignore;    // ストックがある場合はピッキングモードを有効にする、ない場合は無効にする
        }
        else
        {
            // 現在の剣が次に取ることができる状態かどうかを確認して、ボタンのインタラクト可能状態を更新する
            if(swordSpawnCtrl.CurrentSword.IsNextTakeSword())
            {
                take_button.SetEnabled(true);
                take_button.pickingMode = PickingMode.Position;    // ボタンのピッキングモードを有効にする
                swordSpawnCtrl.RemoveCurrentSword();     // 現在の剣を削除する
                return;
            }

            take_button.pickingMode = PickingMode.Ignore;    // ボタンのピッキングモードを無効にする
            take_button.SetEnabled(false);
        }
    }
}