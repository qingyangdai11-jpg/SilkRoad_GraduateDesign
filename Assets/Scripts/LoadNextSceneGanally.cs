using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class LoadNextSceneGanally : MonoBehaviour
{
    public string nextSceneName = "1-02";
    public float fadeDuration=1f;
    public CanvasGroup fadeCanvasGroup;

    

    public void OnButtonClick()
    {
        Debug.Log("按钮被点击了！");
        StartCoroutine(LoadScene());
    }

    public IEnumerator LoadScene()
    {
        Debug.Log("开始执行场景加载协程");
        yield return StartCoroutine(Fade(1));
        Debug.Log("Fade 结束，准备加载场景: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
    private IEnumerator Fade(float targetAlpha)//1是黑，0是透明
    {

        fadeCanvasGroup.blocksRaycasts = true;
        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeDuration;
        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }

        fadeCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator LastLoopWaitAndLoadScene()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(LoadScene());
    }


}
