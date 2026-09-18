using UnityEngine;
using UnityEngine.UI;

public class PaintFade : MonoBehaviour
{
    [Header("涂抹设置")]
    public float fullFadeTime = 5f;
    public BackgroundScroll bgScroll;

    private Image img;
    private RectTransform rt;
    private float alpha = 0f;
    private bool isDone = false;

    void Awake()
    {
        img = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
    }

    void Update()
    {
        // 只有背景【暂停】了才能涂
        if (bgScroll.isScrolling) return;
        if (isDone) return;

        // ==========================
        // 核心修复：鼠标是否在 trans 图片上
        // ==========================
        bool inRect = RectTransformUtility.RectangleContainsScreenPoint(
            rt,
            Input.mousePosition,
            null // UI 必须填 null！这是你没反应的关键原因！
        );

        if (inRect)
        {
            // 鼠标在上面 → 透明度增加
            alpha += Time.deltaTime / fullFadeTime;
            alpha = Mathf.Clamp01(alpha);
            img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);

            // 满了 → 继续滚动
            if (alpha >= 1)
            {
                isDone = true;
                bgScroll.ContinueScroll();
                Debug.Log("✅ 涂抹完成，背景继续滚动");
            }
        }
    }
}