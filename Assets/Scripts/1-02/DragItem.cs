using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTrans;
    private Canvas canvas;
    private Vector2 startPos; // 记录物品初始位置
    public Chest chest; // 拖拽结束后要放入的箱子，在Inspector面板赋值

    void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPos = rectTrans.anchoredPosition;
    }

    // 开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 拖拽时关闭射线检测，避免挡住鼠标判断
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // 拖拽中
    public void OnDrag(PointerEventData eventData)
    {
        // 把鼠标屏幕坐标转换为Canvas内的坐标，让物品跟着鼠标移动
        rectTrans.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // 结束拖拽
    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复射线检测
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // 判断当前物品是否在箱子的范围内
        if (RectTransformUtility.RectangleContainsScreenPoint(
            chest.chestRect,
            Input.mousePosition,
            canvas.worldCamera))
        {
            // 满足条件：物品消失 + 触发箱子抖动
            gameObject.SetActive(false);
            chest.PlayChestShake();
        }
        else
        {
            // 没拖进箱子，物品回到初始位置
            rectTrans.anchoredPosition = startPos;
        }
    }
}