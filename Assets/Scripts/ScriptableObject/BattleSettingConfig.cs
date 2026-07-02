using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// 新しい強化要素を作成する場合は, StatContextのUpgradeTypeに新しい要素を追加し
/// LevelProperty[] と PriceEntry[] にも新しい要素を追加する必要がある
/// </summary>
[CreateAssetMenu(fileName = "BattleSettingConfig", menuName = "Config/BattleSettingConfig")]
public class BattleSettingConfig : ScriptableObject
{

    public readonly static PriceEntry[] PriceEntries = new PriceEntry[]
    {
        new PriceEntry(UpgradeType.SwordStrength, 200, 1.2f),
        new PriceEntry(UpgradeType.SwordCreateInterval, 800, 1.3f),
        new PriceEntry(UpgradeType.CriticalRate, 1000, 1.2f),
        new PriceEntry(UpgradeType.CriticalDamageMultiplier, 700, 1.2f),
        new PriceEntry(UpgradeType.SwordStock, 700, 1.5f),
        new PriceEntry(UpgradeType.SwordAttackRange, 1200, 1.8f),
        new PriceEntry(UpgradeType.SwordThrowForce, 500, 2.2f),
        new PriceEntry(UpgradeType.SwordTurnForce, 700, 2.5f),
        new PriceEntry(UpgradeType.SwordTurnReactTime, 700, 2f),
    };

    public readonly static LevelProperty[] LevelProperties = new LevelProperty[]
    {
        new LevelProperty(UpgradeType.SwordStrength, 100),
        new LevelProperty(UpgradeType.SwordCreateInterval, 8),
        new LevelProperty(UpgradeType.CriticalRate, 8),
        new LevelProperty(UpgradeType.CriticalDamageMultiplier, 8),
        new LevelProperty(UpgradeType.SwordStock, 5),
        new LevelProperty(UpgradeType.SwordAttackRange, 5),
        new LevelProperty(UpgradeType.SwordThrowForce, 8),
        new LevelProperty(UpgradeType.SwordTurnForce, 8),
        new LevelProperty(UpgradeType.SwordTurnReactTime, 8),
    };
    
    public static PriceEntry GetPriceEntry(UpgradeType upgradeType)
    {
        foreach(var entry in PriceEntries)
        {
            if(entry.upgradeType == upgradeType)
            {
                return entry;
            }
        }
        Debug.LogError("指定されたUpgradeTypeに対応するPriceEntryが見つかりませんでした: " + upgradeType);
        return null;
    }

    [Header("ストック")]
    public const int MAX_SWORD_STOCK = 5;     // ストックの最大数
    const int DEFAULT_STOCK = 1;           // 初期ストック数
    public int CurrentMaxStock(int currentLevel)
    {
        return Mathf.Min(DEFAULT_STOCK + currentLevel, MAX_SWORD_STOCK);
    }
    [Header("剣の生成間隔")]
    [SerializeField] float def_SwordCreateInterval = 5f;        // 剣の生成間隔
    [SerializeField, Range(0.4f, 1f)] float[] def_SwordCreateIntervalMultiply;    // 剣の生成間隔の倍率
    public float SwordCreateInterval(int currentLevel)
    {
        return def_SwordCreateInterval * def_SwordCreateIntervalMultiply[currentLevel];
    }


    [Header("剣の攻撃力")]
    [SerializeField] float swordStrength = 1f;     // 剣の攻撃力
    [SerializeField] float def_SwordStrengthLevelUpAmount = 0.5f;     // 剣の攻撃力のレベルアップごとの増加量
    public float SwordStrength(int currentLevel) => swordStrength + currentLevel * def_SwordStrengthLevelUpAmount;     // 剣の攻撃力はレベルに応じて増加するようにする

    [Header("剣のデータ")]
    [SerializeField] SwordDataByType[] swordData;
    public SwordDataByType[] SwordDatas => swordData;
    public Sprite GetSwordIcon(SwordType type)
    {
        foreach(var data in swordData)
        {
            if(data.type == type)
                return data.swordDataSO.Icon;
        }
        Debug.LogError("指定されたSwordTypeに対応する剣のアイコンが見つかりませんでした: " + type);
        return null;
    }

