using UnityEngine;

[CreateAssetMenu(fileName = "TrollDataSO", menuName = "EnemyData/TrollDataSO")]
public class TrollDataSO : EnemyDataSO
{
    [SerializeField, Range(0f, 180f)]
    float minAttackAngleFromFront = 60f;
    [SerializeField]
    Vector2 localForward = Vector2.down;
    [SerializeField] float lockedAttackableResetTime = 0.5f;

    public float MinAttackAngleFromFront => minAttackAngleFromFront;
    public Vector2 LocalForward => localForward;
    public float LockedAttackableResetTime => lockedAttackableResetTime;
}