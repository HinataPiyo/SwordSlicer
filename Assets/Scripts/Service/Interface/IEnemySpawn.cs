public interface ISpawnKinoko
{
    void SpawnEnemyBySpore(KinokoController enemy);
}

public interface ISpawnFire
{
    void SpawnEnemyFire(FireController fire, bool isFlip);
}