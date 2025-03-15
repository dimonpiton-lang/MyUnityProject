using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;



public class RadialInventory : MonoBehaviour
{
    [Header("Settings")]
    public int numberOfSlots = 6;
    public float radius = 100f;
    public float itemRadius = 150f;
    public float itemAngleOffset = 0f;
    public float slotAngleOffset = 0f;
    public float menuScale = 1f;
    public float selectionScaleMultiplier = 1.2f;
    public float selectionSpeed = 10f;
    [SerializeField] private float timeScale = 0.2f;
    private float normalTimeScale = 1f;

    [Header("UI Elements")]
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public Transform slotsParent;
    public Image selectionCursor;
    public Image itemSelectionCursor;
    public Sprite defaultIcon;
    public List<Sprite> categoryIcons;

    private List<RectTransform> slots = new List<RectTransform>();
    private List<GameObject> currentCategoryItems = new List<GameObject>();
    private int currentSelection = -1;
    private int currentItemSelection = -1;
    private bool menuOpen = false;
    private bool cursorVisible = false;

    private float targetScale = 1f;
    public ItemCategory selectedCategory = ItemCategory.None;

    [Header("Inventory Limits")]
    public int PrimaryWeaponSlots = 3;
    public int SecondaryWeaponSlots = 2;
    public int MeleeWeaponSlots = 1;
    public int GadgetSlots = 4;
    public int ConsumableSlots = 5;
    public int ThrowableWeaponSlots = 2;

    [Header("Crafting")]
    public CraftingManager craftingManager;
    public List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>();

    // Категории, которые нужно "брать в руки"
    public List<ItemCategory> equipableCategories = new List<ItemCategory>()
    {
        ItemCategory.PrimaryWeapon,
        ItemCategory.SecondaryWeapon,
        ItemCategory.MeleeWeapon
    };
    // Инвентарь для отображаемых предметов
    public Dictionary<ItemCategory, Item[]> itemSlots = new Dictionary<ItemCategory, Item[]>();
    // Хранилище для материалов
    public Dictionary<Item, int> materialStorage = new Dictionary<Item, int>();

