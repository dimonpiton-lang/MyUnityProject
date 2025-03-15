using UnityEngine;
using UnityEngine.UI;

public class CategoryItemSlot : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public Sprite defaultIcon; // ������ �� ������ ������
    public Image defaultImageIcon;

    private Item item;
    private CraftingRecipe recipe;  // ������ ������
    private ItemCategory itemCategory; // ��������� ��������� ��������

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
            // ���� ������� null, ���������� ������ ������
            if (icon != null) icon.sprite = null;
            if (nameText != null) nameText.text = ""; // ��� ����� ������ ��������
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
            // ���������� ������ � �������� ���������� ������
            if (icon != null) icon.sprite = newRecipe.resultItem.icon;
            if (nameText != null) nameText.text = "��������� " + newRecipe.resultItem.name;
            icon.enabled = true;
            defaultImageIcon.enabled = false;
        }
        else
        {
            // ���� ������ null, ���������� ������ ������
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