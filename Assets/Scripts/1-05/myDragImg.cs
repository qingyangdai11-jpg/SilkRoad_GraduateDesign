using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class myDragImg : MonoBehaviour,IBeginDragHandler,IEndDragHandler, IDragHandler
{
    public int m_id;
    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<Image>().raycastTarget = true;
    }

   
}
