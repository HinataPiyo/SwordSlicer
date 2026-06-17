using UnityEngine;
using UnityEngine.InputSystem;

public interface ISword {
    void Initialize(SwordDataSO data);
    SwordDataSO Data { get; }
}

public class SwordControl : MonoBehaviour, ISword
{

    [SerializeField] InputActionReference pointAction;

    [SerializeField] InputActionReference pressAction;

    bool isDragging = false;
    bool isThrown = false;
    Vector2 startPosition;
    SwordAttack swordAttack;
    SpriteRenderer spriteRenderer;

    Vector2 throwDir;
    Vector2 rotateDir;
    public float Speed { get; private set; } = 0;
    public float RotateAmount { get; private set; } = 0;
    float turnAmount = 0;
    float previwAngle = 0;
    float moveTime = 0f;
    float draggingTime = 0f;
    float deltaTime = 0f;

    public SwordDataSO Data { get; private set; }

    public bool IsNextTakeSword() => !isDragging && isThrown;

    void Awake()
    {
        swordAttack = GetComponent<SwordAttack>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        pointAction?.action?.Enable();
        pressAction?.action?.Enable();
    }

    public void Initialize(SwordDataSO data)
    {
        Data = data;
        swordAttack.Initialize(data);
        spriteRenderer.sprite = data.Icon;    // 剣の見た目を設定
    }

    void Update()
    {
        deltaTime = Time.deltaTime;

        bool isPressed = pressAction.action.IsPressed();
        Vector2 point = pointAction.action.ReadValue<Vector2>();

        StartDrag(point, isPressed);
        Dragging(point);
        EndDrag(point, isPressed);

        Movement();
        Rotate();
    }

    /// <summary>
    /// 指をタップしたときにドラッグを開始する
    /// </summary>
    void StartDrag(Vector2 point, bool isPressed)
    {
        if (!isPressed) return;

        // 剣の周りを押下し、ドラッグしていない状態で、剣を飛ばしていないときにドラッグを開始する
        if (!isDragging && !isThrown && IsTouchOnSword(point))
        {
            startPosition = point;
            draggingTime = 0f;
            isDragging = true;
        }
    }

    /// <summary>
    /// 指を動かしている間、剣を指の位置に追従させる
    /// </summary>
    void Dragging(Vector2 point)
    {
        if (isDragging)
        {
            Vector2 cursor = Camera.main.ScreenToWorldPoint(point);

            GameManager.I.GetSwordArea(out Vector2 center, out Vector2 size);
            // 剣の位置が剣エリアからはみ出ないようにする
            Vector2 clampedPos = new Vector2(
                Mathf.Clamp(cursor.x, center.x - size.x / 2, center.x + size.x / 2),
                Mathf.Clamp(cursor.y, center.y - size.y / 2, center.y + size.y / 2)
            );
            transform.position = clampedPos;
            draggingTime += deltaTime;
        }
    }

    /// <summary>
    /// 指を離したときに剣を飛ばす
    /// </summary>
    void EndDrag(Vector2 point, bool isPressed)
    {
        if (!isPressed && isDragging)
        {
            ThrowImpact(point);
            pointAction?.action?.Disable();
            pressAction?.action?.Disable();
            isDragging = false;
        }
    }

    /// <summary>
    /// 剣を飛ばす方向と回転量を計算する
    /// </summary>
    void ThrowImpact(Vector2 point)
    {
        Vector2 dragVector = point - startPosition;
        throwDir = dragVector.sqrMagnitude > 0.0001f ? dragVector.normalized : Vector2.up;
        

        // ドラッグ時間が長いほど剣の速度を速くする(最小0.5、最大2の速度にする)
        Speed = Mathf.Clamp(draggingTime * 0.5f, 0.5f, 2f) * StatContext.I.SwordThrowForce();

        isThrown = true;
    }

    void Movement()
    {
        if (!isThrown) return;
        // turnAmountを1秒で目標値に近づくようにする
        turnAmount = Mathf.Lerp(turnAmount, RotateAmount, StatContext.I.SwordTurnReactTime() * deltaTime);
        var pos = transform.position;
        pos += new Vector3(throwDir.x, throwDir.y, 0) * Speed * deltaTime;     // 剣を飛ばす方向に移動させる
        pos.x += GetTurnEffect();     // 回転量に応じて剣を横に動かす
        transform.position = pos;

        CheckDistant();
    }

    public float GetTurnEffect()
    {
        // 回転量に応じて剣を横に動かす量を計算する
        return turnAmount * -StatContext.I.SwordTurnForce() * deltaTime;
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
    bool IsTouchOnSword(Vector2 point)
    {
        float range = StatContext.I.SwordAttackRange();  // 剣の攻撃範囲を取得する
        Vector2 swordPos = transform.position;
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(point);
        return Vector2.Distance(swordPos, touchPos) <= range;
    }

    /// <summary>
    /// 剣を飛ばしてから一定時間経ったら剣を破棄する
    /// </summary>
    void CheckDistant()
    {
        moveTime += deltaTime;
        if(moveTime > 5f) Destroy(gameObject);
    }
}
