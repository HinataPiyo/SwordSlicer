using System.Collections;
using UnityEngine;

public class TrollHealth : EnemyHealth<TrollDataSO>
{
    // 初回判定結果を固定し、以降の攻撃可否を変えないようにする
    // ?は初期値がnullであることを示すnullable型を表す
    bool? lockedAttackable = null;
    bool LockedAttackable
    {
        get => lockedAttackable.Value;
        set
        {
            lockedAttackable = value;
            StartCoroutine(ResetLockedAttackable());
        }
    }

    public override bool CheckAttackable(Vector2 attackPoint)
    {
        base.CheckAttackable(attackPoint);
        return ResolveAttackable(attackPoint);
    }

    /// <summary>
    /// 攻撃が正面からの攻撃かどうかを判定し、初回判定結果を固定する
    /// </summary>
    bool ResolveAttackable(Vector2 attackPosition)
    {
        // 初回判定結果が存在する場合はそれを返す
        if (lockedAttackable.HasValue) return lockedAttackable.Value;

        bool isAttackable = IsAttackFromSideOrBack(attackPosition);
        LockedAttackable = isAttackable;
        return isAttackable;
    }

    /// <summary>
    /// 初回判定結果をリセットする
    /// </summary>
    IEnumerator ResetLockedAttackable()
    {
        if (!TryGetTypedData(out var data)) yield break;

        yield return new WaitForSeconds(data.LockedAttackableResetTime);
        lockedAttackable = null;
    }

    /// <summary>
    /// 攻撃が正面からの攻撃かどうかを判定する
    /// </summary>
    bool IsAttackFromSideOrBack(Vector2 attackPosition)
    {
        if (!TryGetTypedData(out var data)) return false;

        Vector2 toAttacker = (attackPosition - (Vector2)transform.position).normalized;
        if (toAttacker == Vector2.zero)
        {
            return true;
        }

        Vector2 forward = transform.TransformDirection(data.LocalForward).normalized;
        if (forward == Vector2.zero)
        {
            forward = transform.up;
        }

        float angleFromFront = Vector2.Angle(forward, toAttacker);
        return angleFromFront >= data.MinAttackAngleFromFront;
    }
}