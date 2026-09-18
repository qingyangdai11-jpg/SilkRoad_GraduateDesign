using UnityEngine;

public class Scale : MonoBehaviour
{
    public GameObject currentItem = null;//用来记录当前托盘上放着的物品

    // 新增变量，用来保存物品进入托盘前的状态
    private Vector3 itemOriginalWorldPos;
    private Transform itemOriginalParent;

    // 仅保存：物品进入托盘前的 旋转Z值
    private float originalRotationZ;

    private void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    //Unity 的内置消息方法，当其他物体进入当前物体的 2D 触发器碰撞体时，自动执行这里的代码
    {
        if (other.CompareTag("Item") && currentItem == null)//避免一个托盘放多个物品
        {
            currentItem = other.gameObject;

            // ✅ 关键：保存物品进入托盘前的世界位置和父物体
            itemOriginalWorldPos = other.transform.position;
            itemOriginalParent = other.transform.parent;

            // ✅ 只保存原始旋转 Z 值
            originalRotationZ = currentItem.transform.eulerAngles.z;

            other.transform.SetParent(transform);
            other.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            //把物品的RectTransform的锚点位置设置为(0,0,0)，也就是让物品在托盘的父坐标系里，刚好对齐托盘的中心位置。
            //这一步是为了让物品放在托盘的正中间，不会歪歪扭扭。
            BalanceManager.Instance.CheckBalance();//调用天平管理器的CheckBalance()方法，通知管理器：“我这里物品变了，快重新算天平的倾斜角度！”
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    //Unity 内置消息方法，当其他物体离开当前触发器碰撞体时，自动执行这里的代码
    {
        if (other.gameObject == currentItem)//离开触发器的物体，必须是当前托盘正在持有的物品（避免其他无关物体离开时误触发）。
        {
            // ✅ 关键：恢复物品的父物体和位置
            other.transform.SetParent(itemOriginalParent);
            //other.transform.position = itemOriginalWorldPos;

            // ✅ 核心：仅还原旋转Z值，X/Y旋转、所有位置都保持不变
            Transform itemTrans = currentItem.transform;
            Vector3 currentRot = itemTrans.eulerAngles;
            itemTrans.eulerAngles = new Vector3(
                currentRot.x,  // 保留当前旋转X
                currentRot.y,  // 保留当前旋转Y
                originalRotationZ // 强制还原原始旋转Z
            );

            currentItem = null;//把currentItem设为null，表示托盘现在空了
            BalanceManager.Instance.CheckBalance();//再次调用天平管理器的CheckBalance()，通知管理器：“托盘空了，重新算天平状态！”
            
        }
    }
}