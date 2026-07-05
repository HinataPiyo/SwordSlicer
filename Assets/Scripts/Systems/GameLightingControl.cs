using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameLightingControl : MonoBehaviour
{
    [SerializeField] EnemySpawnScheduleSO spawnScheduleSO;

    [SerializeField] Light2D spotLight;

    static readonly Dictionary<DifficultyLevel, float> difficultyToLightIntensity = new Dictionary<DifficultyLevel, float>
    {
        { DifficultyLevel.Easy, 0f },
        { DifficultyLevel.Normal, 1f },
        { DifficultyLevel.Hard, 5f },
        { DifficultyLevel.Extreme, 10f },
        { DifficultyLevel.Nightmare, 15f }
    };

    static readonly Dictionary<DifficultyLevel, Color> difficultyToLightColor = new Dictionary<DifficultyLevel, Color>
    {
        { DifficultyLevel.Easy, Color.white },
        { DifficultyLevel.Normal, Color.yellow },
        { DifficultyLevel.Hard, Color.red },
        { DifficultyLevel.Extreme, Color.magenta },
        { DifficultyLevel.Nightmare, Color.blue }
    };

    void Awake()
    {
        DifficultyLevel currentDifficulty = spawnScheduleSO.GetDifficultyLevel();
        if (difficultyToLightIntensity.TryGetValue(currentDifficulty, out float intensity))
        {
            spotLight.intensity = intensity;
        }

        if (difficultyToLightColor.TryGetValue(currentDifficulty, out Color color))
        {
            spotLight.color = color;
        }
    }
}