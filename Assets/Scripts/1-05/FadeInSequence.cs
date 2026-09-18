using UnityEngine;
// 引入 DOTween 命名空间
using DG.Tweening;

public class FadeInSequence : MonoBehaviour
{
    [Header("要渐变的UI物体")]
    // 拖入 port 物体（带CanvasGroup）
    public CanvasGroup portCanvasGroup;
    // 拖入 talk 物体（带CanvasGroup）
    public CanvasGroup talkCanvasGroup;

    [Header("渐变动画参数")]
    // 单张图片的渐变时长（秒），可在Inspector调整
    public float fadeDuration = 1f;
    // 两张图片之间的间隔时间（秒），0表示无缝衔接
    public float interval = 0.5f;

    void Start()
    {
        // 场景启动后，执行依次渐变的动画序列
        PlayFadeInSequence();
    }

    void PlayFadeInSequence()
    {
        // 1. 先重置两张图片的透明度为0（保险操作，避免误改初始值）
        portCanvasGroup.alpha = 0;
        talkCanvasGroup.alpha = 0;

        // 2. 创建 DOTween 动画序列，实现「依次执行」
        Sequence fadeSequence = DOTween.Sequence();

        // 第一步：port 从0渐变到1（完全显示）
        fadeSequence.Append(portCanvasGroup.DOFade(1f, fadeDuration));

        // 第二步：添加间隔时间（等待interval秒后再执行下一步）
        fadeSequence.AppendInterval(interval);

        // 第三步：talk 从0渐变到1（完全显示）
        fadeSequence.Append(talkCanvasGroup.DOFade(1f, fadeDuration));

        // 3. 启动动画序列
        fadeSequence.Play();
    }
}