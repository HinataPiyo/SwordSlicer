using UnityEngine;

public class FrogHealth : EnemyHealth
{
    FrogMovement frogMovement;

    void Awake()
    {
        frogMovement = GetComponent<FrogMovement>();
    }

    /// <summary>
    /// 敵が攻撃可能かどうかをチェックする
    /// </summary>
    public override bool CheckAttackable(Vector2 attackPoint)
    {
        return !frogMovement.IsAnimation;    // ジャンプ中でなければ攻撃可能
    }
}