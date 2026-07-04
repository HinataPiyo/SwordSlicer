using UnityEngine;

public class FireMovement : EnemyMovement
{
    FireContller fireController;
    FireDataSO fireData;

    void Awake()
    {
        fireController = GetComponent<FireContller>();
    }

    protected override void ConvertData()
    {
        FireDataSO data = Data as FireDataSO;
        if(data == null)
        {
            Debug.LogError("FireMovement: Data is not of type FireDataSO.");
            return;
        }

        fireData = data;
    }

    protected override void UpdateMovement()
    {
        progress += Time.deltaTime / Data.ReachDuration;

        float baseX = Mathf.Lerp(startPosition.x, targetPosition.x, progress);
        float baseY = Mathf.Lerp(startPosition.y, targetPosition.y, progress);
        float wobble = Mathf.Sin(progress * Mathf.PI * 2f * fireData.WaveCycles) * fireData.WaveAmplitude;
        float wobbleDirection = fireController.isFlip ? -1f : 1f;

        float newX = baseX;
        float newY = baseY + wobble * wobbleDirection;

        transform.position = new Vector2(newX, newY);
        UpdateScale();
    }
}