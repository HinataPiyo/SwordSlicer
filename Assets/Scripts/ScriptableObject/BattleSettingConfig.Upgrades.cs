using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleSettingConfig
{
    static readonly PriceEntry[] priceEntries = new PriceEntry[]
    {
        new PriceEntry(UpgradeType.SwordStrength, 200, 1.2f),
        new PriceEntry(UpgradeType.SwordCreateInterval, 800, 1.3f),
        new PriceEntry(UpgradeType.CriticalRate, 1000, 1.2f),
        new PriceEntry(UpgradeType.CriticalDamageMultiplier, 700, 1.2f),
        new PriceEntry(UpgradeType.SwordStock, 700, 1.5f),
        new PriceEntry(UpgradeType.SwordAttackRange, 1200, 1.8f),
        new PriceEntry(UpgradeType.SwordThrowForce, 500, 1.7f),
        new PriceEntry(UpgradeType.SwordTurnForce, 700, 1.5f),
        new PriceEntry(UpgradeType.SwordTurnReactTime, 700, 1.6f),
    };

    static readonly LevelProperty[] levelProperties = new LevelProperty[]
    {
        new LevelProperty(UpgradeType.SwordStrength, 33),
        new LevelProperty(UpgradeType.SwordCreateInterval, 8),
        new LevelProperty(UpgradeType.CriticalRate, 8),
        new LevelProperty(UpgradeType.CriticalDamageMultiplier, 8),
        new LevelProperty(UpgradeType.SwordStock, 5),
        new LevelProperty(UpgradeType.SwordAttackRange, 5),
        new LevelProperty(UpgradeType.SwordThrowForce, 8),
        new LevelProperty(UpgradeType.SwordTurnForce, 8),
        new LevelProperty(UpgradeType.SwordTurnReactTime, 8),
    };

    public static event Action OnLoadLevelProperties;

    static bool hasValidatedUpgradeDefinitions;
    static bool hasBuiltPriceEntryMap;
    static Dictionary<UpgradeType, PriceEntry> priceEntryMap;

    public static IReadOnlyList<LevelProperty> LevelProperties => levelProperties;

    static void ValidateUpgradeDefinitionsIfNeeded()
    {
        if (hasValidatedUpgradeDefinitions) return;

        hasValidatedUpgradeDefinitions = true;
        ValidateUpgradeDefinitions();
        BuildPriceEntryMap();
    }

    static void BuildPriceEntryMap()
    {
        if (hasBuiltPriceEntryMap) return;

        hasBuiltPriceEntryMap = true;
        priceEntryMap = new Dictionary<UpgradeType, PriceEntry>(priceEntries.Length);
        foreach (var entry in priceEntries)
        {
            if (entry == null) continue;

            if (priceEntryMap.ContainsKey(entry.upgradeType))
            {
                Debug.LogWarning($"PriceEntry辞書作成時に重複UpgradeTypeを検出しました: {entry.upgradeType}. 先頭定義を採用します。");
                continue;
            }

            priceEntryMap.Add(entry.upgradeType, entry);
        }
    }

    static void ValidateUpgradeDefinitions()
    {
        var enumTypes = new HashSet<UpgradeType>((UpgradeType[])Enum.GetValues(typeof(UpgradeType)));
        var priceTypes = new HashSet<UpgradeType>();
        var levelTypes = new HashSet<UpgradeType>();

        foreach (var entry in priceEntries)
        {
            if (entry == null) continue;
            if (!priceTypes.Add(entry.upgradeType))
            {
                Debug.LogError($"PriceEntries に重複した UpgradeType があります: {entry.upgradeType}");
            }
        }

        foreach (var level in levelProperties)
        {
            if (level == null) continue;
            if (!levelTypes.Add(level.UpgradeType))
            {
                Debug.LogError($"LevelProperties に重複した UpgradeType があります: {level.UpgradeType}");
            }
        }

        foreach (var type in enumTypes)
        {
            if (!priceTypes.Contains(type))
            {
                Debug.LogError($"PriceEntries に {type} の定義がありません。");
            }

            if (!levelTypes.Contains(type))
            {
                Debug.LogError($"LevelProperties に {type} の定義がありません。");
            }
        }

        foreach (var type in priceTypes)
        {
            if (!enumTypes.Contains(type))
            {
                Debug.LogError($"PriceEntries に未定義の UpgradeType が含まれています: {type}");
            }
        }

        foreach (var type in levelTypes)
        {
            if (!enumTypes.Contains(type))
            {
                Debug.LogError($"LevelProperties に未定義の UpgradeType が含まれています: {type}");
            }
        }
    }

    public static void LoadLevelProperties(LevelProperty[] loadedLevelProperties)
    {
        ValidateUpgradeDefinitionsIfNeeded();

        if (loadedLevelProperties == null)
        {
            Debug.LogWarning("ロード対象のLevelPropertyがnullです。既存データを維持します。");
            OnLoadLevelProperties?.Invoke();
            return;
        }

        var loadedByType = new Dictionary<UpgradeType, LevelProperty>(loadedLevelProperties.Length);
        foreach (var loaded in loadedLevelProperties)
        {
            if (loaded == null) continue;

            if (loadedByType.ContainsKey(loaded.UpgradeType))
            {
                Debug.LogWarning($"重複したUpgradeTypeを検出しました: {loaded.UpgradeType}. 最初の値を採用します。");
                continue;
            }

            loadedByType.Add(loaded.UpgradeType, loaded);
        }

        foreach (var current in levelProperties)
        {
            if (current == null) continue;

            if (loadedByType.TryGetValue(current.UpgradeType, out var loaded))
            {
                current.LoadLevel(loaded.CurrentLevel);
                continue;
            }

            Debug.LogWarning($"セーブデータに {current.UpgradeType} が存在しないため、既存値を維持します。");
        }

        OnLoadLevelProperties?.Invoke();
    }

    public static bool TryGetPriceEntry(UpgradeType upgradeType, out PriceEntry entry)
    {
        ValidateUpgradeDefinitionsIfNeeded();
        return priceEntryMap.TryGetValue(upgradeType, out entry);
    }

    public static PriceEntry GetPriceEntry(UpgradeType upgradeType)
    {
        if (TryGetPriceEntry(upgradeType, out var entry))
        {
            return entry;
        }

        Debug.LogError("指定されたUpgradeTypeに対応するPriceEntryが見つかりませんでした: " + upgradeType);
        return null;
    }

    public static LevelProperty[] GetLevelPropertiesSnapshot()
    {
        var snapshot = new LevelProperty[levelProperties.Length];
        Array.Copy(levelProperties, snapshot, levelProperties.Length);
        return snapshot;
    }
}
