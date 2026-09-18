using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class SceneLoader : MonoBehaviour
{
    public string nextSceneName = "1-02";
    private bool isAnimationFinished = false;
    private PlayableDirector playableDirector; // Timeline控制器

    private bool isFade;
    public float fadeDuration;
    public CanvasGroup fadeCanvasGroup;

    private Vector2 mouseDownPos; // 记录鼠标按下时的位置
    private const float CLICK_THRESHOLD = 5f; // 点击判定阈值（像素，移动超过5px算拖拽）

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.stopped += (director) => OnAnimationFinished();//等价于下面这段代码
        //playableDirector.stopped += MyAlarmReceiver;//
        //private void MyAlarmReceiver(PlayableDirector director)
        //{
        //    OnAnimationFinished();
        //}
        Debug.Log("已绑定Timeline结束事件，等待动画播放完成...");
    }
    //void Update()
    //{
    //    if (isAnimationFinished && !isFade && Input.GetMouseButtonDown(0))
    //    {
    //        Debug.Log("触发点击，开始执行渐变");
    //        StartCoroutine(LoadNextScene());
    //    }
    //}

    void Update()
    {
        // 只在动画完成、未在淡入淡出时检测输入
        if (isAnimationFinished && !isFade)
        {
            // 1. 鼠标按下时，记录起始位置
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPos = Input.mousePosition;
            }

            // 2. 鼠标松开时，判定是点击还是拖拽
            if (Input.GetMouseButtonUp(0))
            {
                // 计算鼠标移动的总距离
                float moveDistance = Vector2.Distance(mouseDownPos, Input.mousePosition);

                // 3. 只有移动距离小于阈值，才判定为有效点击，触发跳转
                if (moveDistance < CLICK_THRESHOLD)
                {
                    Debug.Log("有效点击，执行场景切换");
                    StartCoroutine(LoadNextScene());
                }
                else
                {
                    Debug.Log($"拖拽操作（移动{moveDistance}px），忽略跳转");
                }
            }
        }
    }

    public IEnumerator LoadNextScene()
    {
        yield return StartCoroutine(Fade(1));

        
        SceneManager.LoadScene(nextSceneName);
        Destroy(gameObject);

    }



    //动画结束时调用的方法（由动画时间或者timeline信号触发）
    public void OnAnimationFinished()
    {
        
        isAnimationFinished = true;
        Debug.Log("动画播放完毕，现在点击可进入下一场景");
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
