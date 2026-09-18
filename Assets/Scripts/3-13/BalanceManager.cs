using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BalanceManager : MonoBehaviour
{
    public static BalanceManager Instance;
    //定义一个静态单例Instance，作用是让其他脚本（比如Scale.cs）
    //可以直接通过BalanceManager.Instance调用管理器的方法，不用找组件。

    [Header("天平设置")]
    public Scale leftSlot;
    public Scale rightSlot;
    public Transform balancePivot;//天平的旋转支点（也就是天平中间的轴），控制这个物体的旋转，就能让天平倾斜。
    public float maxTiltAngle = 25f;//天平最大的倾斜角度，防止天平转得太夸张
    public float smoothSpeed = 8f;//天平旋转的平滑速度，数值越大，天平转得越快。

    [Header("UI设置")]
    //public Text successText;

    private float _targetRotation = 0f;//用来存天平最终要转到的角度，初始是 0（水平状态）
    private Sequence mainSeq;

    public Animator ChangeAnimator;


    void Awake()//在start之前
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    //单例初始化逻辑：
    //如果Instance还没有赋值，就把当前实例赋值给Instance；
//如果已经有实例了，就销毁当前的BalanceManager，保证整个场景只有一个管理器。

    void Start()
    {
       
    }

    void Update()
    {
        // 平滑旋转天平
        if (balancePivot != null)
        {
            balancePivot.rotation = Quaternion.Lerp(
                balancePivot.rotation,
                Quaternion.Euler(0, 0, _targetRotation),
                Time.deltaTime * smoothSpeed
            );
        }
    }
    //Quaternion.Lerp(a, b, t)：线性插值方法，让物体从当前旋转角度，平滑过渡到目标角度：
   // a：天平当前的旋转角度；
//b：目标角度，用Quaternion.Euler(0, 0, _targetRotation)表示绕 Z 轴旋转_targetRotation度；
//t：插值速度，Time.deltaTime* smoothSpeed保证不同帧率下旋转速度一致，smoothSpeed越大，旋转越快。

    // 外部调用：检查当前天平状态
    public void CheckBalance()
    {
        // 1. 先判断托盘引用是否存在
        if (leftSlot == null || rightSlot == null)
        {
            Debug.LogError("左右托盘引用未赋值！");
            return;
        }

        // 2. 获取两边重量
        int leftValue = GetSlotValue(leftSlot);
        int rightValue = GetSlotValue(rightSlot);
        //用GetSlotValue()方法，分别计算左右托盘上物品的重量

        Debug.Log($"左边重量：{leftValue}，右边重量：{rightValue}");

        // 3. 计算倾斜角度
        if (leftValue >rightValue)
        {
            float diff = leftValue - rightValue;
            _targetRotation = Mathf.Max(diff * 1.5f, -maxTiltAngle);
            // 左边重 → 天平向左倾斜（角度为负）

        }
        else if (rightValue >leftValue)
        {

            // 右边重 → 天平向右倾斜（角度为正）
            float diff = rightValue - leftValue;
            
            _targetRotation = -Mathf.Min(diff * 1.5f, maxTiltAngle);

        }
        else
        {
            // 两边相等，回到水平
            _targetRotation = 0f;
            // 只有平衡时才检测胜利条件
            CheckWinCondition();
        }
//        diff是左边减右边的重量差，diff * 1.5f是把重量差放大，让倾斜更明显；Mathf.Min(..., maxTiltAngle)保证角度不会超过最大倾斜限制；前面加负号，代表向左倾斜（Unity 里 Z 轴负方向是向左）。
//如果右边重：同理，用正的角度让天平向右倾斜。
//如果两边重量相等：天平回到水平（_targetRotation = 0），同时调用CheckWinCondition()检查是否满足胜利条件。
    }

    // 获取盘子上的物品价值
    private int GetSlotValue(Scale slot)
    {
        // 空托盘直接返回0
        if (slot == null || slot.currentItem == null)
        {
            return 0;
        }

        // 尝试获取Item组件
        Item item = slot.currentItem.GetComponent<Item>();
        if (item == null)
        {
            Debug.LogWarning($"物品 {slot.currentItem.name} 没有挂载Item组件！");
            return 0;
        }

        int value = item.GetValue();//调用item.GetValue()，根据物品类型获取重量值，返回给调用者
        Debug.Log($"物品 {slot.currentItem.name} 重量：{value}");
        return value;
    }

    // 检测是否满足胜利条件
    private void CheckWinCondition()
    {
        // 先判断两边托盘是否都有物品
        if (leftSlot == null || rightSlot == null ||
            leftSlot.currentItem == null || rightSlot.currentItem == null)
        {
            Debug.Log("有托盘为空，不检测胜利条件");
            return;
        }//如果有一个托盘是空的，不检测胜利条件

        // 安全获取物品的Item组件,获取左右物品的Item组件，确保两个物品都有这个组件
        Item leftItem = leftSlot.currentItem.GetComponent<Item>();
        Item rightItem = rightSlot.currentItem.GetComponent<Item>();

        // 组件不存在也直接返回
        if (leftItem == null || rightItem == null)
        {
            Debug.Log("物品缺少Item组件，不检测胜利条件");
            return;
        }

        Debug.Log($"左边物品类型：{leftItem.itemType}，右边物品类型：{rightItem.itemType}");

        // 判断是否是丝绸+稀有宝石的组合
        bool isCorrect =
            (leftItem.itemType == Item.ItemType.Silk && rightItem.itemType == Item.ItemType.RareGem) ||
            (leftItem.itemType == Item.ItemType.RareGem && rightItem.itemType == Item.ItemType.Silk);

        if (isCorrect)
        {
            Debug.Log("🎉 谜题解开！胜利条件满足！");
            ChangeAnimator.SetTrigger("Playchange");

            // 1. 先获取加载器（提前获取，避免序列里找不到）
            GameObject loaderObj = GameObject.Find("BG");
            LoadNextSceneGanally loader = loaderObj.GetComponent<LoadNextSceneGanally>();

            // 2. 创建序列：等待2秒 → 再执行场景加载
            mainSeq = DOTween.Sequence();
            mainSeq.AppendInterval(6f);
            // 等待结束后，启动场景加载协程
            mainSeq.AppendCallback(() =>//向序列中添加一个 “回调指令”。它的意思是：“等待 2 秒结束后，执行括号里的代码”
            {
                StartCoroutine(loader.LoadScene());
            });

            // 序列会自动播放，无需额外调用Play()

        }
        else
        {
            Debug.Log("物品组合不正确，继续调整");
        }
    }
}