    void Start()
    {
        // Инициализация инвентаря
        itemSlots = new Dictionary<ItemCategory, Item[]>()
        {
            { ItemCategory.PrimaryWeapon, new Item[PrimaryWeaponSlots] },
            { ItemCategory.SecondaryWeapon, new Item[SecondaryWeaponSlots] },
            { ItemCategory.MeleeWeapon, new Item[MeleeWeaponSlots] },
            { ItemCategory.Gadget, new Item[GadgetSlots] },
            { ItemCategory.Consumable, new Item[ConsumableSlots] },
            { ItemCategory.ThrowableWeapon, new Item[ThrowableWeaponSlots] }
        };

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
            Vector3 position = GetPositionOnCircle(angle, radius);
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
                Debug.LogWarning("Нет иконки категории для слота " + i);
            }
        }
    }
    //Тут была ошибка из за повторного использования
    private Vector3 GetPositionOnCircle(float angle, float radius)
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
            selectionCursor.gameObject.SetActive(true);
            itemSelectionCursor.gameObject.SetActive(false);
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (selectedCategory != ItemCategory.None)
            {
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

        if (menuOpen)
        {
            if (selectedCategory == ItemCategory.None)
            {
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

                if (Input.GetKeyDown(KeyCode.R))
                {
                    UseSelectedItem();
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
        if (item.isCraftingMaterial)
        {
            // Добавляем материал на "склад"
            if (materialStorage.ContainsKey(item))
            {
                materialStorage[item] += 1; // Увеличиваем количество
            }
            else
            {
                materialStorage.Add(item, 1);
            }
            Debug.Log("Материал " + item.name + " добавлен на склад.");
            return true;
        }
        else
        {
            // Добавляем предмет в инвентарь
            Item[] slots = itemSlots[item.category];
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    slots[i] = item;
                    Debug.Log("Предмет " + item.name + " добавлен в слот " + i + " категории " + item.category);
                    return true;
                }
            }
        }

        Debug.Log("Нет места для предмета " + item.name);
        return false;
    }
    //Обновляем отображение
    public bool RemoveItem(Item item)
    {
        if (item.isCraftingMaterial)
        {
            if (materialStorage.ContainsKey(item))
            {
                if (materialStorage[item] > 0)
                {
                    materialStorage[item] -= 1;
                    Debug.Log("Предмет " + item.name + " удален из materialStorage");
                    return true;
                }
            }
            return false;
        }
        else
        {
            Item[] slots = itemSlots[item.category];
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == item)
                {
                    slots[i] = null;
                    Debug.Log("Предмет " + item.name + " удален из ItemSlots");
                    return true;
                }
            }
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
        selectionCursor.gameObject.SetActive(false);
        itemSelectionCursor.gameObject.SetActive(true);
        UpdateItemCursor();
        DisplayCategoryItems();
    }

    void DisplayCategoryItems()
    {
        if (currentSelection < 0 || currentSelection >= slots.Count)
        {
            Debug.LogError("Invalid currentSelection: " + currentSelection);
            return;
        }

        RectTransform selectedSlot = slots[currentSelection];
        Vector3 slotPosition = selectedSlot.anchoredPosition;
        ItemCategory category = selectedCategory;
        Item[] itemsToShow = itemSlots[category];
        int itemCount = itemsToShow.Length;

        float angleRad = Mathf.Atan2(selectedSlot.anchoredPosition.y, selectedSlot.anchoredPosition.x);
        float slotAngle = angleRad * Mathf.Rad2Deg;

        float angleRange = 180f;
        float startAngle = slotAngle - 90f;

        float angleStep = angleRange / (itemCount + 1);

        ClearCategoryItems();

        for (int i = 0; i < itemCount; i++)
        {
            float angle = startAngle + (i + 1) * angleStep + itemAngleOffset;
            float angleRadItem = angle * Mathf.Deg2Rad;

            float x = slotPosition.x + itemRadius * Mathf.Cos(angleRadItem);
            float y = slotPosition.y + itemRadius * Mathf.Sin(angleRadItem);

            GameObject newItemGO = Instantiate(itemPrefab, slotsParent);
            RectTransform itemRect = newItemGO.GetComponent<RectTransform>();
            itemRect.anchoredPosition = new Vector3(x, y, 0f);
            itemRect.localScale = Vector3.one * menuScale;
            currentCategoryItems.Add(newItemGO);

            CategoryItemSlot itemSlot = newItemGO.GetComponent<CategoryItemSlot>();
            CraftingRecipe recipeForSlot = GetRecipeForSlot(i);

            if (itemSlot != null)
            {
                if (itemsToShow[i] != null)
                {
                    itemSlot.SetItem(itemsToShow[i]);
                }
                else if (recipeForSlot != null)
                {
                    itemSlot.SetRecipe(recipeForSlot);
                }
                else
                {
                    itemSlot.SetItem(null);
                }
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

    CraftingRecipe GetRecipeForSlot(int slotIndex)
    {
        // Здесь логика определения, какой рецепт должен быть в этом слоте основываясь на доступных предметах в инвентаре и материалах на складе.
        //Начинаем проверять, можно ли скрафтить предмет
        if (craftingRecipes.Count > 0)
        {
            foreach (CraftingRecipe recipe in craftingRecipes)
            {
                //Если категория рецепта соответствует текущей выбранной.
                if (recipe.craftingCategory == selectedCategory)
                {
                    //Проверяем, можем ли мы скрафтить предмет
                    if (craftingManager.CanCraft(recipe, itemSlots, materialStorage))
                    {
                        return recipe;
                    }
                }
            }
        }
        return null;
    }

    void UseSelectedItem()
    {
        if (selectedCategory != ItemCategory.None && currentItemSelection >= 0 && currentItemSelection < currentCategoryItems.Count)
        {
            CategoryItemSlot selectedSlot = currentCategoryItems[currentItemSelection].GetComponent<CategoryItemSlot>();
            if (selectedSlot != null)
            {
                CraftingRecipe recipeToCraft = selectedSlot.GetRecipe();

                if (recipeToCraft != null)
                {
                    // Выполняем крафт, если выбран рецепт
                    craftingManager.Craft(recipeToCraft, itemSlots, materialStorage);
                    // Укажите категорию для нового предмета
                    Item newItem = Instantiate(recipeToCraft.resultItem);

                    AddItem(newItem);

                    Destroy(currentCategoryItems[currentItemSelection]);
                    currentCategoryItems.RemoveAt(currentItemSelection);
                    currentItemSelection = -1;
                    UpdateItemCursor();
                    SetCategory(selectedCategory);
                    return;
                }

                // Проверяем, что слот не пустой
                Item[] slots = itemSlots[selectedCategory];
                if (slots != null && currentItemSelection < slots.Length)
                {
                    Item itemToUse = slots[currentItemSelection];
                    if (itemToUse != null)
                    {
                        if (equipableCategories.Contains(selectedCategory))
                        {
                            itemToUse.Equip();
                        }
                        else
                        {
                            itemToUse.Use();
                        }

                        //Очищаем ячейку после использования
                        slots[currentItemSelection] = null; //Теперь ячейка пуста
                        Destroy(currentCategoryItems[currentItemSelection]);
                        currentCategoryItems.RemoveAt(currentItemSelection);
                        currentItemSelection = -1;
                        UpdateItemCursor();
                        SetCategory(selectedCategory);

                    }
                    else
                    {
                        Debug.LogWarning("Item is null in slot " + currentItemSelection);
                    }
                }
                else
                {
                    Debug.LogWarning("Slot " + currentItemSelection + " is empty!");
                }
            }
        }
    }
    //Удаляет иконки предметов
    void ClearCategoryItems()
    {
        foreach (GameObject itemGO in currentCategoryItems)
        {
            Destroy(itemGO);
        }
        currentCategoryItems.Clear();
    }

}