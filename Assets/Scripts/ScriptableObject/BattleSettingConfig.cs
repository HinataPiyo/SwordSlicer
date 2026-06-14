using UnityEngine;

[CreateAssetMenu(fileName = "BattleSettingConfig", menuName = "Config/BattleSettingConfig")]
public class BattleSettingConfig : ScriptableObject
{
    public const int MAX_SWORD_STOCK = 5;     // ストックの最大数
    [Header("剣の生成間隔")]
    [SerializeField] float def_SwordCreateInterval = 5f;        // 剣の生成間隔
    [field: SerializeField, ReadOnly] public int CurrentStock { get; private set; } = MAX_SWORD_STOCK;     // 現在のストック数

    [Header("剣の攻撃力")]
    [SerializeField] float swordStrength = 1f;     // 剣の攻撃力

    [Header("剣のデータ")]
    [SerializeField] SwordDataByType[] swordData;
    public SwordDataByType[] SwordDatas => swordData;

    [Header("剣を投げる力")]
    [SerializeField] float def_SwordThrowForce = 10f;           // 剣の飛ぶ力
    [SerializeField, Range(0.8f, 2.5f)] float[] def_SwordThrowForceMultiply;    // 剣の飛ぶ力の倍率
    [SerializeField] LevelProperty level_SwordThrow = new LevelProperty();    // 剣の飛ぶ力のレベル
    public float SwordThrowForce()
    {
        return def_SwordThrowForce * def_SwordThrowForceMultiply[level_SwordThrow.CurrentLevel - 1];
    }

    [Header("剣の曲がる力")]
    [SerializeField] float def_SwordTurnForce = 2f;             // 剣の曲がる力
    [SerializeField, Range(0.8f, 2.5f)] float[] def_SwordTurnForceMultiply;    // 剣の曲がる力の倍率
    [SerializeField, Range(0.6f, 2f)] float[] def_SwordTurnReactTime;    // 剣の曲がる力の反応時間の倍率
    [SerializeField] LevelProperty level_SwordTurn = new LevelProperty();      // 剣の曲がる力のレベル
    [Header("剣の回転力/攻撃速度")]
    [SerializeField] float def_SwordAttackInterval = 0.1f;          // 剣の攻撃間隔
    [SerializeField] float def_MinSwordAttackInterval = 0.1f;       // 剣の攻撃間隔の最小値

    [SerializeField] float def_MaxRotationAmount = 10f;             // 剣の最大回転力

    public float SwordAttackInterval(float rotateAmount)
    {
        // 回転量に応じて攻撃間隔を短くする
        float attackInterval = def_SwordAttackInterval - (rotateAmount * 0.01f);
        return Mathf.Max(def_MinSwordAttackInterval, attackInterval);    // 攻撃間隔の最小値を0.01秒に設定する
    }

    public float MaxRotationAmount() => def_MaxRotationAmount;
    public float SwordTurnForce()
    {
        return def_SwordTurnForce * def_SwordTurnForceMultiply[level_SwordTurn.CurrentLevel - 1];
    }
    public float SwordTurnReactTime()
    {
        return def_SwordTurnReactTime[level_SwordTurn.CurrentLevel - 1];
    }

    [Header("剣の攻撃範囲")]
    [SerializeField] float def_SwordAttackRange = 0.5f;          // 剣の攻撃範囲
    [SerializeField, Range(0.8f, 1.5f)] float[] def_SwordAttackRangeMultiply;    // 剣の攻撃範囲の倍率
    [SerializeField] LevelProperty level_SwordAttackRange = new LevelProperty();    // 剣の攻撃範囲のレベル

    public float SwordAttackRange()
    {
        return def_SwordAttackRange * def_SwordAttackRangeMultiply[level_SwordAttackRange.MaxLevel - 1];
    }

    [Header("クリティカル")]
    [SerializeField, Range(0f, 1f)] float def_CriticalRate = 0.1f;              // クリティカル率 (0~1の範囲で設定)    
    [SerializeField] LevelProperty level_CriticalRate = new LevelProperty();    // クリティカル率のレベル
    [SerializeField] float def_CriticalDamageMultiplier = 1.2f;    // クリティカルダメージ倍率
    [SerializeField] LevelProperty level_CriticalDamage = new LevelProperty();    // クリティカルダメージ倍率のレベル
    
    public float CriticalRate => def_CriticalRate * level_CriticalRate.CurrentLevel;    // クリティカル率はレベルに応じて増加するようにする
    public float CriticalDamageMultiplier => (def_CriticalDamageMultiplier - 1) * level_CriticalDamage.CurrentLevel + 1;    // クリティカルダメージ倍率はレベルに応じて増加するようにする

    public float SwordCreateInterval() => def_SwordCreateInterval;
    public float SwordStrength() => swordStrength;

    [System.Serializable]
    public class SwordDataByType
    {
        public SwordType type;
        public SwordDataSO swordDataSO;
        [Range(0f, 1f)] public float createProbability;    // その剣が生成される確率 (0~1の範囲で設定)
    }
}