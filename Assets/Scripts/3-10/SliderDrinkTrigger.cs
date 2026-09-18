using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // 必须引用

public class SliderDrinkTrigger : MonoBehaviour
{
    [Header("引用组件")]
    public Slider targetSlider;       // 拖拽你的 Slider 到这里
    public Animator drinkAnimator;    // 拖拽你的 02Ani 对象到这里

    public Button nextBu;

    [Header("设置")]
    public float triggerThreshold = 0.99f;  // 触发阈值（越接近1越靠满值，用0.99做容错）
    private bool hasPlayed = false;         // 防止动画重复播放

    void Update()
    {
        // 如果动画已经播放过一次，就不再检测
        if (hasPlayed) return;

        // slider.value 的范围是 0 到 1
        // 1 代表拉满最大值，0 代表最小值
        if (targetSlider.value >= triggerThreshold)
        {
            PlayDrinkAnimation();
        }
    }

    void PlayDrinkAnimation()
    {
        hasPlayed = true;  // 标记为已播放
        drinkAnimator.SetTrigger("PlayDrink");  // 触发动画（需要在Animator中设置名为"PlayDrink"的Trigger参数）

        // 如果你没有设置Trigger参数，也可以直接播放动画剪辑名称：
        // drinkAnimator.Play("drink");
        targetSlider.GetComponent<CanvasGroup>()
            .DOFade(0, 1f);

        nextBu.GetComponent<CanvasGroup>()
            .DOFade(1, 1f);
           
    }
}