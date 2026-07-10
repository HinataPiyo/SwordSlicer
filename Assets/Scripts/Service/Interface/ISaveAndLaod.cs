public interface ISave
{
    void Save();
    bool HaSaveData();
    bool HasPlayedOnce();
}

public interface ILoad
{
    void Load();
    event System.Action OnLoad;
}