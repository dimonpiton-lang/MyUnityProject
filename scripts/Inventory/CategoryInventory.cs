using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryInventory : MonoBehaviour
{
    public GameObject itemSlotPrefab; // ������ ����� ��������
    public Transform contentPanel;     // ������ Content � ScrollView
    public Button closeButton;          // ������ ��� �������� ���������

    private List<CategoryItemSlot> itemSlots = new List<CategoryItemSlot>();

    void Start()
    {
        closeButton.onClick.AddListener(CloseInventory);
        gameObject.SetActive(false);
    }

    public void DisplayItems(List<Item> items)
    {
        ClearSlots();
        foreach (Item item in items)
        {
            GameObject slotGO = Instantiate(itemSlotPrefab, contentPanel);
            CategoryItemSlot slot = slotGO.GetComponent<CategoryItemSlot>();
            if (slot != null)
            {
                slot.SetItem(item);
            }
            itemSlots.Add(slot);
        }
    }

    void ClearSlots()
    {
        foreach (var slot in itemSlots)
        {
            Destroy(slot.gameObject);
        }
        itemSlots.Clear();
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }
}