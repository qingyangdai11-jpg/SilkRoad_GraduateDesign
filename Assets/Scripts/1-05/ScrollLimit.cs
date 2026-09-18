using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrollLimit : MonoBehaviour
{
    public static ScrollLimit instance;

    private ScrollRect scroll;
    private RectTransform content;

    [Header("限制位置：拼图完成前不能滑到这里以下")]
    public float minYPosition = 223f;

    [Header("是否已经解锁滑动")]
    public bool isUnlocked = false;

    void Awake()
    {
        instance = this;
        scroll = GetComponent<ScrollRect>();
        content = scroll.content;
    }

    void Update()
    {
        if (isUnlocked) return;

        // 核心限制：content 的 Y 坐标不能大于 223
        if (content.anchoredPosition.y > minYPosition)
        {
            content.anchoredPosition = new Vector2(
                content.anchoredPosition.x,
                minYPosition
            );
        }
    }

    // 拼图完成后调用这个方法解锁
    public void UnlockScroll()
    {
        isUnlocked = true;
        Debug.Log("✅ 滑动已解锁");
    }

    public static implicit operator ScrollLimit(ScrollLimit9 v)
    {
        throw new NotImplementedException();
    }
}