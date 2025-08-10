using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum SlotMode
{
    Inventory,
    Boiler
}

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemType ItemType;
    public int Amount;

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;

    public Canvas canvas;
    public ItemDatabase itemDatabase;

    private RectTransform rectTransform;
    public SlotMode slotMode = SlotMode.Inventory;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        if (canvas == null)
            canvas = gameObject.transform.parent.root.gameObject.GetComponent<Canvas>();
    }

    public void Setup(ItemType type, int amount, SlotMode mode = SlotMode.Inventory)
    {
        slotMode = mode;

        ItemType = type;
        Amount = amount;

        iconImage.sprite = itemDatabase.GetIcon(type);
        amountText.text = Amount.ToString();

        // Для котла можно сделать визуально иначе или отключить перетаскивание
        canvasGroup.blocksRaycasts = (slotMode == SlotMode.Inventory);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotMode == SlotMode.Boiler) return; // запрет драг для котла

        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotMode == SlotMode.Boiler) return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
            out Vector2 localPoint
        );

        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slotMode == SlotMode.Boiler) return;

        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
        canvasGroup.blocksRaycasts = true;
    }
}
