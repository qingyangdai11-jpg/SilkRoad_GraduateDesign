using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BG_MoveLeft_DOTween : MonoBehaviour
{
    [Header("背景移动设置")]
    public float waitTime ;       // 等待2秒
    public float moveDuration = 3f;  // 背景移动总时长
    public float targetPosX = -1346.8f; // 目标X位置

    [Header("渐显元素设置")]
    public Graphic[] fadeElements;   // 按顺序拖入：02、03、04Ani、Bar_Red
    public float fadeSpeed =1f;   // 单个元素渐显时长
    public float fadeInterval = 1f;  // 元素间的渐显间隔

    private RectTransform bgRect;
    private Sequence mainSeq;

    void Start()
    {
        bgRect = GetComponent<RectTransform>();

        // 1. 初始化所有元素为完全透明
        foreach (var item in fadeElements)
        {
            if (item != null)
                item.color = new Color(item.color.r, item.color.g, item.color.b, 0);
        }

        // 2. 创建主动画序列
        mainSeq = DOTween.Sequence();

        // 步骤1：先等待2秒（无任何动作）
        mainSeq.AppendInterval(waitTime);

        // --------------------------
        // 关键修改：等待结束后，同时启动两个动画
        // --------------------------
        // ① 背景移动动画（和渐显序列同时开始）
        mainSeq.Append(bgRect.DOAnchorPosX(targetPosX, moveDuration)
                          .SetEase(Ease.Linear));

        // ② 渐显序列（做成独立子序列，和背景移动同步启动）
        Sequence fadeSequence = DOTween.Sequence();
        // 第一个元素：和序列同时开始渐显
        fadeSequence.AppendInterval(5f);
        fadeSequence.Join(fadeElements[0].DOFade(1, fadeSpeed));
        // 第二个元素：间隔1秒后渐显
        fadeSequence.AppendInterval(8f);
        fadeSequence.Join(fadeElements[1].DOFade(1, fadeSpeed));
        
        fadeSequence.Join(fadeElements[2].DOFade(1, fadeSpeed));
        
        // 把渐显子序列加入主序列，和背景移动同时启动
        mainSeq.Join(fadeSequence);
    }

    // 防止场景切换时动画报错
    void OnDestroy()
    {
        mainSeq?.Kill();
    }
}