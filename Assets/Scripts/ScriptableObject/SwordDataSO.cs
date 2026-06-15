using UnityEngine;

public enum SwordType { Normal, Advanced, Ultimate }
[CreateAssetMenu(fileName = "SwordDataSO", menuName = "Data/SwordDataSO")]
public class SwordDataSO : ScriptableObject
{
    [SerializeField] float def_SwordStrengthMultiply = 1f;     // 剣の攻撃力の倍率
    [SerializeField] Sprite icon;     // ストックアイコンに表示するスプライト
    
    public float SwordStrengthMultiply() => def_SwordStrengthMultiply;     // 剣の攻撃力の倍率は固定値とする
    public Sprite Icon => icon;
}