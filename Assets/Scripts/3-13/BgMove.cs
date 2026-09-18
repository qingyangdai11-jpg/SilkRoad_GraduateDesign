using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgMove : MonoBehaviour
{
    [Header("背景移动设置")]
    public float waitTime;       // 等待2秒
    public float moveDuration ;  // 背景移动总时长
    public float targetPosX ; // 目标X位置
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
        mainSeq.Append(bgRect.DOAnchorPosX(targetPosX, moveDuration)//DOTween 的 UI 动画方法，让bgRect的锚点 X 轴位置，在moveDuration时间内移动到targetPosX的位置。
//因为targetPosX是负数，背景会向左匀速移动
                          .SetEase(Ease.Linear));//设置动画的缓动模式为Linear（线性），也就是匀速移动，没有加速 / 减速，适合做背景滚动

        mainSeq.AppendInterval(0.5f);
        mainSeq.Append(img.DOFade(1, 2f));

        mainSeq.AppendCallback(() =>
        {
            // 查找场景中所有激活的DraggableItem组件
            DraggableItem[] allDraggableItems = FindObjectsOfType<DraggableItem>();

            // 遍历每个物品，调用oriPosition()更新初始位置
            foreach (var item in allDraggableItems)
            {
                item.oriPosition();
            }
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        mainSeq?.Kill();
    }
}
