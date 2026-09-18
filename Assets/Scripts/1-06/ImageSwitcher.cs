using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageSwitcher : MonoBehaviour
{
    [Header("按顺序放入6张图片")]
    public Image[] pics;

    [Header("固定配置不要改")]
    private readonly int totalImg = 6;
    private readonly float oneLoopAngle = 360f;     // 1整圈=1张图
    private readonly float overlapRate = 0.5f;     // 两张50%重叠
    private readonly float totalFullAngle = 1800f;
    private float crossFadeAngle;//单张图覆盖的角度范围

    private Tween[] fadeTweens;

    void Start()
    {
        // 校验必须6张
        if (pics.Length != totalImg)
        {
            Debug.LogError("必须严格放6张图片！");
            return;
        }

        crossFadeAngle = oneLoopAngle * overlapRate;//两张图重叠渐变的角度大小
        fadeTweens = new Tween[totalImg];

        // 初始化：第一张亮，其他全黑
        for (int i = 0; i < totalImg; i++)
        {
            Color c = pics[i].color;
            c.a = i == 0 ? 1f : 0f;
            pics[i].color = c;
        }
    }

    // 由钟表脚本实时推送角度
    public void UpdateByAngle(float nowTotalAngle)//传入角度
    {
        // 走完5圈：强制定格最后一张α=1
        bool isFinish = nowTotalAngle >= totalFullAngle - 5f;//看看当前旋转的总角度，是不是 ** 快到最大限制（5 圈）** 了

        for (int i = 0; i < totalImg; i++)
        {
            if (isFinish)
            {
                KillTween(i);
                pics[i].color = new Color(1, 1, 1, i == 5 ? 1f : 0f);
                continue;
            }

            // 第i张图：占用一整圈360°
            float imgStart = i * oneLoopAngle;//给每张图分配一段角度，第一张图360°
            float imgEnd = (i + 1) * oneLoopAngle;//第二张图是720°

            float alpha = CalcCurrentAlpha(nowTotalAngle, imgStart, imgEnd);//调用函数，根据当前转到多少度算出这张图的透明度（0~1）
            SetImageFade(pics[i], alpha, i);//平滑渐变
        }
    }

    // 计算单张图透明度（带50%重叠渐变）
    private float CalcCurrentAlpha(float now, float start, float end)
    {
        // 淡入开始位置
        float fadeInArea = start - crossFadeAngle;
        // 淡出结束位置
        float fadeOutArea = end + crossFadeAngle;

        // 1. 完全不在范围内 → 透明
        if (now < fadeInArea || now > fadeOutArea)
            return 0f;

        // 2. 正好在图片核心区间 → 完全显示
        if (now >= start && now <= end)
            return 1f;

        // 3. 还没到核心区 → 正在淡入（从0慢慢变1）
        if (now < start)
            return Mathf.InverseLerp(fadeInArea, start, now);

        // 4. 已经过了核心区 → 正在淡出（从1慢慢变0）
        return Mathf.InverseLerp(fadeOutArea, end, now);
    }
    //    超级通俗翻译：
    //now：现在转到多少度了
    //start ~end：这张图该完全显示的角度区间
    //fadeInArea：提前开始淡入的位置
    //fadeOutArea：延后淡出的位置
    //逻辑像这样：
    //还没到 → 看不见（0）
    //快到了 → 慢慢出现（淡入）
    //正中间 → 完全显示（1）
    //走过了 → 慢慢消失（淡出）
    //走太远 → 看不见（0）
    //Mathf.InverseLerp 是什么？
    //就是 “线性过渡计算”比如：从 180 → 360 度当前在 270 度→ 返回 0.5（半透明）

    // DOTween平滑渐变
    private void SetImageFade(Image img, float targetA, int idx)
    {
        KillTween(idx); // 先停掉旧动画，防止重叠乱跳
        fadeTweens[idx] = img.DOFade(targetA, 0.15f) // 渐变透明度
            .SetEase(Ease.Linear) // 匀速变化
            .SetUpdate(true); // 游戏暂停也能正常执行
    }
//    img.DOFade(目标透明度, 时间)→ DOTween 插件自带的渐变方法→ 0.15 秒超快平滑变透明度
//SetEase(Linear)→ 匀速变化，不加速不减速
//SetUpdate(true)→ 就算游戏暂停，动画也继续执行

    private void KillTween(int idx)//把正在播放的动画停掉，防止多个动画同时执行导致错乱
    {
        if (fadeTweens[idx] != null && fadeTweens[idx].IsPlaying())
            fadeTweens[idx].Kill();
    }

    private void OnDestroy()//物体被删除时，把所有动画关掉，避免内存泄漏、报错
    {
        foreach (var t in fadeTweens) t?.Kill();
    }
}