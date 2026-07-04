using System.Collections;
using UnityEngine;

public class ZombieMovement : EnemyMovement
{
    ZombieDataSO zombieData;
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


    protected override void ConvertData()
    {
        if(Data is not ZombieDataSO)
        {
            Debug.LogError("ZombieMovement: Data is not ZombieDataSO");
            return;
        }

        zombieData = Data as ZombieDataSO;
    }

    public override void Initialize(EnemyDataSO enemyDataSO, Vector2 borderLinePos, Vector2 startPos, System.Action<string> changeAnimation)
    {
        base.Initialize(enemyDataSO, borderLinePos, startPos, changeAnimation);
        waitForInvulnerableTime = new WaitForSeconds(zombieData.InvulnerableTime);
    }

    IEnumerator InvulnerableRoutine()
    {
        yield return waitForInvulnerableTime;
        IsInvulnerable = false;
        invulnerableCoroutine = null;
        Debug.Log("Zombie is no longer invulnerable.");
    }

    protected override void UpdateMovement()
    {
        if(IsInvulnerable) return;        // 無敵状態のときは移動しない

        MoveByProgressToTarget();
        UpdateScale();
    }
    
}