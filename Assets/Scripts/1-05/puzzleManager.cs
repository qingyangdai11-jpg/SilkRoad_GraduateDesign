using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class puzzleManager : MonoBehaviour
{
    public static puzzleManager Instance;
    public Transform m_transSlotsContainer;

    [Header("要渐隐的拼图界面")]
    public CanvasGroup puzzlePanel;

    [Header("要渐显的图片")]
    public CanvasGroup talk1;
    public CanvasGroup port1;

    [Header("动画参数（可在Inspector调整）")]
    public float fadeDuration = 2f; // 渐隐/渐显时长（秒）
    public float waitAfterComplete = 2f; // 完成后等待时长（秒）

    private bool isAnimPlaying = false;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckResult()
    {
        bool isSucceed = true;//是否成功
        for (int i = 0; i < m_transSlotsContainer.childCount; i++)
        {
            var slotDropCtrl = m_transSlotsContainer.GetChild(i).GetComponent<mySlotDrop>();
            if (slotDropCtrl.m_DroppedItemId == -1)
            {
                isSucceed = false;
                break;
            }
            else if (slotDropCtrl.m_DroppedItemId != slotDropCtrl.m_mySlotID)
            {
                isSucceed = false;
                break;
            }
        }
        if (isSucceed == true)
        {
            Debug.Log("成功了");
            OnPuzzleSuccess(); // 执行成功逻辑
        }
        else
        {
            Debug.Log("还没成功");
        }
    }

    void OnPuzzleSuccess()
    {
        Debug.Log("🔍 进入OnPuzzleSuccess");
        isAnimPlaying = true;

        // 1. 解锁滑动
        if (ScrollLimit.instance != null)
        {
            ScrollLimit.instance.UnlockScroll();
            Debug.Log("✅ 滑动已解锁");
        }
        else
        {
            Debug.LogError("❌ UIScrollRestrict.Instance为null！");
        }



        // 2. DOTween动画序列
        Sequence finishSequence = DOTween.Sequence();

        // 第一步：拼图渐隐（先执行完这一步，再走后面的逻辑）
        finishSequence.Append(puzzlePanel.DOFade(0, fadeDuration));

        // 第二步：拼图渐隐完成后，隐藏面板（用Callback保证执行时机）
        finishSequence.AppendCallback(() => {
            Debug.Log("✅ 拼图渐隐完成");
            puzzlePanel.gameObject.SetActive(false);
        });

        // 第三步：【核心】等待1秒（拼图完全隐藏后，再等1秒，再执行后续）
        finishSequence.AppendInterval(1f); // 这里就是你要的1秒等待

        // 第四步：初始化talk1/port1（先设为透明+激活，避免闪屏）
        finishSequence.AppendCallback(() => {
            talk1.alpha = 0;
            port1.alpha = 0;
            talk1.gameObject.SetActive(true);
            port1.gameObject.SetActive(true);
        });

        // 第五步：talk1渐显
        finishSequence.Append(talk1.DOFade(1f, fadeDuration));

        // 第六步：等待0.5秒
        finishSequence.AppendInterval(0.5f);

        // 第七步：port1渐显
        finishSequence.Append(port1.DOFade(1f, fadeDuration));

        // 最后统一播放整个序列
        finishSequence.Play();
    }
}