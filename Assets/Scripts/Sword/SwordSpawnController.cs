using UnityEngine;
using UnityEngine.InputSystem;

public class SwordSpawnController : MonoBehaviour
{
    [SerializeField] SwordControl sorwdControl;
    

    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnSorwd();
        }
    }

    void SpawnSorwd()
    {
        Instantiate(sorwdControl, transform.position, Quaternion.identity);
        Debug.Log("Sword Spawned!");
    }
}