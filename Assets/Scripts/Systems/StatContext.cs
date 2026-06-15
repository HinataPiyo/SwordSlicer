using System.Collections.Generic;
using UnityEngine;


public enum UpgradeType { SwordStrength, SwordThrowForce, SwordTurnForce, SwordTurnReactTime, SwordAttackRange, CriticalRate, CriticalDamageMultiplier }

public class StatContext : MonoBehaviour
{
    public static StatContext I { get; private set; }

    [Header("設定")]
    [SerializeField] BattleSettingConfig config;

    [SerializeField] LevelProperty[] levelProperties;    // 各ステータスのレベルを管理する配列

    public SwordDataSO CreateSword()
    {
        // 剣のデータを確率に応じてランダムに選択する
        float rand = Random.value;
        float cumulativeProbability = 0f;
        foreach(var data in config.SwordDatas)
        {
            cumulativeProbability += data.createProbability;
            if(rand < cumulativeProbability)
            {
                return data.swordDataSO;
            }
        }
        // もし確率の合計が1未満の場合、最後の剣のデータを返す
        return config.SwordDatas[config.SwordDatas.Length - 1].swordDataSO;
    }

    public int GetLevel(UpgradeType upgradeType)
    {
        foreach(var levelProperty in levelProperties)
        {
            if(levelProperty.UpgradeType == upgradeType)
            {
                return levelProperty.CurrentLevel;
            }
        }
        Debug.LogError("指定されたUpgradeTypeに対応するLevelPropertyが見つかりませんでした: " + upgradeType);
        return 1;   // デフォルトでレベル1を返す
    }

    /// <summary>
    /// 剣の攻撃力を計算して返す
    /// クリティカルヒットの場合は攻撃力にクリティカルダメージ倍率を掛ける
    /// クリティカルかどうかは、剣のデータに設定されたクリティカル率を元にランダムに判定する
    /// 例えば、クリティカル率が0.1の場合、10%の確率でクリティカルヒットになる
    /// </summary>
    /// <param name="swordData"></param>
    /// <returns></returns>
    public float GetDamageAmount(SwordDataSO swordData, out bool isCritical)
    {
        float baseStrength = config.SwordStrength(GetLevel(UpgradeType.SwordStrength)) * swordData.SwordStrengthMultiply();
        isCritical = Random.value < config.CriticalRate(GetLevel(UpgradeType.CriticalRate));     // クリティカルかどうかをランダムに判定
        if(isCritical)
        {
            return baseStrength * config.CriticalDamageMultiplier(GetLevel(UpgradeType.CriticalDamageMultiplier));     // クリティカルの場合
        }

        return baseStrength;     // 通常の場合
    }

    // 投げる強さ
    public float SwordThrowForce() => config.SwordThrowForce(GetLevel(UpgradeType.SwordThrowForce));

    // 曲がる強さ
    public float SwordTurnForce() => config.SwordTurnForce(GetLevel(UpgradeType.SwordTurnForce));

    // 曲がる力の反応時間
    public float SwordTurnReactTime() => config.SwordTurnReactTime(GetLevel(UpgradeType.SwordTurnReactTime));

    // 攻撃範囲
    public float GetStockInterval() => config.SwordCreateInterval();
    
    // ストックの最大数
    public int GetCurrentMaxStock() => config.CurrentStock;

    // 攻撃力
    public float SwordAttackRange() => config.SwordAttackRange(GetLevel(UpgradeType.SwordAttackRange));

    // 攻撃速度
    public float SwordAttackInterval(float rotateAmount) => config.SwordAttackInterval(rotateAmount);

    // 剣の回転力/攻撃速度
    public float MaxRotationAmount() => config.MaxRotationAmount();

    void Awake()
    {
        if(I == null)
        {
            DontDestroyOnLoad(gameObject);
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public List<UpgradeEntry> GetUpgradeEntries()
    {
        List<UpgradeEntry> entries = new List<UpgradeEntry>();

        foreach(var levelProperty in levelProperties)
        {
            UpgradeEntry entry = new UpgradeEntry
            {
                statName = GetUpgradeNameByType(levelProperty.UpgradeType),
                currentValue = GetCurrentValueByType(levelProperty.UpgradeType).ToString(),
                currentLevel = levelProperty.CurrentLevel,
                price = config.GetPriceEntry(levelProperty.UpgradeType).GetPrice(levelProperty.CurrentLevel)
            };
            entries.Add(entry);
        }

        return entries;
    }

    string GetUpgradeNameByType(UpgradeType upgradeType)
    {
        switch(upgradeType)
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
            default:
                return "Unknown";
        }
    }

    object GetCurrentValueByType(UpgradeType upgradeType)
    {
        switch(upgradeType)
        {
            case UpgradeType.SwordStrength:
                return config.SwordStrength(GetLevel(UpgradeType.SwordStrength)).ToString("F0");
            case UpgradeType.SwordThrowForce:
                return config.SwordThrowForce(GetLevel(UpgradeType.SwordThrowForce));
            case UpgradeType.SwordTurnForce:
                return config.SwordTurnForce(GetLevel(UpgradeType.SwordTurnForce));
            case UpgradeType.SwordTurnReactTime:
                return config.SwordTurnReactTime(GetLevel(UpgradeType.SwordTurnReactTime)) + "s";
            case UpgradeType.SwordAttackRange:
                return config.SwordAttackRange(GetLevel(UpgradeType.SwordAttackRange)) + "m";
            case UpgradeType.CriticalRate:
                return (config.CriticalRate(GetLevel(UpgradeType.CriticalRate)) * 100) + "%";
            case UpgradeType.CriticalDamageMultiplier:
                return config.CriticalDamageMultiplier(GetLevel(UpgradeType.CriticalDamageMultiplier)) * 100 + "%";
            default:
                return null;
        }
    }

}