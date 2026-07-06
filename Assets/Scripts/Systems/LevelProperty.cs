using UnityEngine;

[System.Serializable]
public class LevelProperty
{
    [SerializeField] UpgradeType upgradeType;   public UpgradeType UpgradeType => upgradeType;
    [field: SerializeField, ReadOnly] public int CurrentLevel { get; private set; } = 0;
    [field: SerializeField, ReadOnly] public int ReleaseLevel { get; private set; } = 0;        // 現在解放されているレベル
    [SerializeField] int maxLevel = 8;  public int MaxLevel => maxLevel;

    // レベルは 0 始まりで扱うため、実際に到達可能な最大インデックスは maxLevel - 1。
    int MaxLevelIndex => Mathf.Max(0, maxLevel - 1);
    public bool IsReleaseMax() => ReleaseLevel >= MaxLevelIndex;

    public void LoadLevel(int level)
    {
        if (level >= 0 && level <= MaxLevelIndex)
        {
            CurrentLevel = level;
        }
        else
        {
            Debug.LogWarning($"Invalid level {level} for {upgradeType}. Level must be between 0 and {MaxLevelIndex}.");
        }
    }

    public LevelProperty(UpgradeType upgradeType, int maxLevel)
    {
        this.upgradeType = upgradeType;
        this.maxLevel = maxLevel;
    }

    public void ReleaseUp()
    {
        if (ReleaseLevel < MaxLevelIndex)
        {
            ReleaseLevel++;
        }
        else
        {
            Debug.Log("最大レベルに達しています");
        }
    }

    public void LevelUp()
    {
        if (CurrentLevel < ReleaseLevel && CurrentLevel < MaxLevelIndex)
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
        // レベルダウンは現在のレベルが1以上の場合にのみ許可する（レベル0が最小）
        if (CurrentLevel > 0)
        {
            CurrentLevel--;
        }
        else
        {
            Debug.Log("最小レベルに達しています");
        }
    }
}