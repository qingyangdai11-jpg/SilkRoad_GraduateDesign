using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rt;//物品的RectTransform组件（UI 元素的位置、大小都靠它控制）
    private Canvas _canvas;//物品所在的父 Canvas，拖拽时要把物品临时放到 Canvas 下，避免层级问题
    private CanvasGroup _canvasGroup;//物品的CanvasGroup组件，用来控制射线检测（防止拖拽时挡住托盘的触发器）
    private Vector3 _originalPos;//物品的初始位置，拖拽结束后如果没放到托盘上，就回到这个位置
    private Transform _originalParent;//物品的初始父物体，放回原位时恢复父物体

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _originalParent = transform.parent;
    }

    public void oriPosition()
    {
        _originalPos = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();//把当前物品的 UI 层级设置为 “最后一个”，也就是显示在所有其他 UI 元素的最上层。
        //为什么要这么写：防止拖拽时，物品被背景、托盘等其他 UI 元素挡住，导致 “看起来脱离鼠标” 的错觉。
        _canvasGroup.blocksRaycasts = false;//关闭物品的射线检测，这样鼠标点击就不会被物品挡住，托盘的触发器才能正常检测到物品
        transform.SetParent(_canvas.transform);//把物品的父物体临时设置为 Canvas，这样拖拽时物品会显示在所有 UI 的最上层，不会被其他物体挡住
        _rt.anchoredPosition = Vector2.zero;//把物品的锚点位置重置为(0,0)。
        //为什么要这么写：消除物品在原父物体下的位置偏移，确保后续跟随鼠标时，位置计算不会出现错位、跳动。
    }

    public void OnDrag(PointerEventData eventData)
    {
        //_rt.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        //        eventData.delta：鼠标从上一帧到当前帧的位置变化量；
        //除以_canvas.scaleFactor：适配不同分辨率的 Canvas，保证拖拽的速度在不同屏幕上一致；
        //_rt.anchoredPosition += ...：让物品跟着鼠标移动。

        Vector3 mousePos;//声明一个Vector3类型的变量，用来接收转换后的鼠标世界坐标
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out mousePos);
        //Unity 官方提供的屏幕坐标→UI 世界坐标转换工具，是 UI 拖拽的标准写法
        //_canvas.GetComponent<RectTransform>()	参考的 UI 矩形区域，这里就是整个 Canvas 的 RectTransform
        //eventData.position 鼠标当前的屏幕坐标（像素坐标）
        //eventData.pressEventCamera 事件对应的相机（比如 Canvas 的 Render Camera），用来做坐标转换
        //out mousePos 输出参数，转换后的世界坐标会存到这里
        //它不是靠累加位移差，而是直接把鼠标当前的屏幕坐标，转换成物品所在世界的坐标，物品位置和鼠标位置是 “实时绑定” 的，不会有累计误差，也就不会脱离鼠标
        transform.position = mousePos;
//        把物品的世界坐标，直接设置成上面转换好的mousePos。
//效果：鼠标在哪，物品的位置就被设置在哪，实现 “死死粘住鼠标” 的效果，完全不会飘
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (transform.parent == _canvas.transform)
        {
            transform.position = _originalPos;
            transform.SetParent(_originalParent);

            
        }//物品的父物体还是 Canvas（说明没放到托盘上）：
        //把物品的位置恢复到初始位置；
//把父物体恢复成初始的父物体，物品回到原来的层级。
    }
}