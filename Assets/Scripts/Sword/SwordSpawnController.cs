using UnityEngine;

public class SwordSpawnController : MonoBehaviour
{
    [SerializeField] SwordControl sorwdControlPrefab;

    public void SpawnSorwd()
    {
        SwordDataSO swordData = StatContext.I.GetSwordData(SwordType.Normal);
        SwordControl sword = Instantiate(sorwdControlPrefab, transform.position, Quaternion.identity);
        sword.Initialize(swordData);
        Debug.Log("Sword Spawned!");
    }
}