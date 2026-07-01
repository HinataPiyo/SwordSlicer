using UnityEngine;

public class AudioService : MonoBehaviour, IAudioService
{
    [SerializeField] AudioDataSO audioData;
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioSource bgmSource;

    void Start()
    {
        PlayBGM("MainBGM");
    }

    public void PlaySE(string name)
    {
        AudioClip clip = audioData.GetSE(name);
        if(clip != null) seSource.PlayOneShot(clip);
    }

    public void PlayBGM(string name)
    {
        AudioClip clip = audioData.GetBGM(name);
        if(clip != null)
        {
            bgmSource.Stop();

            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }
}