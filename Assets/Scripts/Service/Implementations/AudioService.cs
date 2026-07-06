using UnityEngine;
using UnityEngine.Audio;

public enum AudioType { Master, BGM, SE }
public class AudioService : MonoBehaviour, IAudioService
{
    [SerializeField] AudioMixer audioMixer;
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

    public void SetVolume(AudioType type, float volume)
    {
        switch(type)
        {
            case AudioType.Master:
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
                break;
            case AudioType.BGM:
                audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
                break;
            case AudioType.SE:
                audioMixer.SetFloat("SEVolume", Mathf.Log10(volume) * 20);
                break;
        }
    }

    public float GetVolume(AudioType type)
    {
        float volume = 0f;
        switch(type)
        {
            case AudioType.Master:
                audioMixer.GetFloat("MasterVolume", out volume);
                break;
            case AudioType.BGM:
                audioMixer.GetFloat("BGMVolume", out volume);
                break;
            case AudioType.SE:
                audioMixer.GetFloat("SEVolume", out volume);
                break;
        }
        return Mathf.Pow(10, volume / 20);
    }
}