using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DragWoodImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拖到目标：03图片")]
    public RectTransform targetImage;

    private RectTransform _rect;
    private Vector2 _originPos;    // 原始位置
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _originPos = _rect.anchoredPosition;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // 开始拖拽
    public void OnBeginDrag(PointerEventData eventData){}

    // 拖拽中
    public void OnDrag(PointerEventData eventData)
    {
        _rect.anchoredPosition += eventData.delta; // 跟随鼠标
    }

    // 结束拖拽
    [System.Obsolete]
    public void OnEndDrag(PointerEventData eventData)
    {
        // 检测是否拖到了03上
        bool isInTarget = RectTransformUtility.RectangleContainsScreenPoint(
            targetImage,
            eventData.position,
            eventData.pressEventCamera
        );

        if (isInTarget)
        {
            // 成功：回到原位 → 渐隐 → 禁用
            DoSuccessAnimation();
        }
        else
        {
            // 失败：直接回到原位
            _rect.anchoredPosition = _originPos;
        }
    }

    // 成功动画
    [System.Obsolete]
    private void DoSuccessAnimation()
    {
        // 1. 回到原来位置
        _rect.DOAnchorPos(_originPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            // 2. 渐隐消失
            _canvasGroup.DOFade(0, 0.4f).OnComplete(() =>
            {
                // 3. 彻底禁用
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
                gameObject.active = false;
            });
        });
    }
}