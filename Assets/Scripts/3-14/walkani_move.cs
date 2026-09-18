using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class walkani_move : MonoBehaviour
{
    [Header("背景移动设置")]
    public float waitTime=2f;       // 等待2秒
    public float moveDuration=13f;  // 背景移动总时长
    public float targetPosX= -1096f; // 目标X位置

    [Header("角色动画设置")]
    public Animator walkAnimator; // 把角色身上的Animator组件拖到这里

    public Image img;

    private RectTransform bgRect;
    private Sequence mainSeq;
    // Start is called before the first frame update
    void Start()
    {
        bgRect = GetComponent<RectTransform>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        mainSeq = DOTween.Sequence();
        mainSeq.AppendInterval(waitTime);
        // 等待结束后，立刻触发walk动画
        mainSeq.AppendCallback(PlayWalkAnimation);
        mainSeq.Append(bgRect.DOAnchorPosX(targetPosX, moveDuration)//DOTween 的 UI 动画方法，让bgRect的锚点 X 轴位置，在moveDuration时间内移动到targetPosX的位置。
                                                                    //因为targetPosX是负数，背景会向左匀速移动
                          .SetEase(Ease.Linear));//设置动画的缓动模式为Linear（线性），也就是匀速移动，没有加速 / 减速，适合做背景滚动
        
        // 5. 动画结束，执行图片淡入
        mainSeq.Append(img.DOFade(1, 1.5f).SetEase(Ease.Linear));


    }

    void PlayWalkAnimation()
    {
        // 安全校验，避免空引用报错
        if (walkAnimator != null)
        {
            // 给Animator发送Trigger，触发Idle→walk的过渡
            walkAnimator.SetTrigger("Idle");
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
