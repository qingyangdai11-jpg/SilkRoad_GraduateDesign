using UnityEngine;
using UnityEngine.EventSystems;

public class ClockHandRotator : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("中心点(钟表圆心)")]
    public RectTransform centerPivot;

    // 固定配置：6张图→6整圈
    private readonly int imageCount = 6;
    private readonly float totalFullLoops = 5f;
    private float MaxTotalAngle => 360f * totalFullLoops;

    [Header("当前累计总角度")]
    public float currentTotalAngle;

    private ImageSwitcher imgSwitch;
    private bool isDraging;
    private float lastMouseWorldAngle;

    // 新增：引用场景加载脚本
    public LoadNextSceneGanally sceneLoader;
    //private bool isLastLoopTriggered = false;
    private Coroutine lastLoopWaitCoroutine;

    void Start()
    {
        imgSwitch = GetComponentInParent<ImageSwitcher>();
        if (centerPivot == null) centerPivot = GetComponent<RectTransform>();

        currentTotalAngle = 0f;
        lastMouseWorldAngle = GetMouseAngleToCenter();

        // 自动查找并获取引用（脚本必须在场景中存在）
        sceneLoader = FindObjectOfType<LoadNextSceneGanally>();
    }

    // 获取鼠标相对于圆心的角度
    private float GetMouseAngleToCenter()
    {
        // 计算鼠标位置指向中心点的方向向量
        Vector2 dir = Input.mousePosition - centerPivot.position;
        // 计算弧度并转换为角度（Atan2基于Y轴，需转换为基于X轴的数学逻辑）
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
//    ① Mathf.Atan2(dir.y, dir.x)
//这是数学函数：给方向，返回弧度
//传入方向的 y 和 x
//返回一个 弧度值（表示方向角度）
//② Mathf.Rad2Deg
//Unity 固定常量：弧度 → 角度 的换算比例因为 Atan2 返回的是弧度，人看不懂乘以它就能变成 0~360 度（人类能理解的角度）
//③ 合起来
//plaintext
//Mathf.Atan2(方向) → 得到弧度
//× Mathf.Rad2Deg → 变成角度
//return → 返回这个角度


    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraging = true;
        lastMouseWorldAngle = GetMouseAngleToCenter();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 如果拖拽结束 或 角度达到上限，直接返回
        if (!isDraging || currentTotalAngle >= MaxTotalAngle) return;

        float nowAngle = GetMouseAngleToCenter();
        float delta = Mathf.DeltaAngle(lastMouseWorldAngle, nowAngle);
        lastMouseWorldAngle = nowAngle;

        // 只允许顺时针累加（delta负=顺时针）
        if (delta < 0)
        {
            currentTotalAngle -= delta;
        }

        // 硬封顶6圈
        currentTotalAngle = Mathf.Clamp(currentTotalAngle, 0f, MaxTotalAngle);

        // 指针视觉转圈（只看模360）
        centerPivot.localEulerAngles = new Vector3(0, 0, -currentTotalAngle % 360f);//localEulerAngles,Unity 里专门用来 “设置 / 读取物体旋转角度” 的属性

        // 同步给图片切换
        imgSwitch?.UpdateByAngle(currentTotalAngle);//?. = 安全调用（防止脚本为空报错）



        // 检测是否进入最后一圈
        if (currentTotalAngle >= MaxTotalAngle )
        {
            
            //if (sceneLoader != null)
            //{
                // 用字符串调用，无需修改LastLoopWaitAndLoadScene的private权限
                sceneLoader.StartCoroutine("LastLoopWaitAndLoadScene");
            SceneAudioController.Instance.PlayDragEnd(true);
            //}
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDraging = false;
    }

    // 重置（可选）
    public void ResetClock()//将角度、旋转、图片状态全部归零，用于重置表盘
    {
        // 新增：重置最后一圈标记和协程
        //isLastLoopTriggered = false;
        if (lastLoopWaitCoroutine != null)
            StopCoroutine(lastLoopWaitCoroutine);

        currentTotalAngle = 0f;
        centerPivot.localEulerAngles = Vector3.zero;
        imgSwitch?.UpdateByAngle(0f);
    }
}