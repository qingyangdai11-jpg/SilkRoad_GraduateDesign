using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class scrollScale : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [Header("滑动的刻度尺")]
    public RectTransform ruler;

    [Header("要控制的图片")]
    public Image targetImage;
    public float maxScale = 2f;        // 最大缩放倍数
    public Vector2 maxPosition;        // 最大时的目标位置（在面板填写）

    [Header("刻度设置")]
    public float tickSpacing = 30f;
    public float snapSpeed = 12f;

    [Header("滑动边界限制")]
    public float leftLimit = -150f;   // 最小位置（原始大小+原始位置）
    public float rightLimit = 150f;   // 最大位置（最大缩放+目标位置）

    private float currentX;
    private float targetX;
    private bool isSnapping = false;

    private Vector2 startPos;         // 自动记录初始位置
    private float startScale = 1f;    // 初始缩放固定为1

    void Start()
    {
        // 记录初始位置（最小位置）
        startPos = targetImage.rectTransform.anchoredPosition;

        // 初始化滚轮停在最左侧
        currentX = leftLimit;
        targetX = leftLimit;
        ruler.anchoredPosition = new Vector2(currentX, ruler.anchoredPosition.y);

        // 初始化图片：缩放=1，位置=初始位置
        targetImage.rectTransform.localScale = Vector3.one;//Vector3.one，x,y,z都是一倍大小
        targetImage.rectTransform.anchoredPosition = startPos;
    }

    void Update()
    {
        if (isSnapping)
        {
            currentX = Mathf.Lerp(currentX, targetX, Time.deltaTime * snapSpeed);
            currentX = Mathf.Clamp(currentX, leftLimit, rightLimit);
            ruler.anchoredPosition = new Vector2(currentX, ruler.anchoredPosition.y);

            if (Mathf.Abs(currentX - targetX) < 0.5f)
                isSnapping = false;
        }

        UpdateScaleAndPosition();
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
        targetX = Mathf.Round(currentX / tickSpacing) * tickSpacing;
        targetX = Mathf.Clamp(targetX, leftLimit, rightLimit);
        isSnapping = true;
    }

    // 核心：同时控制 缩放 + 位置
    void UpdateScaleAndPosition()
    {
        // 获取滑动比例 0~1
        // 第一步：计算滚轮当前滑动的【百分比进度】
        // Mathf.InverseLerp(最小值, 最大值, 当前值) → 把当前位置映射成 0~1 的数字
        //算出滚轮滑了 “百分之几”
        float t = Mathf.InverseLerp(leftLimit, rightLimit, currentX);

        // 1. 缩放：1 → maxScale
        // 第二步：根据滑动进度 t, 计算图片应该缩放到多大
        // Mathf.Lerp(最小值, 最大值, 进度) → 线性插值计算大小
        //根据进度 t，算出图片该多大
        float finalScale = Mathf.Lerp(1f, maxScale, t);


        // 第三步：安全限制 → 强制让缩放值保持在 1 ~ maxScale 之间, 防止出现异常大小
        //保险！不让图片变太大或太小
        finalScale = Mathf.Clamp(finalScale, 1f, maxScale);

        // 2. 位置：初始位置 → 目标位置
        // 第四步：根据滑动进度 t, 计算图片应该移动到什么位置
        // Vector2.Lerp(起始位置, 目标位置, 进度) → 让图片在两个位置之间平滑过渡
        //根据进度 t，算出图片该移到哪儿
        Vector2 finalPos = Vector2.Lerp(startPos, maxPosition, t);

        // 应用到图片
        // 第五步：把计算好的【缩放值】应用到图片上
        // new Vector3(横向缩放, 纵向缩放, 深度) → 2D图片深度写1即可
        targetImage.rectTransform.localScale = new Vector3(finalScale, finalScale, 1);


        // 第六步：把计算好的【位置】应用到图片上
        // 让图片真正移动到我们算出来的目标坐标
        targetImage.rectTransform.anchoredPosition = finalPos;
    }

    // 判断是否滑到最右（缩放=2且到指定位置）
    public bool IsScaleComplete()
    {
        return Mathf.Approximately(currentX, rightLimit);
    }
}