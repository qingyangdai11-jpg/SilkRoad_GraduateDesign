using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("滚动速度")]
    public float scrollSpeed = 180f;

    [Header("按 Left 值判断")]
    public float pauseLeft;    // 暂停位置
    public float endLeft;      // 彻底停止位置

    private RectTransform rect;
    public bool isScrolling = true;
    private bool isStopped = false;
    private bool hasPaused = false;   // 记录是否已经暂停过，防止继续滚动后再次被判定为需要暂停


    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isStopped || !isScrolling) return;//被终止或被暂停的时候，不再继续自动滑动
       
        // 向左滚动 = Left 不断减小
        rect.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;
        //Debug.Log($"[Update] offsetMin.x={rect.offsetMin.x:F2}, isStopped={isStopped}, isScrolling={isScrolling}");
        // 当前 UI Left 值,rect.offsetMin.x Unity UI 里你看到的 Left 数值
        if (!hasPaused && rect.offsetMin.x <= pauseLeft)// 到达 Left=-2589暂停, rect.offsetMin.x:Unity UI 里你看到的 Left 数值
        {
            hasPaused = true;
            StopAtPausePosition();
        }
        if (rect.offsetMin.x <= endLeft)// 到达 Left=-4137 → 彻底停止
        {
            StopAtEndPosition();
        }
    }

    void StopAtPausePosition()//暂停
    {
        Debug.Log("已暂停");
        isScrolling = false;
        rect.offsetMin = new Vector2(pauseLeft, rect.offsetMin.y);
    }

    void StopAtEndPosition()//终止
    {
        isStopped = true;
        isScrolling = false;
        rect.offsetMin = new Vector2(endLeft, rect.offsetMin.y);
    }

    // 涂抹完成后调用这个方法，继续滚动
    public void ContinueScroll()
    {
        isScrolling = true;
        isStopped = false;  // ← 加上这一行！
        Debug.Log("☑️ 背景已恢复滚动");
    }
}