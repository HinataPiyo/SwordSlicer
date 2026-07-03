using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeService : MonoBehaviour, ICameraShake
{
    CinemachineBasicMultiChannelPerlin perlin;

    void Awake()
    {
        perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ApplyShake(duration, magnitude));
    }

    IEnumerator ApplyShake(float duration, float magnitude)
    {
        perlin.AmplitudeGain = magnitude;
        yield return new WaitForSeconds(duration);
        perlin.AmplitudeGain = 0f;
    }
}