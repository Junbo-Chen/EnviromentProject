using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentAfterDrag;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;

    [HideInInspector] public Object2D Data; // Dit is de correcte variabele naam
    public WorldObjectSpawner spawner;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        var dataHolder = GetComponent<Object2DDataHolder>();
        if (dataHolder == null)
        {
            Debug.LogError("Geen DataHolder gevonden op: " + gameObject.name);
            return;
        }

        Data = dataHolder.Data;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.localPosition;
        originalParent = transform.parent;
        parentAfterDrag = transform.parent;

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public async void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        bool isValidDrop = false;
        Transform targetCell = null;

        // Gebruik RaycastAll om de juiste cel te vinden
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("GridCell"))
            {
                targetCell = result.gameObject.transform;
                break;
            }
        }
        if (targetCell != null && targetCell.childCount == 1)
        {
            if (Data == null)
            {
                Debug.LogError("Data is null!");
                return;
            }

            // Gebruik wereldcoördinaten van de cel
            Vector3 cellWorldPosition = targetCell.position;
            Data.positionX = cellWorldPosition.x;
            Data.positionY = cellWorldPosition.y;

            Debug.Log($"Dropped at cell: {cellWorldPosition}");

            isValidDrop = await spawner.UpdateObjectInApi(Data);
            parentAfterDrag = targetCell;
        }

        if (!isValidDrop)
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition;
        }
        else
        {
            transform.SetParent(parentAfterDrag);
            transform.localPosition = Vector3.zero;
        }
    }
}