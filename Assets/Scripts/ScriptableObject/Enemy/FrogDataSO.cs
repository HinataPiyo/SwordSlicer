using UnityEngine;

[CreateAssetMenu(fileName = "FrogDataSO", menuName = "EnemyData/FrogDataSO")]
public class FrogDataSO : EnemyDataSO
{
    [Header("ジャンプの高さ"), SerializeField] float jumpHeight;
    [Header("ジャンプ後の硬直時間"), SerializeField] float jumpCooldown;
    [Header("アニメーションの名前")] 
    [SerializeField] string jumpAnimationName;

    public float JumpHeight => jumpHeight;
    public float JumpCooldown => jumpCooldown;
    public string JumpAnimationName => jumpAnimationName;
}