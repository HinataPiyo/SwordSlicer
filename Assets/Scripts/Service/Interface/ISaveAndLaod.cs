public interface ISave
{
    void Save();
    bool HaSaveData();
}

public interface ILoad
{
    void Load();
    event System.Action OnLoad;
}