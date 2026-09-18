using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ArrowSlide : MonoBehaviour
{
    [Header("滑动设置")]
    public float moveSpeed = 200f;
    public float leftLimit = -411f;
    public float rightLimit = 419f;

    [Header("绿色成功区域")]
    public float greenLeft = -64f;
    public float greenRight = 125f;

    [Header("动画设置")]
    public Animator hitAnimator;
    public string hitStateName = "hit"; // 和Animator里的状态名完全一致

    private RectTransform rectTransform;
    private bool moveRight = true;
    private int clickCount = 0;

    // 新加的变量
    public LoadNextSceneGanally sceneLoader; //  Inspector拖入你的场景加载脚本
    private int successCount = 0; // 成功次数
    private bool isLoad = false; // 防止重复跳转
    private Sequence mainSeq;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //if (rectTransform == null)
        //{
        //    Debug.LogError("❌ 箭头没有RectTransform组件！");
        //    enabled = false;
        //    return;
        //}

        // 强制初始位置到左边界
        rectTransform.anchoredPosition = new Vector2(leftLimit, rectTransform.anchoredPosition.y);
        Debug.Log("✅ 脚本启动，动画监听就绪");
    }

    void Update()
    {
        if (rectTransform == null) return;

        // 箭头滑动逻辑
        MoveArrow();

        // 强制钳制位置，防止越界
        rectTransform.anchoredPosition = new Vector2(
            Mathf.Clamp(rectTransform.anchoredPosition.x, leftLimit, rightLimit),
            rectTransform.anchoredPosition.y
        );//箭头的 x 坐标强制限制在leftLimit和rightLimit之间，不管滑动逻辑里怎么动，最后都会被钳制住，彻底解决箭头越界的问题。

        // 鼠标点击判断
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            CheckClickResult(clickCount);
        }
    }

    void MoveArrow()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;

        if (moveRight)
        {
            currentPos.x += moveSpeed * Time.deltaTime;
            if (currentPos.x >= rightLimit)
            {
                moveRight = false;
            }
        }
        else
        {
            currentPos.x -= moveSpeed * Time.deltaTime;
            if (currentPos.x <= leftLimit)
            {
                moveRight = true;
            }
        }

        rectTransform.anchoredPosition = currentPos;
    }

    void CheckClickResult(int clickID)
    {
        float arrowX = rectTransform.anchoredPosition.x;
        //string logMsg = $"【点击 {clickID} | 时间: {Time.time:F2}s】箭头位置: {arrowX:F2} -> ";

        if (arrowX >= greenLeft && arrowX <= greenRight)
        {
            Debug.Log("成功，强制从头播放Hit动画");

                // 【关键】直接强制播放hit动画，不管当前状态，从头开始
                hitAnimator.Play(hitStateName, 0, 0f);
            // 参数说明：
            // 1. hitStateName：动画状态名（和Animator里的完全一致）
            // 2. 0：层索引，默认0
            // 3. 0f：归一化时间，0就是从头开始播放
            successCount++; // 成功次数+1
                            // 成功3次，触发渐隐+跳转
            if (successCount >= 3 && !isLoad)
            {
                isLoad = true; // 先标记，防止重复触发
                mainSeq = DOTween.Sequence();

                // 1. 先等待5秒
                mainSeq.AppendInterval(4f);

                // 2. 等待结束后，再调用加载协程（用AppendCallback把逻辑加到序列里）
                mainSeq.AppendCallback(() =>
                {
                    StartCoroutine(sceneLoader.LoadScene());
                });
            }
        }
        else
        {
            Debug.Log("失败，不播放动画");
        }
    }
}