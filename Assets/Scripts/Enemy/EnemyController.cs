using UnityEngine;

public interface IEnemy {
    EnemyDataSO Data { get; }
    void Initialize(EnemyDataSO enemyData);
    void Tick();
}

public class EnemyController : MonoBehaviour
{
    
    [SerializeField] EnemyDataSO enemyDataSO;

    protected EnemyMovement movement;
    protected EnemyHealth health;

    Animator animator;

    public EnemyDataSO Data => enemyDataSO;


    protected virtual void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 敵が出現する際の初期化処理
    /// </summary>
    public void Initialize(Vector2 borderLinePos, float enemyStatusMultiplier)
    {
        movement.Initialize(enemyDataSO, borderLinePos, ChangeAnimationState);
        health.Initialize(enemyDataSO, enemyStatusMultiplier);

        RegisterDieAnimation();     // 死亡アニメーションを登録
    }

    void Update()
    {
        if(health.IsDead) return;    // 死亡している場合は移動しない

        // 敵の移動とその他の処理を更新
        movement.Tick();
    }

    protected void ChangeAnimationState(string stateName)
    {
        if (animator == null) return;

        animator.SetTrigger(stateName);
    }

    protected void ChangeAnimationState(string stateName, bool isLooping)
    {
        if (animator == null) return;

        animator.SetBool(stateName, isLooping);
    }

    /// <summary>
    /// 敵が削除されるときのコールバックを登録する
    /// </summary>
    public void RegisterDestroy(System.Action onRemove) => health.RegisterDestroy(onRemove);

    void RegisterDieAnimation() => health.RegisterDieAnimation(() => animator.SetTrigger("Die"));
}