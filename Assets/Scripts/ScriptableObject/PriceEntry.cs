using UnityEngine;

[System.Serializable]
public class PriceEntry
{
    public UpgradeType upgradeType;
    public int def_Price;
    [Range(1f, 5f)] public float multiply;

    public PriceEntry(UpgradeType upgradeType, int def_Price, float multiply)
    {
        this.upgradeType = upgradeType;
        this.def_Price = def_Price;
        this.multiply = multiply;
    }

    public int GetPrice(int currentLevel)
    {
        return Mathf.RoundToInt(def_Price * Mathf.Pow(multiply, currentLevel));
    }
}
