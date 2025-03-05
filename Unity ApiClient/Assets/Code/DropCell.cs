using UnityEngine;
using UnityEngine.EventSystems;

public class DropCell : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null && transform.childCount == 0)
        {
            dropped.transform.SetParent(transform);
            dropped.transform.localPosition = Vector3.zero; 
        }
    }
}

