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