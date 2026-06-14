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

[System.Serializable]
public class LevelProperty
{
    [field: SerializeField, ReadOnly] public int CurrentLevel { get; private set; } = 1;
    [SerializeField] int maxLevel = 8;
    public int MaxLevel => maxLevel;

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