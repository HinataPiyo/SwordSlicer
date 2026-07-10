using UnityEngine;

public class SaveAndLoadService : MonoBehaviour, ISave, ILoad
{
    public void Save()
    {
        SaveData saveData = new SaveData()
        {
            levelProperty = BattleSettingConfig.GetLevelPropertiesSnapshot(),
            currencyAmount = CurrencyManager.Currency
        };

        string json = JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
    }

    public bool HaSaveData()
    {
        return PlayerPrefs.HasKey("SaveData");
    }

    /// <summary>
    /// ゲームを一度でもプレイしたかどうかを判定する
    /// </summary>
    public bool HasPlayedOnce()
    {
        return PlayerPrefs.GetInt(GameManager.HAS_PLAYED_ONCE_KEY, 0) == 1;
    }

    public void Load()
    {
        if(HaSaveData())
        {
            string json = PlayerPrefs.GetString("SaveData");

            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            BattleSettingConfig.LoadLevelProperties(saveData.levelProperty);
            CurrencyManager.LoadCurrency(saveData.currencyAmount);
            OnLoad?.Invoke();
        }
    }

    public event System.Action OnLoad;
}

[System.Serializable]
public class SaveData
{
    public LevelProperty[] levelProperty;
    public int currencyAmount;
}