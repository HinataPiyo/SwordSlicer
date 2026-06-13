using UnityEngine;

public enum SwordType { Normal, One, Two }
[CreateAssetMenu(fileName = "SwordDataSO", menuName = "Data/SwordDataSO")]
public class SwordDataSO : ScriptableObject
{
    [SerializeField] Sprite icon;     // ストックアイコンに表示するスプライト
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
    public float SwordTurnForce()
    {
        return def_SwordTurnForce * def_SwordTurnForceMultiply[level_SwordTurn.CurrentLevel - 1];
    }
    public float SwordTurnReactTime()
    {
        return def_SwordTurnReactTime[level_SwordTurn.CurrentLevel - 1];
    }

    [Header("剣の攻撃範囲")]
    [SerializeField] float def_SwordAttackStrength = 1f;         // 剣の攻撃力
    [SerializeField] float def_SwordAttackRange = 0.5f;          // 剣の攻撃範囲
    [SerializeField, Range(0.8f, 2.2f)] float[] def_SwordAttackRangeMultiply;    // 剣の攻撃範囲の倍率
    [SerializeField] LevelProperty level_SwordAttack = new LevelProperty();    // 剣の攻撃範囲のレベル
    public float SwordAttackRange()
    {
        return def_SwordAttackRange * def_SwordAttackRangeMultiply[level_SwordAttack.CurrentLevel - 1];
    }

    public float SwordAttackStrength => def_SwordAttackStrength;
    public Sprite Icon => icon;
}

[System.Serializable]
public class LevelProperty
{
    [field: SerializeField, ReadOnly] public int CurrentLevel { get; private set; } = 1;
    [SerializeField] int maxLevel = 8;

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