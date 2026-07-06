using UnityEngine;

/// <summary>
/// バトル中に参照するパラメータ設定。
/// アップグレード定義（価格・レベル定義・ロード処理）は partial の別ファイルで管理する。
/// </summary>
[CreateAssetMenu(fileName = "BattleSettingConfig", menuName = "Config/BattleSettingConfig")]
public partial class BattleSettingConfig : ScriptableObject
{
    [Header("ストック")]
    public const int MAX_SWORD_STOCK = 5;
    const int DEFAULT_STOCK = 1;

    [Header("剣の生成間隔")]
    [SerializeField] float def_SwordCreateInterval = 5f;
    [SerializeField, Range(0.4f, 1f)] float[] def_SwordCreateIntervalMultiply;

    [Header("剣の攻撃力")]
    [SerializeField] float swordStrength = 1f;
    [SerializeField] float def_SwordStrengthLevelUpAmount = 0.5f;

    [Header("剣のデータ")]
    [SerializeField] SwordDataByType[] swordData;

    [Header("剣を投げる力")]
    [SerializeField] float def_SwordThrowForce = 10f;
    [SerializeField, Range(0.8f, 2.5f)] float[] def_SwordThrowForceMultiply;

    [Header("剣の曲がる力")]
    [SerializeField] float def_SwordTurnForce = 2f;
    [SerializeField, Range(0.8f, 1.6f)] float[] def_SwordTurnForceMultiply;
    [SerializeField, Range(1f, 0.35f)] float[] def_SwordTurnReactTime;

    [Header("剣の回転力/攻撃速度")]
    [SerializeField] float def_SwordAttackInterval = 0.1f;
    [SerializeField] float def_MinSwordAttackInterval = 0.1f;
    [SerializeField] float def_MaxRotationAmount = 10f;

    [Header("剣の攻撃範囲")]
    [SerializeField] float def_SwordAttackRange = 0.5f;
    [SerializeField, Range(0.8f, 1.5f)] float[] def_SwordAttackRangeMultiply;

    [Header("クリティカル")]
    [SerializeField, Range(0f, 1f)] float def_CriticalRate = 0.1f;
    [SerializeField] float def_CriticalDamageMultiplier = 1.2f;

    public SwordDataByType[] SwordDatas => swordData;

    float GetLevelArrayValue(float[] values, int currentLevel, float fallbackValue, string fieldName)
    {
        if (values == null || values.Length == 0)
        {
            Debug.LogError($"{fieldName} が未設定です。フォールバック値 {fallbackValue} を使用します。");
            return fallbackValue;
        }

        int levelIndex = Mathf.Clamp(currentLevel, 0, values.Length - 1);
        return values[levelIndex];
    }

    public int CurrentMaxStock(int currentLevel)
    {
        return Mathf.Min(DEFAULT_STOCK + currentLevel, MAX_SWORD_STOCK);
    }

    public float SwordCreateInterval(int currentLevel)
    {
        float multiply = GetLevelArrayValue(def_SwordCreateIntervalMultiply, currentLevel, 1f, nameof(def_SwordCreateIntervalMultiply));
        return def_SwordCreateInterval * multiply;
    }

    public float SwordStrength(int currentLevel)
    {
        return swordStrength + currentLevel * def_SwordStrengthLevelUpAmount;
    }

    public Sprite GetSwordIcon(SwordType type)
    {
        foreach (var data in swordData)
        {
            if (data.type == type)
            {
                return data.swordDataSO.Icon;
            }
        }

        Debug.LogError("指定されたSwordTypeに対応する剣のアイコンが見つかりませんでした: " + type);
        return null;
    }

    public float SwordThrowForce(int currentLevel)
    {
        float multiply = GetLevelArrayValue(def_SwordThrowForceMultiply, currentLevel, 1f, nameof(def_SwordThrowForceMultiply));
        return def_SwordThrowForce * multiply;
    }

    public float SwordTurnReactTime(int currentLevel)
    {
        return GetLevelArrayValue(def_SwordTurnReactTime, currentLevel, 1f, nameof(def_SwordTurnReactTime));
    }

    public float SwordTurnForce(int currentLevel)
    {
        float multiply = GetLevelArrayValue(def_SwordTurnForceMultiply, currentLevel, 1f, nameof(def_SwordTurnForceMultiply));
        return def_SwordTurnForce * multiply;
    }

    public float SwordAttackInterval(float rotateAmount)
    {
        float attackInterval = def_SwordAttackInterval - (Mathf.Abs(rotateAmount) * 0.01f);
        return Mathf.Max(def_MinSwordAttackInterval, attackInterval);
    }

    public float MaxRotationAmount()
    {
        return def_MaxRotationAmount;
    }

    public float SwordAttackRange(int currentLevel)
    {
        float multiply = GetLevelArrayValue(def_SwordAttackRangeMultiply, currentLevel, 1f, nameof(def_SwordAttackRangeMultiply));
        return def_SwordAttackRange * multiply;
    }

    public float CriticalRate(int currentLevel)
    {
        return Mathf.Clamp01(def_CriticalRate * currentLevel);
    }

    public float CriticalDamageMultiplier(int currentLevel)
    {
        return (def_CriticalDamageMultiplier - 1f) * currentLevel + 1f;
    }

    [System.Serializable]
    public class SwordDataByType
    {
        public SwordType type;
        public SwordDataSO swordDataSO;
        [Range(0f, 1f)] public float createProbability;

        public string GetTypeName()
        {
            return type.ToString();
        }

        public string GetSwordName()
        {
            switch (type)
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
            switch (type)
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