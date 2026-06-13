using UnityEngine;

public class StatContext : MonoBehaviour
{
    public static StatContext I { get; private set; }

    [Header("設定")]
    [SerializeField] BattleSettingConfig config;
    [SerializeField] SwordDataByType[] swordData;

    public SwordDataSO GetSwordData(SwordType type)
    {
        foreach(var data in swordData)
        {
            if(data.type == type)
            {
                return data.swordDataSO;
            }
        }
        Debug.LogError("指定された剣のデータが見つかりませんでした: " + type);
        return null;
    }

    void Awake()
    {
        if(I == null) I = this;
    }

}

[System.Serializable]
public class SwordDataByType
{
    public SwordType type;
    public SwordDataSO swordDataSO;
}