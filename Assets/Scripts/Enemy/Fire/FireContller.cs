using UnityEngine;

public class FireContller : EnemyController
{
    FireDataSO convertData;
    public bool isFlip { get; private set; }
    public FireDataSO ConvertDataSO()
    {
        FireDataSO data = Data as FireDataSO;
        if (data == null)
        {
            Debug.LogError("FireDataSO is null");
            return null;
        }

        convertData = data;
        return convertData;
    }

    public void Initialize(Vector2 targetPosition, Vector2 startPosition, float enemyStatusMultiplier, bool isFlip = false)
    {
        base.Initialize(targetPosition, startPosition, enemyStatusMultiplier);
        this.isFlip = isFlip;
    }
}