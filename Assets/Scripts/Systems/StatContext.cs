using UnityEngine;

public class StatContext : MonoBehaviour
{
    public static StatContext I { get; private set; }

    [Header("設定")]
    [SerializeField] BattleSettingConfig config;

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

    /// <summary>
    /// 剣の攻撃力を計算して返す
    //  クリティカルヒットの場合は攻撃力にクリティカルダメージ倍率を掛ける
    //  クリティカルかどうかは、剣のデータに設定されたクリティカル率を元にランダムに判定する
    //  例えば、クリティカル率が0.1の場合、10%の確率でクリティカルヒットになる
    /// </summary>
    /// <param name="swordData"></param>
    /// <returns></returns>
    public float GetDamageAmount(SwordDataSO swordData, out bool isCritical)
    {
        float baseStrength = config.SwordStrength() * swordData.SwordStrengthMultiply();
        isCritical = Random.value < config.CriticalRate;     // クリティカルかどうかをランダムに判定
        if(isCritical)
        {
            return baseStrength * config.CriticalDamageMultiplier;     // クリティカルの場合
        }

        return baseStrength;     // 通常の場合
    }

    public float SwordThrowForce() => config.SwordThrowForce();
    public float SwordTurnForce() => config.SwordTurnForce();
    public float SwordTurnReactTime() => config.SwordTurnReactTime();
    public float GetStockInterval() => config.SwordCreateInterval();
    public int GetCurrentMaxStock() => config.CurrentStock;
    public float SwordAttackRange() => config.SwordAttackRange();
    public float SwordAttackInterval(float rotateAmount) => config.SwordAttackInterval(rotateAmount);
    public float MaxRotationAmount() => config.MaxRotationAmount();

    void Awake()
    {
        if(I == null) I = this;
    }

}