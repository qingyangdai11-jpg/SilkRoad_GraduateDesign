using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;

public class Chest : MonoBehaviour
{
    [Header("箱子配置")]
    public RectTransform chestRect;
    public float shakeDuration = 0.3f;
    public float shakeStrength = 10f;

    [Header("音效配置")] // 新增：音效部分
    public AudioClip putInSound; // 把你的音效文件拖到这里
    private AudioSource audioSource; // 引用音频源组件

    [Header("场景跳转配置")] // 新增：场景跳转相关参数
    public int totalItems = 3; // 需要放入的总物品数（默认3个）
    public string nextSceneName; // 下一个场景的名称（在Build Settings里配置）
    private int currentPutCount = 0; // 当前已放入的物品数（初始为0）

    private bool isFade;
    public float fadeDuration;
    public CanvasGroup fadeCanvasGroup;

    void Awake()
    {
        // 获取挂在箱子上的 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayChestShake()
    {
        // 1. 播放抖动动画
        chestRect.DOKill();
        chestRect.DOShakePosition(shakeDuration, shakeStrength, 10, 90, false, true);

        // 2. 播放音效（新增）
        if (audioSource != null && putInSound != null)
        {
            audioSource.PlayOneShot(putInSound); // PlayOneShot 适合播放这种一次性触发音效
        }

        currentPutCount++;
        if (currentPutCount >= totalItems)
        {
            Debug.Log("所有物品已放入，跳转场景！");
            StartCoroutine(LoadNextScene()); // 启动协程
        }
    }

    private IEnumerator LoadNextScene()
    {
        // 延迟1秒（可选，让动画和音效播完）
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(Fade(1));
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator Fade(float targetAlpha)//1是黑，0是透明
    {
        isFade = true;

        fadeCanvasGroup.blocksRaycasts = true;
        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeDuration;
        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }

        fadeCanvasGroup.blocksRaycasts = false;
        isFade = false;
    }

}