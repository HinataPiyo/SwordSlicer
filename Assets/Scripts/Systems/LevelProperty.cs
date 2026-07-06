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
        // 旧セーブデータ互換: ReleaseLevel を保持していない場合は CurrentLevel と同値として復元する。
        LoadLevels(level, level);
    }

    /// <summary>
    /// ロードされたレベルを使用して、現在の LevelProperty を更新します。
    /// </summary>
    public void LoadLevels(int currentLevel, int releaseLevel)
    {
        int clampedCurrent = Mathf.Clamp(currentLevel, 0, MaxLevelIndex);
        int clampedReleaseRaw = Mathf.Clamp(releaseLevel, 0, MaxLevelIndex);
        int clampedRelease = Mathf.Max(clampedReleaseRaw, clampedCurrent);

        // 範囲外値のみ警告する。release < current の補正は旧セーブ互換として通常ケース。
        bool hadOutOfRangeValue = currentLevel != clampedCurrent || releaseLevel != clampedReleaseRaw;
        if (hadOutOfRangeValue)
        {
            Debug.LogWarning($"Loaded level data was clamped for {upgradeType}. Current={currentLevel}, Release={releaseLevel}, MaxIndex={MaxLevelIndex}.");
        }

        CurrentLevel = clampedCurrent;
        ReleaseLevel = clampedRelease;
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