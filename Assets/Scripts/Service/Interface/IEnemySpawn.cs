public interface ISpawnKinoko
{
    void SpawnEnemyBySpore(KinokoController enemy);
}

public interface ISpawnFire
{
    void SpawnEnemyFire(FireContller fire, bool isFlip);
}