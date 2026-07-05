using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] AudioService s_Audio;
    [SerializeField] StatService s_Stat;
    [SerializeField] SaveAndLoadService s_SaveAndLoad;

    void Awake()
    {
        ServiceLocator.Register<IAudioService>(s_Audio);
        ServiceLocator.Register<IStateService>(s_Stat);
        ServiceLocator.Register<ISave>(s_SaveAndLoad);
        ServiceLocator.Register<ILoad>(s_SaveAndLoad);

        CurrencyManager.TestAddCurrency();
    }
}