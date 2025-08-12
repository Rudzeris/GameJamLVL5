using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MortarClicker : MonoBehaviour, IInteractable
{
    [Header("UI Elements")]
    public GameObject GamePanel;
    public Button clickButton;
    public Slider progressBar;
    public TextMeshProUGUI statusText;

    [Header("Settings")]
    public float clicksRequired = 10;
    public RequiredItem[] requiredItems; // ������ � �����������
    public ItemType rewardItem;
    public int rewardAmount = 1;

    private float currentClicks = 0;
    private bool canClick = false;

    void Start()
    {
        clickButton.onClick.AddListener(OnClickMortar);
        progressBar.value = 0;

        CheckIngredients();
    }

    void CheckIngredients()
    {
        foreach (var req in requiredItems)
        {
            if (Managers.Inventory.GetItemCount(req.type) < req.amount)
            {
                canClick = false;
                statusText.text = "�� ������� ������������!";
                return;
            }
        }

        canClick = true;
        statusText.text = "������, ����� �������!";
    }

    void OnClickMortar()
    {
        if (!canClick)
        {
            statusText.text = "������� ������ �����������!";
            return;
        }

        currentClicks++;
        progressBar.value = currentClicks / clicksRequired;

        if (currentClicks >= clicksRequired)
        {
            CompleteMix();
        }
    }

    void CompleteMix()
    {
        // ������� �����������
        foreach (var req in requiredItems)
        {
            Managers.Inventory.RemoveItem(req.type, req.amount);
        }

        // ��� �������
        Managers.Inventory.AddItem(rewardItem, rewardAmount);

        statusText.text = $"������� {rewardItem}!";

        // �����
        currentClicks = 0;
        progressBar.value = 0;

        CheckIngredients();
    }

    public void Activate()
    {
        Managers.Mission.TryDeliver(DeliveryPointType.Mortar);
        if(Managers.Mission.GetTaskStatus(DeliveryPointType.Mortar) == TaskStatus.Completed)
        {
            GamePanel.SetActive(true);
        }
    }
}

[System.Serializable]
public struct RequiredItem
{
    public ItemType type;
    public int amount;
}
