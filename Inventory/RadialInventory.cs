using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadialInventory : MonoBehaviour
{
    [Header("Settings")]
    public int numberOfSlots = 6; // ���������� ���������
    public float radius = 100f;   // ������ ��� ���������
    public float itemRadius = 150f; // ������ ��� ���������
    public float itemAngleOffset = 0f;
    public float slotAngleOffset = 0f;
    public float menuScale = 1f;
    public float selectionScaleMultiplier = 1.2f;
    public float selectionSpeed = 10f;
    [SerializeField] private float timeScale = 0.2f;
    private float normalTimeScale = 1f;

    [Header("UI Elements")]
    public GameObject slotPrefab;     // ������ ��� ���������
    public GameObject itemPrefab;     // ������ ��� ���������
    public Transform slotsParent;    // �������� ��� ������
    public Image selectionCursor;     // ������ ��� ���������
    public Image itemSelectionCursor; // ����� ������ ��� ���������
    public Sprite defaultIcon;
    public List<Sprite> categoryIcons; // ������ ��� ���������

    public List<Item> items = new List<Item>(); // ������ ���� ���������

    private List<RectTransform> slots = new List<RectTransform>();    // ����� ���������
    private List<GameObject> currentCategoryItems = new List<GameObject>(); // �������� � ���������
    private int currentSelection = -1;
    private int currentItemSelection = -1; // ����� ������ ���������� ��������
    private bool menuOpen = false;
    private bool cursorVisible = false;

    private float targetScale = 1f;
    public ItemCategory selectedCategory = ItemCategory.None;

    // ����������� �� ���-�� ��������� ��� ������ ���������.
    public Dictionary<ItemCategory, int> categoryItemLimits = new Dictionary<ItemCategory, int>()
    {
        { ItemCategory.PrimaryWeapon, 3 },   // ��������, �� ������ 3 �������� ������
        { ItemCategory.SecondaryWeapon, 2 }, // �� ������ 2 ��������������� ������
        { ItemCategory.MeleeWeapon, 1 },    // ������ ���� ������ �������� ���
        { ItemCategory.Gadget, 4 },           // �� ������ 4 ��������
        { ItemCategory.Consumable, 5 },       // �� ������ 5 �����������
        { ItemCategory.ThrowableWeapon, 3 }    // �� ������ 3 ����������� ������
    };

    void Start()
    {
        normalTimeScale = Time.timeScale;
        CreateSlots();
        UpdateVisuals();
        ToggleCursorVisibility();
        SetSlotsActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }

        if (menuOpen)
        {
            HandleInput();
        }

        UpdateVisuals();
    }

    void CreateSlots()
    {
        ItemCategory[] categories = (ItemCategory[])System.Enum.GetValues(typeof(ItemCategory));

        for (int i = 0; i < numberOfSlots; i++)
        {
            float angle = i * (360f / numberOfSlots) + slotAngleOffset;
            Vector3 position = GetPositionOnCircle(angle, radius); // ���������� ������ ���������
            GameObject newSlot = Instantiate(slotPrefab, slotsParent);
            RectTransform slotTransform = newSlot.GetComponent<RectTransform>();
            slotTransform.anchoredPosition = position;
            slotTransform.localScale = Vector3.one * menuScale;
            slots.Add(slotTransform);

            Image slotImage = newSlot.GetComponent<Image>();
            if (slotImage != null && i < categoryIcons.Count)
            {
                slotImage.sprite = categoryIcons[i];
            }
            else
            {
                Debug.LogWarning("��� ������ ��������� ��� ����� " + i);
            }
        }
    }

    Vector3 GetPositionOnCircle(float angle, float radius)
    {
        float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector3(x, y, 0f);
    }

    void ToggleMenu()
    {
        menuOpen = !menuOpen;
        selectionCursor.gameObject.SetActive(menuOpen);
        itemSelectionCursor.gameObject.SetActive(false);
        ToggleCursorVisibility();
        SetSlotsActive(menuOpen);

        if (menuOpen)
        {
            Time.timeScale = timeScale;
            UpdateVisuals();
        }
        else
        {
            Time.timeScale = normalTimeScale;
            currentSelection = -1;
            currentItemSelection = -1;
            UpdateVisuals();
            ClearCategoryItems();
            // ���������� ������ ��������� � �������� ������ ������
            selectionCursor.gameObject.SetActive(true);
            itemSelectionCursor.gameObject.SetActive(false);
        }
    }

    void HandleInput()
    {
        //������������ ����� �������� ��������� (��������� / �����)
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (selectedCategory != ItemCategory.None)
            {
                //������� �� ������ ��������� �� ������
                selectedCategory = ItemCategory.None;
                ClearCategoryItems();
                itemSelectionCursor.gameObject.SetActive(false);
                selectionCursor.gameObject.SetActive(true);
                currentItemSelection = -1;

            }
            else
            {
                SetCategoryBySelection();
            }
        }

        //���������
        if (menuOpen)
        {
            if (selectedCategory == ItemCategory.None)
            {
                //���������
                if (Input.GetKeyDown(KeyCode.D))
                {
                    currentSelection = (currentSelection + 1) % numberOfSlots;
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    currentSelection--;
                    if (currentSelection < 0)
                    {
                        currentSelection = numberOfSlots - 1;
                    }
                }
                UpdateItemCursor();
            }
            else
            {
                //�����
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (currentCategoryItems.Count > 0)
                    {
                        currentItemSelection = (currentItemSelection + 1) % currentCategoryItems.Count;
                    }

                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (currentCategoryItems.Count > 0)
                    {
                        currentItemSelection--;
                        if (currentItemSelection < 0)
                        {
                            currentItemSelection = currentCategoryItems.Count - 1;
                        }
                    }
                }

            }
            UpdateItemCursor();
        }
    }

    void SetCategoryBySelection()
    {
        switch (currentSelection)
        {
            case 0:
                SetCategory(ItemCategory.PrimaryWeapon);
                break;
            case 1:
                SetCategory(ItemCategory.SecondaryWeapon);
                break;
            case 2:
                SetCategory(ItemCategory.MeleeWeapon);
                break;
            case 3:
                SetCategory(ItemCategory.Gadget);
                break;
            case 4:
                SetCategory(ItemCategory.Consumable);
                break;
            case 5:
                SetCategory(ItemCategory.ThrowableWeapon);
                break;
            default:
                SetCategory(ItemCategory.None);
                break;
        }
    }

    void UpdateVisuals()
    {
        if (menuOpen)
        {
            if (currentSelection >= 0 && currentSelection < numberOfSlots && slots.Count > 0)
            {
                selectionCursor.rectTransform.anchoredPosition = Vector3.Lerp(selectionCursor.rectTransform.anchoredPosition, slots[currentSelection].anchoredPosition, Time.deltaTime * selectionSpeed);
                Debug.Log("Current Selection is " + currentSelection);
            }

            for (int i = 0; i < slots.Count; i++)
            {
                if (i == currentSelection)
                {
                    slots[i].localScale = Vector3.Lerp(slots[i].localScale, Vector3.one * menuScale * selectionScaleMultiplier, Time.deltaTime * selectionSpeed);
                }
                else
                {
                    slots[i].localScale = Vector3.Lerp(slots[i].localScale, Vector3.one * menuScale, Time.deltaTime * selectionSpeed);
                }
            }
        }
        else
        {
            selectionCursor.gameObject.SetActive(false);
            itemSelectionCursor.gameObject.SetActive(false);
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].localScale = Vector3.Lerp(slots[i].localScale, Vector3.one * menuScale, Time.deltaTime * selectionSpeed);
            }
        }
    }

    public bool AddItem(Item item)
    {
        if (items.Count < 12)
        {
            int currentCategoryCount = 0;
            foreach (Item existingItem in items)
            {
                if (existingItem.category == item.category)
                {
                    currentCategoryCount++;
                }
            }

            //��������� �����
            if (currentCategoryCount < categoryItemLimits[item.category])
            {
                items.Add(item);
                return true;
            }
            else
            {
                Debug.Log("����� ��� ��������� " + item.category + " ���������!");
                return false;
            }
        }
        else
        {
            Debug.Log("��������� �����!");
            return false;
        }
    }

    private void ToggleCursorVisibility()
    {
        cursorVisible = !cursorVisible;

        if (menuOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public bool IsOpen()
    {
        return menuOpen;
    }

    private void SetSlotsActive(bool active)
    {
        foreach (RectTransform slot in slots)
        {
            slot.gameObject.SetActive(active);
        }
    }

    public void SetCategory(ItemCategory category)
    {
        selectedCategory = category;
        ClearCategoryItems();
        currentItemSelection = -1;
        //�������� ������ ��������� � ���������� ������ ������
        selectionCursor.gameObject.SetActive(false);
        itemSelectionCursor.gameObject.SetActive(true);

        List<Item> filteredItems = new List<Item>();
        foreach (Item item in items)
        {
            if (item.category == category)
            {
                filteredItems.Add(item);
            }
        }

        DisplayCategoryItems(filteredItems);
        UpdateItemCursor();
    }

    //���������� ������ ��������� ���������
    void DisplayCategoryItems(List<Item> items)
    {
        if (currentSelection < 0 || currentSelection >= slots.Count)
        {
            Debug.LogError("Invalid currentSelection: " + currentSelection);
            return;
        }

        RectTransform selectedSlot = slots[currentSelection];
        Vector3 slotPosition = selectedSlot.anchoredPosition; // Use anchoredPosition for UI elements
        int itemCount = items.Count;
        itemCount = Mathf.Min(itemCount, 12);

        //�������
        float angleRad = Mathf.Atan2(selectedSlot.anchoredPosition.y, selectedSlot.anchoredPosition.x);
        //��������� � �������
        float slotAngle = angleRad * Mathf.Rad2Deg;

        float angleRange = 180f; // ��������
        float startAngle = slotAngle - 90f; // ������� ��������

        float angleStep = angleRange / (itemCount + 1);  //��������

        for (int i = 0; i < itemCount; i++)
        {
            float angle = startAngle + (i + 1) * angleStep + itemAngleOffset;  //��������

            // Convert angle to radians
            float angleRadItem = angle * Mathf.Deg2Rad;

            // Calculate the position of the item
            float x = slotPosition.x + itemRadius * Mathf.Cos(angleRadItem);
            float y = slotPosition.y + itemRadius * Mathf.Sin(angleRadItem);

            // Create a new item and position it relative to the slot
            GameObject newItemGO = Instantiate(itemPrefab, slotsParent);
            RectTransform itemRect = newItemGO.GetComponent<RectTransform>();
            itemRect.anchoredPosition = new Vector3(x, y, 0f);  // Use anchoredPosition for UI elements
            itemRect.localScale = Vector3.one * menuScale;
            currentCategoryItems.Add(newItemGO);

            // Set the item information
            CategoryItemSlot itemSlot = newItemGO.GetComponent<CategoryItemSlot>();
            if (itemSlot != null)
            {
                itemSlot.SetItem(items[i]);
            }

            Button button = newItemGO.GetComponent<Button>();
            if (button != null)
            {
                int itemIndex = i; // Capturing the index for the lambda expression
                button.onClick.AddListener(() => UseItem(itemIndex));
            }
        }
    }

    void UpdateItemCursor()
    {
        if (currentCategoryItems.Count == 0)
        {
            itemSelectionCursor.gameObject.SetActive(false);
            return;
        }

        if (currentItemSelection < 0 || currentItemSelection >= currentCategoryItems.Count)
        {
            currentItemSelection = 0;
        }
        itemSelectionCursor.gameObject.SetActive(true);
        RectTransform selectedItem = currentCategoryItems[currentItemSelection].GetComponent<RectTransform>();
        itemSelectionCursor.rectTransform.anchoredPosition = selectedItem.anchoredPosition;
    }

    void UseItem(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < currentCategoryItems.Count)
        {
            Debug.Log("Used item: " + itemIndex);

            // �������� ����� ������ ��� ������������� ��������
            // ��������, ����� ������ Use() � �������� ��� ���������� �����-���� ������ ��������
        }
    }

    //������� ������ ���������
    void ClearCategoryItems()
    {
        foreach (GameObject itemGO in currentCategoryItems)
        {
            Destroy(itemGO);
        }
        currentCategoryItems.Clear();
    }
}