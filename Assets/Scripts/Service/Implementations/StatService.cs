using System.Collections.Generic;
using UnityEngine;


public enum UpgradeType
{
    SwordStrength,
    SwordThrowForce,
    SwordTurnForce,
    SwordTurnReactTime,
    SwordAttackRange,
    CriticalRate,
    CriticalDamageMultiplier,
    SwordCreateInterval,
    SwordStock
}

public class StatService : MonoBehaviour, IStateService
{
    [Header("設定")]
    [SerializeField] BattleSettingConfig config;
    public BattleSettingConfig.SwordDataByType CreateSword()
    {
        // 剣のデータを確率に応じてランダムに選択する
        float rand = Random.value;
        float cumulativeProbability = 0f;
        foreach (var data in config.SwordDatas)
        {
            cumulativeProbability += data.createProbability;
            if (rand < cumulativeProbability)
            {
                return data;
            }
        }
        // もし確率の合計が1未満の場合、最後の剣のデータを返す
        return config.SwordDatas[config.SwordDatas.Length - 1];
    }

    public static int Level(UpgradeType upgradeType)
    {
        foreach (var levelProperty in BattleSettingConfig.LevelProperties)
        {
            if (levelProperty.UpgradeType == upgradeType)
            {
                return levelProperty.CurrentLevel;
            }
        }
        Debug.LogError("指定されたUpgradeTypeに対応するLevelPropertyが見つかりませんでした: " + upgradeType);
        return 0;   // デフォルトでレベル0を返す
    }

    /// <summary>
    /// 剣の攻撃力を計算して返す
    /// クリティカルヒットの場合は攻撃力にクリティカルダメージ倍率を掛ける
    /// クリティカルかどうかは、剣のデータに設定されたクリティカル率を元にランダムに判定する
    /// 例えば、クリティカル率が0.1の場合、10%の確率でクリティカルヒットになる
    /// </summary>
    /// <param name="swordData"></param>
    /// <returns></returns>
    public float DamageAmount(BattleSettingConfig.SwordDataByType data, out bool isCritical)
    {
        float baseStrength = config.SwordStrength(Level(UpgradeType.SwordStrength)) * data.swordDataSO.SwordStrengthMultiply();
        isCritical = Random.value < config.CriticalRate(Level(UpgradeType.CriticalRate));     // クリティカルかどうかをランダムに判定
        if (isCritical)
        {
            return baseStrength * config.CriticalDamageMultiplier(Level(UpgradeType.CriticalDamageMultiplier));     // クリティカルの場合
        }

        return baseStrength;     // 通常の場合
    }

    // 投げる強さ
    public float SwordThrowForce() => config.SwordThrowForce(Level(UpgradeType.SwordThrowForce));

    // 曲がる強さ
    public float SwordTurnForce() => config.SwordTurnForce(Level(UpgradeType.SwordTurnForce));

    // 曲がる力の反応時間
    public float SwordTurnReactTime() => config.SwordTurnReactTime(Level(UpgradeType.SwordTurnReactTime));


    // 攻撃力
    public float SwordAttackRange() => config.SwordAttackRange(Level(UpgradeType.SwordAttackRange));

    // 攻撃速度
    public float SwordAttackInterval(float rotateAmount) => config.SwordAttackInterval(rotateAmount);

    // 攻撃範囲
    public float StockInterval() => config.SwordCreateInterval(Level(UpgradeType.SwordCreateInterval));

    // 現在のストックの最大数
    public int CurrentMaxStock() => config.CurrentMaxStock(Level(UpgradeType.SwordStock));

    // 剣の回転力/攻撃速度
    public float MaxRotateAmount() => config.MaxRotationAmount() * SwordTurnForce();

    public BattleSettingConfig.SwordDataByType[] SwordDatas() => config.SwordDatas;

    public List<UpgradeEntry> UpgradeEntries()
    {
        List<UpgradeEntry> entries = new List<UpgradeEntry>();

        foreach (var levelProperty in BattleSettingConfig.LevelProperties)
        {
            UpgradeEntry entry = new UpgradeEntry
            {
                statName = GetUpgradeNameByType(levelProperty.UpgradeType),
                currentValue = () => GetCurrentValueByType(levelProperty.UpgradeType).ToString(),
                levelProperty = levelProperty,
                price = () => BattleSettingConfig.GetPriceEntry(levelProperty.UpgradeType).GetPrice(levelProperty.ReleaseLevel)      // 解放段階に応じて値段変更
            };
            entries.Add(entry);
        }

        return entries;
    }

    string GetUpgradeNameByType(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.SwordStrength:
                return "攻撃力";
            case UpgradeType.SwordThrowForce:
                return "投げる力";
            case UpgradeType.SwordTurnForce:
                return "曲がる力";
            case UpgradeType.SwordTurnReactTime:
                return "曲がる反応時間";
            case UpgradeType.SwordAttackRange:
                return "攻撃範囲";
            case UpgradeType.CriticalRate:
                return "クリティカル率";
            case UpgradeType.CriticalDamageMultiplier:
                return "クリティカルダメージ倍率";
            case UpgradeType.SwordCreateInterval:
                return "鍛造間隔";
            case UpgradeType.SwordStock:
                return "ストック数";
            default:
                return "Unknown";
        }
    }

    /// <summary>
    /// UpgradeTypeに応じて現在の値を取得する
    /// 例えば、SwordStrengthの場合は、現在の攻撃力を計算して返す
    /// 値の形式は、攻撃力なら小数点1位まで、クリティカル率ならパーセント表示など、見やすい形式にする
    /// ここで返した値はUpgradePanelUIで表示される
    /// </summary>
    object GetCurrentValueByType(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.SwordStrength:
                return config.SwordStrength(Level(UpgradeType.SwordStrength)).ToString("F1");
            case UpgradeType.SwordThrowForce:
                return config.SwordThrowForce(Level(UpgradeType.SwordThrowForce));
            case UpgradeType.SwordTurnForce:
                return config.SwordTurnForce(Level(UpgradeType.SwordTurnForce));
            case UpgradeType.SwordTurnReactTime:
                return config.SwordTurnReactTime(Level(UpgradeType.SwordTurnReactTime)) + "s";
            case UpgradeType.SwordAttackRange:
                return config.SwordAttackRange(Level(UpgradeType.SwordAttackRange)) + "m";
            case UpgradeType.CriticalRate:
                return (config.CriticalRate(Level(UpgradeType.CriticalRate)) * 100) + "%";
            case UpgradeType.CriticalDamageMultiplier:
                return config.CriticalDamageMultiplier(Level(UpgradeType.CriticalDamageMultiplier)) * 100 + "%";
            case UpgradeType.SwordCreateInterval:
                return config.SwordCreateInterval(Level(UpgradeType.SwordCreateInterval)) + "s";
            case UpgradeType.SwordStock:
                return config.CurrentMaxStock(Level(UpgradeType.SwordStock)) + "枠";
            default:
                return null;
        }
    }

}