using UnityEngine;

public class BattleBootstrap : MonoBehaviour
{
    [SerializeField] SwordSpawnController s_SwordSpawnController;
    [SerializeField] ResultService s_Result;

    void Awake()
    {
        ServiceLocator.Register<ISwordDraggingArea>(s_SwordSpawnController);
        ServiceLocator.Register<IResultService>(s_Result);
    }
}