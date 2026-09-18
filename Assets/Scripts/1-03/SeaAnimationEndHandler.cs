using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SeaAnimationEndHandler : MonoBehaviour
{
    [Header("跳转设置")]
    public string targetSceneName; // 这里填写你要跳转的场景名称
    private bool isFade;
    public float fadeDuration;
    public CanvasGroup fadeCanvasGroup;

    // 动画结束时会被动画事件调用
    public void OnAnimationComplete()
    {
        // 等待2秒后跳转场景
        StartCoroutine(WaitAndFadeAndJump());
        //SceneManager.LoadScene(targetSceneName);
    }

    private IEnumerator WaitAndFadeAndJump()
    {
        // 等待指定的秒数
        yield return new WaitForSeconds(1f);
        yield return Fade(1f);
        SceneManager.LoadScene(targetSceneName);
        
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