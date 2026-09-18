using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 通用场景音频控制器，用于控制单个场景的背景音乐和交互音效。
/// 挂载在场景中的某个空物体上即可生效。
/// </summary>
public class SceneAudioController : MonoBehaviour
{
    public static SceneAudioController Instance { get; private set; }

    [Header("背景音乐设置 (BGM)")]
    [Tooltip("此场景专用的背景音乐")]
    [SerializeField] private AudioClip bgmClip;
    [Tooltip("BGM 目标音量")]
    [SerializeField] [Range(0f, 1f)] private float bgmVolume = 0.8f;
    [Tooltip("BGM 淡入时长（秒）")]
    [SerializeField] private float fadeInDuration = 1.0f;
    [Tooltip("BGM 淡出时长（秒）")]
    [SerializeField] private float fadeOutDuration = 1.0f;

    [Header("拖拽音效设置 (Drag)")]
    [Tooltip("拖拽开始时的瞬间音效（可选）")]
    [SerializeField] private AudioClip dragStartClip;
    [Tooltip("拖拽过程中的循环音效（可选）")]
    [SerializeField] private AudioClip dragLoopClip;
    [Tooltip("拖拽放置正确（成功）时的音效")]
    [SerializeField] private AudioClip dragCorrectClip;
    [Tooltip("拖拽放置错误（失败）时的音效")]
    [SerializeField] private AudioClip dragIncorrectClip;

    [Header("点击与长按音效设置 (Click & Long Press)")]
    [Tooltip("常规单击音效")]
    [SerializeField] private AudioClip clickClip;
    [Tooltip("长按开始时的音效（可选）")]
    [SerializeField] private AudioClip longPressStartClip;
    [Tooltip("长按过程中的循环音效（可选）")]
    [SerializeField] private AudioClip longPressLoopClip;
    [Tooltip("长按完成时的音效")]
    [SerializeField] private AudioClip longPressCompleteClip;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        // 场景级单例初始化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning($"场景中已存在另一个 SceneAudioController，将销毁当前的物体: {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        // 初始化背景音乐播放组件
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.spatialBlend = 0f; // 2D 音效

        // 初始化音效播放组件
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f; // 2D 音效
    }

    private void Start()
    {
        // 启动时自动播放背景音乐并淡入
        if (bgmClip != null)
        {
            PlayBGM();
        }
    }

    /// <summary>
    /// 开始播放背景音乐（带淡入效果）
    /// </summary>
    public void PlayBGM()
    {
        if (bgmClip == null || bgmSource == null) return;

        bgmSource.clip = bgmClip;
        bgmSource.volume = 0f;
        bgmSource.Play();
        bgmSource.DOKill();
        bgmSource.DOFade(bgmVolume, fadeInDuration).SetUpdate(true);
    }

    /// <summary>
    /// 手动淡出背景音乐
    /// </summary>
    /// <param name="duration">淡出时长</param>
    public void FadeOutBGM(float duration)
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.DOKill();
            bgmSource.DOFade(0f, duration).SetUpdate(true);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        // 当该组件被销毁（例如切换场景、关卡重置）且正在播放音乐时，
        // 动态创建一个临时的 DontDestroyOnLoad 物体用于完成淡出，避免声音突然中断。
        if (bgmSource != null && bgmSource.isPlaying && bgmSource.volume > 0f)
        {
            GameObject tempObj = new GameObject("TempBGM_FadeOut");
            DontDestroyOnLoad(tempObj);

            AudioSource tempSource = tempObj.AddComponent<AudioSource>();
            tempSource.clip = bgmSource.clip;
            tempSource.time = bgmSource.time;
            tempSource.volume = bgmSource.volume;
            tempSource.loop = true;
            tempSource.spatialBlend = 0f;
            tempSource.Play();

            // 使用 DOTween 控制临时音频源淡出，并在完成后销毁临时物体
            tempSource.DOFade(0f, fadeOutDuration)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (tempObj != null)
                    {
                        Destroy(tempObj);
                    }
                });
        }
    }

    #region 拖拽音效 API (Drag SFX)

    /// <summary>
    /// 开始拖拽特定对象时调用。
    /// </summary>
    /// <param name="targetSource">可选，传入该物体的 AudioSource 以播放 3D 空间音效。若不传，则在 Controller 挂载的 2D 源上播放。</param>
    public void PlayDragStart(AudioSource targetSource = null)
    {
        PlayClip(dragStartClip, targetSource);
        if (dragLoopClip != null)
        {
            StartLoopClip(dragLoopClip, targetSource);
        }
    }

    /// <summary>
    /// 停止拖拽循环音效。通常在拖拽结束或取消时调用。
    /// </summary>
    public void StopDragLoop(AudioSource targetSource = null)
    {
        StopLoopClip(targetSource);
    }

    /// <summary>
    /// 拖拽完成后调用，用于播放成功或失败的反馈音效。
    /// </summary>
    /// <param name="isCorrect">拖拽放置是否正确/成功</param>
    /// <param name="targetSource">可选，传入该物体的 AudioSource 以播放 3D 空间音效。若不传，则在 Controller 挂载的 2D 源上播放。</param>
    public void PlayDragEnd(bool isCorrect, AudioSource targetSource = null)
    {
        StopDragLoop(targetSource);
        PlayClip(isCorrect ? dragCorrectClip : dragIncorrectClip, targetSource);
    }

    #endregion

    #region 点击与长按音效 API (Click & Long Press SFX)

    /// <summary>
    /// 单击特定对象时调用。
    /// </summary>
    public void PlayClick(AudioSource targetSource = null)
    {
        PlayClip(clickClip, targetSource);
    }

    /// <summary>
    /// 开始长按特定对象时调用。
    /// </summary>
    public void PlayLongPressStart(AudioSource targetSource = null)
    {
        PlayClip(longPressStartClip, targetSource);
        if (longPressLoopClip != null)
        {
            StartLoopClip(longPressLoopClip, targetSource);
        }
    }

    /// <summary>
    /// 停止长按循环音效（在长按被玩家中途取消/松开时调用）。
    /// </summary>
    public void StopLongPressLoop(AudioSource targetSource = null)
    {
        StopLoopClip(targetSource);
    }

    /// <summary>
    /// 长按成功触发时调用。
    /// </summary>
    public void PlayLongPressComplete(AudioSource targetSource = null)
    {
        StopLongPressLoop(targetSource);
        PlayClip(longPressCompleteClip, targetSource);
    }

    #endregion

    #region 私有辅助方法

    private void PlayClip(AudioClip clip, AudioSource targetSource)
    {
        if (clip == null) return;

        AudioSource source = (targetSource != null) ? targetSource : sfxSource;
        source.PlayOneShot(clip);
    }

    private void StartLoopClip(AudioClip clip, AudioSource targetSource)
    {
        if (clip == null) return;

        AudioSource source = (targetSource != null) ? targetSource : sfxSource;
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    private void StopLoopClip(AudioSource targetSource)
    {
        AudioSource source = (targetSource != null) ? targetSource : sfxSource;
        if (source.isPlaying && source.loop)
        {
            source.Stop();
            source.loop = false;
        }
    }

    #endregion
}
