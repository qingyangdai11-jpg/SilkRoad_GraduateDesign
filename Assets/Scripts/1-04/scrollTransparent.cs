using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class scrollTransparent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("滑动的刻度尺")]
    public RectTransform ruler;

    [Header("要控制的图片")]
    public Image targetImage;

    [Header("刻度设置")]
    public float tickSpacing = 30f;
    public float snapSpeed = 12f;

    [Header("滑动边界限制")]
    public float leftLimit = -150f;   // 最左 = Alpha 220
    public float rightLimit = 150f;   // 最右 = Alpha 0

    private float currentX;
    private float targetX;
    private bool isSnapping = false;

    void Start()
    {
        // 初始化滚轮在最左侧（对应Alpha=220）
        currentX = leftLimit;
        targetX = leftLimit;
        ruler.anchoredPosition = new Vector2(currentX, ruler.anchoredPosition.y);

        // 初始化图片透明度 = 220
        Color startColor = targetImage.color;
        startColor.a = 220f / 255f;
        targetImage.color = startColor;

    }

    void Update()
    {
        if (isSnapping)//吸附动画
        {
            currentX = Mathf.Lerp(currentX, targetX, Time.deltaTime * snapSpeed);
            currentX = Mathf.Clamp(currentX, leftLimit, rightLimit);
            ruler.anchoredPosition = new Vector2(currentX, ruler.anchoredPosition.y);

            if (Mathf.Abs(currentX - targetX) < 0.5f)
                isSnapping = false;
        }

        UpdateOpacity();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isSnapping = false;
        SceneAudioController.Instance.PlayDragStart();
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentX += eventData.delta.x;
        currentX = Mathf.Clamp(currentX, leftLimit, rightLimit);
        ruler.anchoredPosition = new Vector2(currentX, ruler.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        targetX = Mathf.Round(currentX / tickSpacing) * tickSpacing;//round四舍五入取整函数，找到离当前滚轮最近的刻度位置
        targetX = Mathf.Clamp(targetX, leftLimit, rightLimit);
        isSnapping = true;//自动吸附
    }

    void UpdateOpacity()//实时更新透明度
    {
        // 计算位置比例：最左=1(220)，最右=0(0)
        float t = Mathf.InverseLerp(leftLimit, rightLimit, currentX);

        // 核心：透明度从 220 → 0
        float alpha = (1 - t) * (220f / 255f);
        alpha = Mathf.Clamp01(alpha);

        Color c = targetImage.color;
        c.a = alpha;
        targetImage.color = c;
    }

    // 判断是否滑到最右（透明度=0）
    public bool IsTransparentComplete()
    {
        // 用 Approximately 避免浮点误差，判断 currentX 是否等于 rightLimit
        return Mathf.Approximately(currentX, rightLimit);
    }
}