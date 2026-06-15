using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public interface ISword {
    void Initialize(SwordDataSO data);
    SwordDataSO Data { get; }
}

public class SwordControl : MonoBehaviour, ISword
{

    bool isDragging = false;
    bool isThrown = false;
    Vector2 startPosition;
    TouchControl touch;
    SwordAttack swordAttack;
    SpriteRenderer spriteRenderer;

    float speed;
    Vector2 throwDir;
    Vector2 rotateDir;
    public float RotateAmount { get; private set; } = 0;
    float turnProgress = 0;
    float previwAngle = 0;
    float moveTime = 0f;

    public SwordDataSO Data { get; private set; }

    public bool IsNextTakeSword() => !isDragging && isThrown;

    void Awake()
    {
        swordAttack = GetComponent<SwordAttack>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(SwordDataSO data)
    {
        Data = data;
        swordAttack.Initialize(data);
        spriteRenderer.sprite = data.Icon;    // 剣の見た目を設定
    }

    void Update()
    {
        if (Touchscreen.current == null) return;
        touch = Touchscreen.current.primaryTouch;
        StartDrag();
        Dragging();
        EndDrag();

        Movement();
        Rotate();
    }

    /// <summary>
    /// 指をタップしたときにドラッグを開始する
    /// </summary>
    void StartDrag()
    {
        // 剣の周りを押下し、ドラッグしていない状態で、剣を飛ばしていないときにドラッグを開始する
        if (touch.press.isPressed && !isDragging && !isThrown && IsTouchOnSword())
        {
            startPosition = touch.position.ReadValue();
            isDragging = true;
        }
    }

    /// <summary>
    /// 指を動かしている間、剣を指の位置に追従させる
    /// </summary>
    void Dragging()
    {
        if (isDragging)
        {
            Vector2 cursor = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());

            GameManager.I.GetSwordArea(out Vector2 center, out Vector2 size);
            // 剣の位置が剣エリアからはみ出ないようにする
            Vector2 clampedPos = new Vector2(
                Mathf.Clamp(cursor.x, center.x - size.x / 2, center.x + size.x / 2),
                Mathf.Clamp(cursor.y, center.y - size.y / 2, center.y + size.y / 2)
            );
            transform.position = clampedPos;
        }
    }

    /// <summary>
    /// 指を離したときに剣を飛ばす
    /// </summary>
    void EndDrag()
    {
        if (!touch.press.isPressed && isDragging)
        {
            ThrowImpact();
            isDragging = false;
        }
    }

    /// <summary>
    /// 剣を飛ばす方向と回転量を計算する
    /// </summary>
    void ThrowImpact()
    {
        throwDir = touch.delta.ReadValue().normalized;
        
        speed = StatContext.I.SwordThrowForce();    // スワイプの距離に応じて剣の速度を決定

        isThrown = true;
    }

    void Movement()
    {
        if (!isThrown) return;
        float dt = Time.deltaTime;
        // turnAmountを1秒で目標値に近づくようにする
        turnProgress = Mathf.Lerp(turnProgress, RotateAmount, StatContext.I.SwordTurnReactTime() * dt);
        var pos = transform.position;
        pos += new Vector3(throwDir.x, throwDir.y, 0) * speed * dt;     // 剣を飛ばす方向に移動させる
        pos.x += turnProgress * -StatContext.I.SwordTurnForce() * dt;     // 回転量に応じて剣を横に動かす
        transform.position = pos;

        CheckDistant(dt);
    }

    /// <summary>
    /// 回転量に応じてずっと剣を回転させる
    /// </summary>
    void Rotate()
    {
        if (isThrown || isDragging)
        {
            // startPositionを中心に剣を回転させる
            Vector2 center = Camera.main.ScreenToWorldPoint(startPosition);
            rotateDir = (Vector2)transform.position - center;                 // 剣の位置と中心の位置から回転方向を計算する
            float currentAngle = Mathf.Atan2(rotateDir.y, rotateDir.x) * Mathf.Rad2Deg;     // 現在の角度を計算する
            float deltaAngle = Mathf.DeltaAngle(previwAngle, currentAngle);     // 前回の角度と現在の角度の差を計算する
            RotateAmount += deltaAngle * Time.deltaTime;      // 回転量を増加させる

            RotateAmount = Mathf.Clamp(RotateAmount, -StatContext.I.MaxRotationAmount(), StatContext.I.MaxRotationAmount());    // 回転量の最大値を設定する

            transform.Rotate(0, 0, RotateAmount);    // 剣を回転させる
            previwAngle = currentAngle;
        }
    }

    /// <summary>
    /// 剣の周りをタップしたときに攻撃する
    /// </summary>
    bool IsTouchOnSword()
    {
        float range = StatContext.I.SwordAttackRange();  // 剣の攻撃範囲を取得する
        Vector2 swordPos = transform.position;
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
        return Vector2.Distance(swordPos, touchPos) <= range;
    }

    /// <summary>
    /// 剣を飛ばしてから一定時間経ったら剣を破棄する
    /// </summary>
    void CheckDistant(float dt)
    {
        moveTime += dt;
        if(moveTime > 10f) Destroy(gameObject);
    }
}
