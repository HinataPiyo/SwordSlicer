using UnityEngine;

[CreateAssetMenu(fileName = "BattleSettingConfig", menuName = "Config/BattleSettingConfig")]
public class BattleSettingConfig : ScriptableObject
{
    [Header("剣の生成間隔")]
    [SerializeField] float def_SwordCreateInterval = 5f;        // 剣の生成間隔
}