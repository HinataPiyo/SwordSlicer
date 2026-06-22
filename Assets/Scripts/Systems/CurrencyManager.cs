public static class CurrencyManager
{
    const int MAX_CURRENCY = 9999999;     // 通貨の上限
    public static int Currency { get; private set; } = 100;        // 通貨

    /// <summary>
    /// 通貨を追加する処理
    /// </summary>
    public static void AddCurrency(int amount)
    {
        // 通貨の上限を超えないようにする
        if(CheckMaxCurrency(amount))
        {
            Currency += amount;
            return;
        }

        Currency = MAX_CURRENCY;
    }

    /// <summary>
    /// 通貨を消費する処理
    /// </summary>
    public static bool SpendCurrency(int amount)
    {
        if (Currency >= amount)
        {
            Currency -= amount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 通貨を追加したときに、上限を超えないかチェックする処理
    /// </summary>
    static bool CheckMaxCurrency(int amount)
    {
        return Currency + amount <= MAX_CURRENCY;
    }
}