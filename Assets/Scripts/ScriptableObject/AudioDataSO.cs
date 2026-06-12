using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "AudioDataSO")]
public class AudioDataSO : ScriptableObject
{
    [SerializeField] AudioClipData[] se;
    [SerializeField] AudioClipData[] bgm;

    public AudioClip GetSE(string tag)
    {
        foreach(var data in se)
        {
            if(data.tag == tag) return data.clip;
        }
        Debug.LogError($"AudioDataSO: SE with tag '{tag}' not found.");
        return null;
    }

    public AudioClip GetBGM(string tag)
    {
        foreach(var data in bgm)
        {
            if(data.tag == tag) return data.clip;
        }
        Debug.LogError($"AudioDataSO: BGM with tag '{tag}' not found.");
        return null;
    }

    [System.Serializable]
    public class AudioClipData
    {
        public string tag;
        public AudioClip clip;
    }
}