public interface IAudioService
{
    void PlaySE(string tag);
    void PlayBGM(string tag);
    void SetVolume(AudioType type, float volume);
    float GetVolume(AudioType type);
    AudioVolumeData GetVolumeData();
    void LoadVolumeData(AudioVolumeData volumeData);
}