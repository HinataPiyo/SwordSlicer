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
        CurrencyManager.OnCurrencyChanged += UpdateCurrency;   // 通貨の値が変化したときに、UIを更新する処理を登録する
    }

    void Start()
    {
        CurrencyManager.AddCurrency(100);
    }

    void OnEnable()
    {
        displayedCurrency = CurrencyManager.Currency;
        currencyText.text = displayedCurrency.ToString("#,###");
    }

    void OnDisable()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }

    void OnDestroy()
    {
        CurrencyManager.OnCurrencyChanged -= UpdateCurrency;
    }
    

    /// <summary>
    /// 通貨の値を更新する処理
    /// </summary>
    public void UpdateCurrency(int currency)
    {
        if (!this || !isActiveAndEnabled)
        {
            return;
        }

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
        bool isAdding = IsAddingCurrency(from, to);

        curerncyContainer.AddToClassList(isAdding ? "currency-container-get" : "currency-container-spend");

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

        curerncyContainer.RemoveFromClassList(isAdding ? "currency-container-get" : "currency-container-spend");

        updateCoroutine = null;
    }

    bool IsAddingCurrency(int from, int to)
    {
        return to > from;
    }
}