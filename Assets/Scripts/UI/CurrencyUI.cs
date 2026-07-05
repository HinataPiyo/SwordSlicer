using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CurrencyUI : MonoBehaviour
{
    const float CurrencyAnimationDuration = 1f;

    UIDocument uiDoc;

    Label currencyText;
    VisualElement curerncyContainer;

    Coroutine updateCoroutine;
    
    private int displayedCurrency = 0;  // 現在表示している通貨値
    

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        currencyText = uiDoc.rootVisualElement.Q<Label>("currency-value");
        curerncyContainer = uiDoc.rootVisualElement.Q<VisualElement>("currency-container");
        
        displayedCurrency = CurrencyManager.Currency;
        CurrencyManager.OnCurrencyChanged += UpdateCurrency;   // 通貨の値が変化したときに、UIを更新する処理を登録する
    }

    void OnEnable()
    {
        displayedCurrency = CurrencyManager.Currency;
        currencyText.text = displayedCurrency.ToString("#,##0");
    }

    void OnDisable()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }

        RemoveCurrencyStateClasses();       // 通貨の状態を示すクラスを削除する

        displayedCurrency = CurrencyManager.Currency;
        if (currencyText != null)
        {
            currencyText.text = displayedCurrency.ToString("#,##0");
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

        if (displayedCurrency == currency)
        {
            displayedCurrency = currency;
            currencyText.text = displayedCurrency.ToString("#,##0");
            return;
        }

        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }

        // 0 への到達は即時反映し、表示取りこぼしを防ぐ
        if (currency == 0)
        {
            displayedCurrency = 0;
            currencyText.text = displayedCurrency.ToString("#,##0");
            RemoveCurrencyStateClasses();
            return;
        }
        
        updateCoroutine = StartCoroutine(AnimateCurrency(displayedCurrency, currency));
    }

    /// <summary>
    ///  通貨を獲得したときのアニメーション処理
    /// </summary>
    IEnumerator AnimateCurrency(int from, int to)
    {
        bool isAdding = IsAddingCurrency(from, to);

        RemoveCurrencyStateClasses();
        curerncyContainer.AddToClassList(isAdding ? "currency-container-get" : "currency-container-spend");

        float elapsed = 0f;
        while (elapsed < CurrencyAnimationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / CurrencyAnimationDuration);
            displayedCurrency = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            currencyText.text = displayedCurrency.ToString("#,##0");
            yield return null;
        }

        // 補間誤差対策として最終値を確実に設定
        displayedCurrency = to;
        currencyText.text = displayedCurrency.ToString("#,##0");

        RemoveCurrencyStateClasses();

        updateCoroutine = null;
    }

    /// <summary>
    /// 通貨の状態を示すクラスを削除する
    /// </summary>
    void RemoveCurrencyStateClasses()
    {
        if (curerncyContainer == null)
        {
            return;
        }

        curerncyContainer.RemoveFromClassList("currency-container-get");
        curerncyContainer.RemoveFromClassList("currency-container-spend");
    }

    bool IsAddingCurrency(int from, int to)
    {
        return to > from;
    }
}