using UnityEngine;
using DG.Tweening; // 必须引用

public class ImageFadeIn : MonoBehaviour
{
    // 把 4 张图片拖进这里
    public UnityEngine.UI.Image[] images;

    // 每张图片淡入时间
    public float fadeDuration = 1f;

    // 上一张结束后，延迟多久显示下一张
    public float delayBetween = 0.5f;

    void Start()
    {
        PlayFadeIn();
    }

    void PlayFadeIn()
    {
        // 先把所有图片设为透明
        foreach (var img in images)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        }

        // 创建 DOTween 序列
        Sequence seq = DOTween.Sequence();

        // 依次添加淡入动画
        foreach (var img in images)
        {
            // 图片透明度从 0 → 1，用时 fadeDuration
            seq.Append(img.DOFade(1, fadeDuration));

            // 每张之间加延迟
            seq.AppendInterval(delayBetween);
        }
    }
}