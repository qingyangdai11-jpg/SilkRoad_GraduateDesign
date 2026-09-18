using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BgSequenceController : MonoBehaviour
{
    public static BgSequenceController Instance;

    [Header("初始依次渐显图片 01→02→03")]
    public CanvasGroup img01;
    public CanvasGroup img02;
    public CanvasGroup img03;

    [Header("完成后显示图片 04→05→06")]
    public CanvasGroup img04;
    public CanvasGroup img05;
    public CanvasGroup img06;

    [Header("动画速度")]
    public float fadeTime = 1f;
    public float interval = 0.5f;

    private bool _allWoodChecked = false;

    public GameObject wood1;
    public GameObject wood2;
    public GameObject wood3;
    public GameObject wood4;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 初始隐藏所有图片
        img01.alpha = 0;
        img02.alpha = 0;
        img03.alpha = 0;
        img04.alpha = 0;
        img05.alpha = 0;
        img06.alpha = 0;

        // 播放 01→02→03 依次渐显动画
        PlayStartAnimation();
    }

    private void Update()
    {
        // 每帧检测 4 个 wood 是否全部隐藏
        CheckAllWoodInactive();
    }

    #region 01→02→03 依次渐显
    private void PlayStartAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(img01.DOFade(1, fadeTime));
        seq.AppendInterval(interval);
        seq.Append(img02.DOFade(1, fadeTime));
        seq.AppendInterval(interval);
        seq.Append(img03.DOFade(1, fadeTime));
    }
    #endregion

    #region 检测 4 个 wood 是否全部 setActive(false)
    private void CheckAllWoodInactive()
    {
       
        if (_allWoodChecked) return;

        // 只要有一个没拖，就不判断
        if (wood1 == null || wood2 == null || wood3 == null || wood4 == null)
            return;

        // 核心判断：四个全部 inactive
        bool allFalse =
            !wood1.activeSelf &&
            !wood2.activeSelf &&
            !wood3.activeSelf &&
            !wood4.activeSelf;

        if (allFalse)
        {
            _allWoodChecked = true;
            OnAllWoodComplete(); // 满足条件才调用
        }
    }
    #endregion


    #region 全部完成：解锁滑动 + 04→05→06 渐显
    void OnAllWoodComplete()
    {
        Debug.Log("🔍 进入OnAllWoodComplete");
        _allWoodChecked = true;

        // 1. 解锁滑动
        if (ScrollLimit9.instance != null)
        {
            ScrollLimit9.instance.UnlockScroll();
            Debug.Log("✅ 滑动已解锁");
        }
        else
        {
            Debug.LogError("❌ UIScrollRestrict.Instance为null！");
        }


        // 2. DOTween动画序列
        Sequence finishSequence = DOTween.Sequence();
        finishSequence.AppendInterval(1f);

        //img04.gameObject.SetActive(true);

        finishSequence.AppendCallback(() => {
            img04.alpha = 0;
            img05.alpha = 0;
            img06.alpha = 0;
            img04.gameObject.SetActive(true);
            img05.gameObject.SetActive(true);
            img06.gameObject.SetActive(true);
        });

        // 第五步：talk1渐显
        finishSequence.Append(img04.DOFade(1f, fadeTime));

        // 第六步：等待0.5秒
        finishSequence.AppendInterval(interval);

        // 第七步：port1渐显
        finishSequence.Append(img05.DOFade(1f, fadeTime));

        finishSequence.AppendInterval(interval);

        // 第七步：port1渐显
        finishSequence.Append(img06.DOFade(1f, fadeTime));

        // 最后统一播放整个序列
        finishSequence.Play();
    }
    #endregion


}