    [Header("剣を投げる力")]
    [SerializeField] float def_SwordThrowForce = 10f;           // 剣の飛ぶ力
    [SerializeField, Range(0.8f, 2.5f)] float[] def_SwordThrowForceMultiply;    // 剣の飛ぶ力の倍率
    public float SwordThrowForce(int currentLevel)
    {
        return def_SwordThrowForce * def_SwordThrowForceMultiply[currentLevel];
    }

    [Header("剣の曲がる力")]
    [SerializeField] float def_SwordTurnForce = 2f;             // 剣の曲がる力
    [SerializeField, Range(0.8f, 1.6f)] float[] def_SwordTurnForceMultiply;    // 剣の曲がる力の倍率
    [SerializeField, Range(1f, 0.35f)] float[] def_SwordTurnReactTime;    // 剣の曲がる力の反応時間の倍率
    
    public float SwordTurnReactTime(int currentLevel)
    {
        return def_SwordTurnReactTime[currentLevel];
    }

    public float SwordTurnForce(int currentLevel)
    {
        return def_SwordTurnForce * def_SwordTurnForceMultiply[currentLevel];
    }


    [Header("剣の回転力/攻撃速度")]
    [SerializeField] float def_SwordAttackInterval = 0.1f;          // 剣の攻撃間隔
    [SerializeField] float def_MinSwordAttackInterval = 0.1f;       // 剣の攻撃間隔の最小値

    [SerializeField] float def_MaxRotationAmount = 10f;             // 剣の最大回転力

    public float SwordAttackInterval(float rotateAmount)
    {
        // 回転量に応じて攻撃間隔を短くする
        float attackInterval = def_SwordAttackInterval - (Mathf.Abs(rotateAmount) * 0.01f);
        float interval = Mathf.Max(def_MinSwordAttackInterval, attackInterval);
        return interval;   // 最小値を超えないようにする
    }

    public float MaxRotationAmount() => def_MaxRotationAmount;
    

    [Header("剣の攻撃範囲")]
    [SerializeField] float def_SwordAttackRange = 0.5f;          // 剣の攻撃範囲
    [SerializeField, Range(0.8f, 1.5f)] float[] def_SwordAttackRangeMultiply;    // 剣の攻撃範囲の倍率

    public float SwordAttackRange(int currentLevel)
    {
        return def_SwordAttackRange * def_SwordAttackRangeMultiply[currentLevel];
    }

    [Header("クリティカル")]
    [SerializeField, Range(0f, 1f)] float def_CriticalRate = 0.1f;              // クリティカル率 (0~1の範囲で設定)    
    [SerializeField] float def_CriticalDamageMultiplier = 1.2f;    // クリティカルダメージ倍率
    
    public float CriticalRate(int currentLevel) => def_CriticalRate * currentLevel;    // クリティカル率はレベルに応じて増加するようにする
    public float CriticalDamageMultiplier(int currentLevel) => (def_CriticalDamageMultiplier - 1) * currentLevel + 1;    // クリティカルダメージ倍率はレベルに応じて増加するようにする

    [System.Serializable]
    public class SwordDataByType
    {
        public SwordType type;
        public SwordDataSO swordDataSO;
        [Range(0f, 1f)] public float createProbability;    // その剣が生成される確率 (0~1の範囲で設定)

        public string GetTypeName()
        {
            return type.ToString();
        }

        public string GetSwordName()
        {
            switch(type)
            {
                case SwordType.Normal:
                    return "手になじむ剣";
                case SwordType.Advanced:
                    return "池から出てきた剣";
                case SwordType.Ultimate:
                    return "たぶん凄い神器";
                default:
                    return "不明な剣のタイプ";
            }
        }
        
        public string GetEffectDescription()
        {
            switch(type)
            {
                case SwordType.Normal:
                    return $"高確率で生成され、{swordDataSO.SwordStrengthMultiply()}倍の攻撃力を持つ基本的な剣。";
                case SwordType.Advanced:
                    return $"中程度の確率で生成され、{swordDataSO.SwordStrengthMultiply()}倍の攻撃力を持つ強化された剣。";
                case SwordType.Ultimate:
                    return $"低確率で生成され、{swordDataSO.SwordStrengthMultiply()}倍の攻撃力を持つ究極の剣。";
                default:
                    return "不明な剣のタイプです。";
            }
        }
    }
}

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