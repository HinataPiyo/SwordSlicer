using UnityEngine;

[CreateAssetMenu(fileName = "FireDataSO", menuName = "EnemyData/FireDataSO")]
public class FireDataSO : EnemyDataSO
{
    [Header("波の振幅"), SerializeField] float waveAmplitude = 1f;
    [Header("波の周期数"), SerializeField] float waveCycles = 2f;

    public float WaveAmplitude => waveAmplitude;
    public float WaveCycles => waveCycles;
}