using UnityEngine;

[System.Serializable]
public class LevelProperty
{
    [SerializeField] UpgradeType upgradeType;   public UpgradeType UpgradeType => upgradeType;
    [field: SerializeField, ReadOnly] public int CurrentLevel { get; private set; } = 1;
    [field: SerializeField, ReadOnly] public int ReleaseLevel { get; private set; } = 1;        // 現在解放されているレベル
    [SerializeField] int maxLevel = 8;  public int MaxLevel => maxLevel;

    public void LevelUp()
    {
        if (CurrentLevel < maxLevel)
        {
            CurrentLevel++;
        }
        else
        {
            Debug.Log("最大レベルに達しています");
        }
    }

    public void LevelDown()
    {
        if (CurrentLevel > 1)
        {
            CurrentLevel--;
        }
        else
        {
            Debug.Log("最小レベルに達しています");
        }
    }
}