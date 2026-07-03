using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] AudioService s_Audio;
    [SerializeField] StatService s_Stat;

    void Awake()
    {
        ServiceLocator.Register<IAudioService>(s_Audio);
        ServiceLocator.Register<IStateService>(s_Stat);

        CurrencyManager.TestAddCurrency();
    }
}