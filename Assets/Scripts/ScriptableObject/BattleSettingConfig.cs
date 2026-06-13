using UnityEngine;

[CreateAssetMenu(fileName = "BattleSettingConfig", menuName = "Config/BattleSettingConfig")]
public class BattleSettingConfig : ScriptableObject
{
    [Header("剣の生成間隔")]
    [SerializeField] float def_SwordCreateInterval = 5f;        // 剣の生成間隔
    public const int MAX_SWORD_STOCK = 5;     // ストックの最大数
    [field: SerializeField, ReadOnly] public int CurrentStock { get; private set; } = MAX_SWORD_STOCK;     // 現在のストック数

    public float SwordCreateInterval() => def_SwordCreateInterval;
}