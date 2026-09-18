using UnityEngine;
using UnityEngine.UI;

public class ScrollTrigger : MonoBehaviour
{
    [Header("引用组件")]
    public ScrollRect scrollRect; // 拖拽你的 Scroll View 到这里
    public Animator seaAnimator;  // 拖拽你的 Sea 对象到这里

    [Header("设置")]
    public float triggerThreshold = 0.01f; // 触发阈值（越接近0越靠近底部）
    private bool hasPlayed = false;         // 防止动画重复播放

    void Update()
    {
        // 如果动画已经播放过一次，就不再检测
        if (hasPlayed) return;

        // scrollRect.verticalNormalizedPosition 的范围是 0 到 1
        // 1 代表在最顶部，0 代表在最底部
        if (scrollRect.verticalNormalizedPosition <= triggerThreshold)
        {
            PlaySeaAnimation();
        }
    }
    void PlaySeaAnimation()
    {
        hasPlayed = true; // 标记为已播放
        seaAnimator.SetTrigger("PlaySea"); // 触发动画（需要在Animator中设置名为"PlaySea"的Trigger参数）

        // 如果你没有设置Trigger参数，也可以直接播放动画剪辑名称：
        // seaAnimator.Play("SeaAnimation");
    }
}