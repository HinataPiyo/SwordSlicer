using UnityEngine;

[CreateAssetMenu(fileName = "GoblinDataSO", menuName = "EnemyData/GoblinDataSO")]
public class GoblinDataSO : EnemyDataSO
{
    [Header("一度に進む距離"), SerializeField] float moveDistance;
    [Header("移動後の硬直時間"), SerializeField] float moveCooldown;
    [Header("アニメーションの名前")]
    [SerializeField] string moveAnimationName;
    public float MoveDistance => moveDistance;
    public float MoveCooldown => moveCooldown;
    public string MoveAnimationName => moveAnimationName;
}