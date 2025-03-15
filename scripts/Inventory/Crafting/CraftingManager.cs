using UnityEngine;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    public List<CraftingRecipe> recipes;  // ������ ��������� ��������

    public bool CanCraft(CraftingRecipe recipe, Dictionary<ItemCategory, Item[]> itemSlots, Dictionary<Item, int> materialStorage)
    {
        foreach (ItemAmount ingredient in recipe.ingredients)
        {
            int itemCount = 0;

            // ��������� � itemSlots (������������ ���������)
            foreach (var itemArray in itemSlots.Values)
            {
                foreach (var item in itemArray)
                {
                    if (item == ingredient.item)
                    {
                        itemCount++;
                    }
                }
            }

            // ��������� � materialStorage ("�����")
            if (materialStorage.ContainsKey(ingredient.item))
            {
                itemCount += materialStorage[ingredient.item];
            }

            if (itemCount < ingredient.amount)
            {
                return false; // ������������ ������������
            }
        }
        return true; // ���������� ������������
    }

    public void Craft(CraftingRecipe recipe, Dictionary<ItemCategory, Item[]> itemSlots, Dictionary<Item, int> materialStorage)
    {
        if (CanCraft(recipe, itemSlots, materialStorage))
        {
            // ������� �����������
            foreach (ItemAmount ingredient in recipe.ingredients)
            {
                RemoveIngredients(ingredient.item, ingredient.amount, itemSlots, materialStorage);
            }

            // ��������� ��������� ������ (��� ������)
            Debug.Log("���������: " + recipe.resultItem.name);
        }
        else
        {
            Debug.Log("������������ ������������ ��� ������: " + recipe.name);
        }
    }

    public void RemoveIngredients(Item itemToRemove, int amount, Dictionary<ItemCategory, Item[]> itemSlots, Dictionary<Item, int> materialStorage)
    {
        // 1. ������� �� itemSlots (������������ ���������)
        int removedFromInventory = 0;
        foreach (var itemArray in itemSlots.Values)
        {
            for (int i = 0; i < itemArray.Length; i++)
            {
                if (itemArray[i] == itemToRemove)
                {
                    itemArray[i] = null;
                    removedFromInventory++;
                    if (removedFromInventory >= amount)
                    {
                        break;
                    }
                }
            }
        }

        // 2. ������� �� materialStorage ("�����")
        int remainingAmount = amount - removedFromInventory;
        if (remainingAmount > 0 && materialStorage.ContainsKey(itemToRemove))
        {
            int storedAmount = materialStorage[itemToRemove];
            if (storedAmount <= remainingAmount)
            {
                materialStorage.Remove(itemToRemove);
            }
            else
            {
                materialStorage[itemToRemove] = storedAmount - remainingAmount;
            }
        }
    }
}