using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public interface ISword {
    void Initialize(BattleSettingConfig.SwordDataByType data);
    BattleSettingConfig.SwordDataByType Data { get; }
}

public class SwordControl : MonoBehaviour, ISword
{
    struct DragSample
    {
        public Vector2 Position;
        public float Time;

        public DragSample(Vector2 position, float time)
        {
            Position = position;
            Time = time;
        }
    }

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
    const float ReferenceFps = 120f;
    const float DragVectorToRotateAmount = 2f;

    // ドラッグの動きをサンプリングする時間間隔と、サンプルを保持する時間
    const float ThrowDirectionSampleWindow = 0.1f;
    const float DragSampleKeepSeconds = 0.3f;
    readonly List<DragSample> dragSamples = new();      // ドラッグの位置と時間のサンプルを記録するリスト

    public BattleSettingConfig.SwordDataByType Data { get; private set; }

    public bool IsNextTakeSword() => !isDragging && isThrown;

    /// <summary>
    /// UIに表示する用で剣の速度を取得する。
    /// 剣が飛んでいる場合はそのままの速度を返す。ドラッグ中の場合は、ドラッグ時間に応じた速度を返す。
    /// </summary>
    public float GetDisplaySpeed()
    {
        if (isThrown)
        {
            return Speed;
        }

        if(!isDragging)
        {
            return 0f;
        }

        return CalculateThrowSpeed(draggingTime);
    }

    /// <summary>
    /// UIに表示する用で剣の回転量を取得する。
    /// 飛んでいる場合はそのままの回転量を返す。ドラッグ中の場合は、ドラッグ時間に応じた回転量を返す。
    /// </summary>
    public float GetDisplayCurvePower()
    {
        float activeRotateAmount = isThrown ? turnAmount : RotateAmount;
        return activeRotateAmount * -ServiceLocator.Get<IStateService>().SwordTurnForce();
    }

    void Awake()
    {
        swordAttack = GetComponent<SwordAttack>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(BattleSettingConfig.SwordDataByType data)
    {
        pointAction?.action?.Enable();
        pressAction?.action?.Enable();
        
        Data = data;
        swordAttack.Initialize(data);
        spriteRenderer.sprite = data.swordDataSO.Icon;    // 剣の見た目を設定
        transform.localScale = Vector3.one * ServiceLocator.Get<IStateService>().SwordAttackRange();    // 剣のサイズを設定
    }

    void Update()
    {
        deltaTime = Time.deltaTime;

        bool isPressed = pressAction.action.IsPressed();
        Vector2 point = pointAction.action.ReadValue<Vector2>();

        StartDrag(point, isPressed);
        Dragging(point);
        EndDrag(isPressed);

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
            dragSamples.Clear();
            isDragging = true;
        }
    }

    /// <summary>
    /// 指を動かしている間、剣を指の位置に追従させる
    /// </summary>
    void Dragging(Vector2 point)
    {
        if (isDragging && pressAction.action.IsPressed())
        {
            Vector2 cursor = Camera.main.ScreenToWorldPoint(point);

            ServiceLocator.Get<ISwordDraggingArea>().GetSwordArea(out Vector2 center, out Vector2 size);
            // 剣の位置が剣エリアからはみ出ないようにする
            Vector2 clampedPos = new Vector2(
                Mathf.Clamp(cursor.x, center.x - size.x / 2, center.x + size.x / 2),
                Mathf.Clamp(cursor.y, center.y - size.y / 2, center.y + size.y / 2)
            );
            transform.position = clampedPos;
            RecordDragSample(clampedPos);
            draggingTime += deltaTime;
        }
    }

    /// <summary>
    /// 指を離したときに剣を飛ばす
    /// </summary>
    void EndDrag(bool isPressed)
    {
        if (!isPressed && isDragging)
        {
            ThrowImpact();
            pointAction?.action?.Disable();
            pressAction?.action?.Disable();
            isDragging = false;
        }
    }

    /// <summary>
    /// 剣を飛ばす方向と回転量を計算する
    /// </summary>
    void ThrowImpact()
    {
        // 離す直前の0.1秒前の位置と現在の位置からドラッグ方向を計算する
        Vector2 dragVector = GetRecentDragVector();
        throwDir = dragVector.sqrMagnitude > 0.0001f ? dragVector.normalized : Vector2.up;

        // 直線ドラッグでは角度差が出にくいため、投擲時にドラッグベクトルから回転量を補完する
        float fallbackRotateAmount = dragVector.x * DragVectorToRotateAmount;
        RotateAmount += fallbackRotateAmount;
        RotateAmount = Mathf.Clamp(RotateAmount, -ServiceLocator.Get<IStateService>().MaxRotateAmount(), ServiceLocator.Get<IStateService>().MaxRotateAmount());
        

        // ドラッグ時間が長いほど剣の速度を速くする(最小0.5、最大2の速度にする)
        Speed = CalculateThrowSpeed(draggingTime);

        isThrown = true;
    }

