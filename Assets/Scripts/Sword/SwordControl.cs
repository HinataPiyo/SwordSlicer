using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SwordControl : MonoBehaviour
{
    [SerializeField] float throwForce = 10f;

    [SerializeField] float turnForce = 2f;
    [SerializeField] float turnReactTime = 1.5f;

    bool isDragging = false;
    bool isThrown = false;
    Vector2 startPosition;
    TouchControl touch;
    SwordAttack swordAttack;

    float speed;
    Vector2 throwDir;
    Vector2 rotateDir;
    public float RotateAmount { get; private set; } = 0;
    float turnProgress = 0;
    float previwAngle = 0;

    void Awake()
    {
        swordAttack = GetComponent<SwordAttack>();
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
            transform.position = new Vector3(cursor.x, cursor.y);
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
        
        speed = 1 * throwForce;    // スワイプの距離に応じて剣の速度を決定

        isThrown = true;
    }

    void Movement()
    {
        if (!isThrown) return;
        float dt = Time.deltaTime;
        // turnAmountを1秒で目標値に近づくようにする
        turnProgress = Mathf.Lerp(turnProgress, RotateAmount, turnReactTime * dt);
        var pos = transform.position;
        pos += new Vector3(throwDir.x, throwDir.y, 0) * speed * dt;     // 剣を飛ばす方向に移動させる
        pos.x += turnProgress * turnForce * dt;     // 回転量に応じて剣を横に動かす
        transform.position = pos;
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

            transform.Rotate(0, 0, RotateAmount);    // 剣を回転させる
            previwAngle = currentAngle;
        }
    }

    /// <summary>
    /// 剣の周りをタップしたときに攻撃する
    /// </summary>
    bool IsTouchOnSword()
    {
        float range = swordAttack.AttackRange;
        Vector2 swordPos = transform.position;
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
        return Vector2.Distance(swordPos, touchPos) <= range;
    }
}
