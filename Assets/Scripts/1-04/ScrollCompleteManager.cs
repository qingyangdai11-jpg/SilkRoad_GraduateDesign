using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollCompleteManager : MonoBehaviour
{
    [Header("两个滚动条脚本")]
    public scrollTransparent transparentScroll;
    public scrollScale scaleScroll;

    [Header("要隐藏的滚动条父物体")]
    public GameObject scroll01;
    public GameObject scroll02;

    [Header("BG 的 ScrollRect")]
    public ScrollRect bgScrollRect;

    [Header("要渐变显示的 Port 和 Button")]
    public CanvasGroup portImage;    // 拖入 port 图片的 CanvasGroup
    public CanvasGroup button;       // 拖入 button 的 CanvasGroup

    [Header("渐变设置")]
    public float waitBeforeFade = 1f;      // 滚动条完成后等待1秒
    public float scrollFadeDuration = 0.5f;// 滚动条渐变消失时长
    public float showFadeDuration = 0.5f;  // port/button 渐变出现时长

    private bool isFading = false;

    void Update()
    {
        if (isFading) return;

        bool transparentDone = transparentScroll != null && transparentScroll.IsTransparentComplete();
        bool scaleDone = scaleScroll != null && scaleScroll.IsScaleComplete();

        if (transparentDone && scaleDone)
        {
            SceneAudioController.Instance.PlayDragEnd(true);
            isFading = true;
            StartCoroutine(FadeScrollsAndShowPort());
        }
    }

    IEnumerator FadeScrollsAndShowPort()
    {
        // 1. 先等待指定时间
        yield return new WaitForSeconds(waitBeforeFade);

        // 2. 获取滚动条的 CanvasGroup（自动添加）
        CanvasGroup group01 = scroll01.GetComponent<CanvasGroup>();
        CanvasGroup group02 = scroll02.GetComponent<CanvasGroup>();

        // 3. 滚动条渐变消失（alpha 1 → 0）
        float timer = 0f;
        while (timer < scrollFadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1 - (timer / scrollFadeDuration);
            group01.alpha = alpha;
            group02.alpha = alpha;
            yield return null;
        }
        group01.alpha = 0f;
        group02.alpha = 0f;
        scroll01.SetActive(false);
        scroll02.SetActive(false);

        // 4. Port 和 Button 渐变出现（alpha 0 → 1）
        timer = 0f;
        while (timer < showFadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / showFadeDuration;
            portImage.alpha = alpha;
            button.alpha = alpha;
            yield return null;
        }
        portImage.alpha = 1f;
        button.alpha = 1f;

        // 5. 解锁 BG 的 ScrollRect
            bgScrollRect.enabled = true;

        // 6. 禁用自己.所有过渡完成后，禁用自身脚本，避免后续 Update 继续执行
        enabled = false;
    }
}