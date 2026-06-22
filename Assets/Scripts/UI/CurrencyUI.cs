using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CurrencyUI : MonoBehaviour
{
    UIDocument uiDoc;

    Label currencyText;
    VisualElement curerncyContainer;

    [SerializeField] float showAnimationDuration = 0.5f;

    Coroutine updateCoroutine;
    WaitForSeconds showDuration;
    
    private int displayedCurrency = 0;  // 現在表示している通貨値
    

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        currencyText = uiDoc.rootVisualElement.Q<Label>("currency-value");
        curerncyContainer = uiDoc.rootVisualElement.Q<VisualElement>("currency-container");
        showDuration = new WaitForSeconds(showAnimationDuration);
        
        displayedCurrency = CurrencyManager.Currency;

        Debug.Log($"curerncyContainer: {curerncyContainer}"); 
    }

    void OnEnable()
    {
        displayedCurrency = CurrencyManager.Currency;
        currencyText.text = displayedCurrency.ToString("#,###");
    }
    

    /// <summary>
    /// 通貨の値を更新する処理
    /// </summary>
    public void UpdateCurrency(int currency)
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
        
        updateCoroutine = StartCoroutine(AnimateCurrency(displayedCurrency, currency));
    }

    /// <summary>
    ///  通貨を獲得したときのアニメーション処理
    /// </summary>
    IEnumerator AnimateCurrency(int from, int to)
    {
        curerncyContainer.AddToClassList("currency-container-get");

        int current = from;
        int direction = to > from ? 1 : -1;         // 増加する場合は1、減少する場合は-1
        int difference = Mathf.Abs(to - from);      // 変化量
        
        // 値を1ずつ変化させる
        for (int i = 0; i < difference; i++)
        {
            current += direction;
            displayedCurrency = current;
            currencyText.text = displayedCurrency.ToString("#,###");
            yield return null;  // 次のフレームまで待機
        }

        // 最終値を確実に設定
        displayedCurrency = to;
        currencyText.text = displayedCurrency.ToString("#,###");
        
        yield return showDuration;

        curerncyContainer.RemoveFromClassList("currency-container-get");

        updateCoroutine = null;
    }
}