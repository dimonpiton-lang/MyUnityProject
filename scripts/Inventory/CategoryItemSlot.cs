using UnityEngine;
using UnityEngine.UI;

public class CategoryItemSlot : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public Sprite defaultIcon; // Ссылка на пустую иконку
    public Image defaultImageIcon;

    private Item item;
    private CraftingRecipe recipe;  // Рецепт крафта
    private ItemCategory itemCategory; // Сохраняем категорию предмета

    void Start()
    {
        defaultImageIcon.sprite = defaultIcon;
    }
    public void SetItem(Item newItem)
    {
        item = newItem;
        recipe = null;
        if (newItem != null)
        {
            if (icon != null) icon.sprite = item.icon;
            if (nameText != null) nameText.text = item.name;
            icon.enabled = true;
            defaultImageIcon.enabled = false;

        }
        else
        {
            // Если предмет null, отображаем пустую иконку
            if (icon != null) icon.sprite = null;
            if (nameText != null) nameText.text = ""; // Или любое другое значение
            icon.enabled = false;
            defaultImageIcon.enabled = true;
        }
    }

    public void SetRecipe(CraftingRecipe newRecipe)
    {
        recipe = newRecipe;
        item = null;

        if (newRecipe != null)
        {
            // Отображаем иконку и название результата крафта
            if (icon != null) icon.sprite = newRecipe.resultItem.icon;
            if (nameText != null) nameText.text = "Скрафтить " + newRecipe.resultItem.name;
            icon.enabled = true;
            defaultImageIcon.enabled = false;
        }
        else
        {
            // Если рецепт null, отображаем пустую иконку
            if (icon != null) icon.sprite = defaultIcon;
            if (nameText != null) nameText.text = "";
            icon.enabled = false;
            defaultImageIcon.enabled = true;
        }
    }

    public Item GetItem()
    {
        return item;
    }
    public CraftingRecipe GetRecipe()
    {
        return recipe;
    }
}