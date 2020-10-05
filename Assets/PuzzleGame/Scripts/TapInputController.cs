using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapInputController : MonoBehaviour, 
    IPointerDownHandler, 
    IPointerUpHandler,
    IDragHandler
{
    public event Action<int> PointerDown;
    public event Action<int> PointerDrag;
    public event Action PointerUp;
    
    bool pointerDown;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.tag != "Brick") return;
        
        pointerDown = true;
        int index = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();
        PointerDown?.Invoke(index);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!pointerDown) return;

        if (eventData.pointerCurrentRaycast.gameObject == null ||
            eventData.pointerCurrentRaycast.gameObject.tag != "Brick") return;
        
        int index = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();
        PointerDrag?.Invoke(index);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        PointerUp?.Invoke();
    }
}
