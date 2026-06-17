using UnityEngine;

public interface IEnemy {
    EnemyDataSO Data { get; }
    void Initialize(EnemyDataSO enemyData);
    void Tick();
}

public class EnemyController : MonoBehaviour
{
    
    [SerializeField] EnemyDataSO enemyDataSO;
    [SerializeField] SpriteRenderer spriteRenderer;

    EnemyMovement movement;
    EnemyHealth health;

    Animator animator;


    void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 敵が出現する際の初期化処理
    /// </summary>
    public void Initialize(Vector2 borderLinePos, int sortingOrder)
    {
        movement.Initialize(enemyDataSO, borderLinePos, ChangeAnimationState);
        health.Initialize(enemyDataSO);
        spriteRenderer.sortingOrder = sortingOrder;    // ソートオーダーを設定

        RegisterDieAnimation();     // 死亡アニメーションを登録
    }

    void Update()
    {
        if(health.IsDead) return;    // 死亡している場合は移動しない

        // 敵の移動とその他の処理を更新
        movement.Tick();
    }

    void ChangeAnimationState(string stateName)
    {
        if (animator == null) return;

        animator.SetTrigger(stateName);
    }

    /// <summary>
    /// 敵が削除されるときのコールバックを登録する
    /// </summary>
    public void RegisterDestroy(System.Action onRemove) => health.RegisterDestroy(onRemove);

    void RegisterDieAnimation() => health.RegisterDieAnimation(() => animator.SetTrigger("Die"));
}