    /// <summary>
    /// ドラッグ時間に応じて剣の速度を計算する
    /// </summary>
    float CalculateThrowSpeed(float dragTime)
    {
        return Mathf.Clamp(dragTime * 0.5f, 0.5f, 2f) * ServiceLocator.Get<IStateService>().SwordThrowForce();
    }

    /// <summary>
    /// ドラッグのサンプルを記録する。古いサンプルは一定時間経ったら削除する
    /// </summary>
    void RecordDragSample(Vector2 worldPosition)
    {
        float now = Time.time;
        dragSamples.Add(new DragSample(worldPosition, now));

        float minTime = now - DragSampleKeepSeconds;

        // 古いサンプルを削除する
        while (dragSamples.Count > 0 && dragSamples[0].Time < minTime)
        {
            dragSamples.RemoveAt(0);
        }
    }

    /// <summary>
    /// ドラッグの最近の動きを元に、剣を飛ばす方向を計算する
    /// </summary>
    Vector2 GetRecentDragVector()
    {
        // ドラッグのサンプルがない場合は、剣の位置とドラッグ開始位置から方向を計算する
        if (dragSamples.Count == 0)
        {
            return (Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(startPosition);
        }

        float targetTime = Time.time - ThrowDirectionSampleWindow;
        Vector2 currentPos = dragSamples[dragSamples.Count - 1].Position;
        Vector2 pastPos = dragSamples[0].Position;

        // ドラッグのサンプルから、targetTimeに最も近い過去の位置を見つける
        for (int i = dragSamples.Count - 1; i >= 0; i--)
        {
            if (dragSamples[i].Time <= targetTime)
            {
                pastPos = dragSamples[i].Position;
                break;
            }
        }

        return currentPos - pastPos;        // 現在の位置と過去の位置の差分からドラッグ方向を計算する
    }

    void Movement()
    {
        if (!isThrown) return;
        // ReactTimeを「秒」として扱うため、指数補間でFPS非依存に追従させる
        float reactTime = Mathf.Max(0.0001f, ServiceLocator.Get<IStateService>().SwordTurnReactTime());
        float blend = 1f - Mathf.Exp(-deltaTime / reactTime);
        turnAmount = Mathf.Lerp(turnAmount, RotateAmount, blend);

        // 回転量に応じて進行方向を少しずつ回し、自然なカーブを作る
        // Speed を掛けることで角速度∝速度にし、旋回半径を速度によらず一定に保つ
        float turnRate = GetTurnEffect();
        throwDir = (Vector2)(Quaternion.Euler(0f, 0f, -turnRate * Speed * deltaTime) * throwDir);
        throwDir = throwDir.sqrMagnitude > 0.0001f ? throwDir.normalized : Vector2.up;

        var pos = transform.position;
        pos += new Vector3(throwDir.x, throwDir.y, 0) * Speed * deltaTime;     // 剣を飛ばす方向に移動させる
        transform.position = pos;

        CheckDistant();
    }

    /// <summary>
    /// 回転量に応じて剣を横に動かす量を計算する
    /// </summary>
    public float GetTurnEffect()
    {
        // 回転量に応じた「進行方向の旋回速度」を返す
        return turnAmount * -ServiceLocator.Get<IStateService>().SwordTurnForce();
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
            // 60fps時の体感を基準にしつつ、FPS差による挙動変化を抑える
            RotateAmount += deltaAngle / ReferenceFps;      // 回転量を増加させる

            RotateAmount = Mathf.Clamp(RotateAmount, -ServiceLocator.Get<IStateService>().MaxRotateAmount(), ServiceLocator.Get<IStateService>().MaxRotateAmount());    // 回転量の最大値を設定する

            transform.Rotate(0, 0, RotateAmount * (deltaTime * ReferenceFps));    // 剣を回転させる
            previwAngle = currentAngle;
        }
    }

    /// <summary>
    /// 剣の周りをタップしたときに攻撃する
    /// </summary>
    bool IsTouchOnSword(Vector2 point)
    {
        float range = ServiceLocator.Get<IStateService>().SwordAttackRange();  // 剣の攻撃範囲を取得する
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
