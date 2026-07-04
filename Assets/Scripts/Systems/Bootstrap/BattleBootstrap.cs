using UnityEngine;

public class BattleBootstrap : MonoBehaviour
{
    [SerializeField] SwordSpawnController s_SwordSpawnController;
    [SerializeField] ResultService s_Result;
    [SerializeField] CameraShakeService s_CameraShakeService;
    [SerializeField] EnemySpawnController s_EnemySpawnController;

    void Awake()
    {
        ServiceLocator.Register<ISwordDraggingArea>(s_SwordSpawnController);
        ServiceLocator.Register<IResultService>(s_Result);
        ServiceLocator.Register<ICameraShake>(s_CameraShakeService);
        ServiceLocator.Register<ISpawnKinoko>(s_EnemySpawnController);
    }
}