using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }
    [SerializeField] AudioDataSO audioData;
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioSource bgmSource;

    void Awake()
    {
        if(I == null) I = this;
    }

    void Start()
    {
        PlayBGM("Battle");
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
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }
}