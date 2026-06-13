using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))] // メインカメラそのものにアタッチしたほうがわかりやすそう
public class AdjustAspectRatio : MonoBehaviour
{
    [SerializeField] private Vector2 aspectVec; //目的解像度

    [SerializeField] private float referenceWidth = 1080f; //基準解像度
    [SerializeField] private float pixelsPerUnit = 100f; // 1ユニットあたりのピクセル数

    [SerializeField] private float targetAspectRatio = 9f / 16f; // 目標アスペクト比

    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void Start()
    {
        // 初期化時にアスペクト比を調整するパターン
        // AdjustAspectRatioByViewport();
        // AutoOrthoSize();
        // SetCameraAspect();
    }

    void Update()
    {
        // Updateは毎フレーム呼ばれるので、実行する必要があるかどうかを確認
        AdjustAspectRatioByViewport(); //アスペクト比を調整
    }

    // カメラのアスペクト比を調整するメソッド
    // 画面のアスペクト比に合わせてカメラのViewportを調整する
    void AdjustAspectRatioByViewport()
    {
        var screenAspect = Screen.width / (float)Screen.height; //画面のアスペクト比
        var targetAspect = aspectVec.x / aspectVec.y; //目的のアスペクト比

        var magRate = targetAspect / screenAspect; //目的アスペクト比にするための倍率

        var viewportRect = new Rect(0, 0, 1, 1); //Viewport初期値でRectを作成

        if (magRate < 1)
        {
            viewportRect.width = magRate; //使用する横幅を変更
            viewportRect.x = 0.5f - viewportRect.width * 0.5f;//中央寄せ
        }
        else
        {
            viewportRect.height = 1 / magRate; //使用する縦幅を変更
            viewportRect.y = 0.5f - viewportRect.height * 0.5f;//中央余生
        }

        _cam.rect = viewportRect; //カメラのViewportに適用
    }

    // カメラのOrthographicSizeを自動調整するメソッド
    void AutoOrthoSize()
    {
        float aspect = (float)Screen.width / (float)Screen.height; //画面のアスペクト比
        _cam.orthographicSize = referenceWidth / (2f * pixelsPerUnit * aspect); //カメラのOrthographicSizeを調整
    }

    // カメラのアスペクト比を調整するメソッド
    void SetCameraAspect()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height; //ウィンドウのアスペクト比
        float scaleHeight = windowAspect / targetAspectRatio; //ウィンドウのアスペクト比と目標アスペクト比の比率
        if (scaleHeight < 1.0f)
        {
            Rect rect = _cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            _cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = _cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            _cam.rect = rect;
        }
    }
}
