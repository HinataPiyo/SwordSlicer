using System.Collections;
using UnityEngine;

public class ZombieMovement : EnemyMovement<ZombieDataSO>
{
    WaitForSeconds waitForInvulnerableTime;
    Coroutine invulnerableCoroutine;
    public event System.Action<bool> OnInvulnerableChanged;    // 無敵状態が変化したときのイベント

    bool isInvulnerable = false;
    public bool IsInvulnerable
    {
        get => isInvulnerable;
        set
        {
            if (isInvulnerable == value) return;
            isInvulnerable = value;

            OnInvulnerableChanged?.Invoke(isInvulnerable);    // 無敵状態が変化したときにイベントを発火
            if (invulnerableCoroutine != null) return;

            if (isInvulnerable)
            {
                invulnerableCoroutine = StartCoroutine(InvulnerableRoutine());
            }
        }
    }
    public override void Initialize(EnemyDataSO enemyDataSO, Vector2 borderLinePos, Vector2 startPos, System.Action<string> changeAnimation)
    {
        base.Initialize(enemyDataSO, borderLinePos, startPos, changeAnimation);

        if (!TryGetTypedData(out var data))
        {
            enabled = false;
            return;
        }

        waitForInvulnerableTime = new WaitForSeconds(data.InvulnerableTime);
    }

    IEnumerator InvulnerableRoutine()
    {
        yield return waitForInvulnerableTime;
        IsInvulnerable = false;
        invulnerableCoroutine = null;
    }

    protected override bool ShouldPauseMovement()
    {
        return IsInvulnerable;
    }
    
}