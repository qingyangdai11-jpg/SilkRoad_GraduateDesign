using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class mySlotDrop : MonoBehaviour,IDropHandler
{
    public int m_mySlotID;//自己的ID
    public int m_DroppedItemId=-1;//当前放置的拼图id
    public void OnDrop(PointerEventData eventData)
    {
        //if (eventData.pointerDrag != null)
        //{
        //    Debug.Log("有一个对象拖拽过来了" + eventData.pointerDrag.name);
        //    eventData.pointerDrag.transform.position = this.transform.position;

        //    m_DroppedItemId = eventData.pointerDrag.GetComponent<myDragImg>().m_id;

        //    puzzleManager.Instance.CheckResult();
        //}

        // 🔴 第一层校验：必须有拖拽对象
        if (eventData.pointerDrag == null)
        {
            return; // 直接退出，不执行任何逻辑
        }

        // 🔴 第二层校验：拖拽的对象必须有 myDragImg 组件（是真正的拼图块）
        myDragImg dragImg = eventData.pointerDrag.GetComponent<myDragImg>();
        if (dragImg == null)
        {
            // 拦截非拼图块的误触发
            return;
        }

        Debug.Log("有一个对象拖拽过来了" + eventData.pointerDrag.name);
        eventData.pointerDrag.transform.position = this.transform.position;
        m_DroppedItemId = eventData.pointerDrag.GetComponent<myDragImg>().m_id;
        puzzleManager.Instance.CheckResult();
        SceneAudioController.Instance.PlayDragEnd(true);
    }
}

   
    
