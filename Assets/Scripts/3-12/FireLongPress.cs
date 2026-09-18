using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FireLongPress : MonoBehaviour
{
    [Header("设置")]
    public float holdDuration = 2f; // 每级需要长按的时间
    public int maxStage = 3;       // 最多到第3级

    [Header("额外功能")]
    public ScrollRect scrollRect; // 把你的 ScrollRect 拖进来
    public int finalStage = 3;    // 最后一个阶段（Fire03）
    public Image img04;
    public Image but;

    private Animator _anim;
    private float _pressTimer;
    private int _currentStage;
    private bool _isHolding;
    private Sequence mainSeq;

    void Start()
    {
        _anim = GetComponent<Animator>();
        // 初始状态：FireStage=0，停在Idle
        _currentStage = 0;
        _anim.SetInteger("FireStage", 0);
    }

    void Update()
    {
        HandleInput();

        if (_isHolding)
        {
            _pressTimer += Time.deltaTime;
            UpdateStage();
        }
    }

    // 处理按下/抬起
    void HandleInput()
    {
        // 按下：开始计时，重置阶段
        if (Input.GetMouseButtonDown(0))
        {
            _isHolding = true;
            _pressTimer = 0;
            _currentStage = 0;
            _anim.SetInteger("FireStage", 0);
        }

        // 抬起：停止计时，重置回Idle
        if (Input.GetMouseButtonUp(0))
        {
            _isHolding = false;
            _pressTimer = 0;
            _currentStage = 0;
            _anim.SetInteger("FireStage", 0);
        }
    }

    // 核心逻辑：每满holdDuration秒，升一级
    void UpdateStage()
    {
        // 计算当前应该到的阶段（满2秒=1级，满4秒=2级，满6秒=3级）
        int targetStage = Mathf.FloorToInt(_pressTimer / holdDuration) + 1;
        //算法解析：Mathf.FloorToInt(2.1 / 2) + 1 = 1 + 1 = 2。表示长按超过 2 秒，就进入第 2 阶段。
        // 不超过最大阶段
        targetStage = Mathf.Min(targetStage, maxStage);

        // 只有阶段变化时，才更新动画
        if (targetStage != _currentStage)
        //防抖动：只有 targetStage 和 _currentStage 不一致时才切换动画，避免同一阶段重复触发。
        {
            _currentStage = targetStage;
            _anim.SetInteger("FireStage", _currentStage);
            Debug.Log($"阶段更新：{_currentStage}（长按时间：{_pressTimer:F1}秒）");

            // 【新加：最后阶段播放时启用 ScrollRect】
            if (_currentStage == finalStage)
            {
                if (scrollRect != null)
                    scrollRect.enabled = true;


                // 3. 只有对象不为空时，才执行动画
                if (img04 != null && but != null)
                {
                    // 关键：先把透明度重置为0，再播放淡入动画
                    img04.color = new Color(img04.color.r, img04.color.g, img04.color.b, 0);
                    but.color = new Color(but.color.r, but.color.g, but.color.b, 0);

                    // 4. 构建动画序列
                    mainSeq = DOTween.Sequence();
                    mainSeq.AppendInterval(5f);                // 等待2秒
                    mainSeq.Append(img04.DOFade(1, 3f));     // img04淡入1.5秒
                    mainSeq.AppendInterval(3f);                 // 再等5秒
                    mainSeq.Append(but.DOFade(1, 1.5f));       // but淡入1.5秒
                }
            }
        }
    }
}