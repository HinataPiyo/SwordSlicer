using UnityEngine;
using UnityEngine.InputSystem;

public class SwordSpawnController : MonoBehaviour
{
    [SerializeField] SwordControl sorwdControl;
    

    public void SpawnSorwd()
    {
        Instantiate(sorwdControl, transform.position, Quaternion.identity);
        Debug.Log("Sword Spawned!");
    }
}