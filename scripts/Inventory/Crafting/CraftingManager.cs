using UnityEngine;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    public List<CraftingRecipe> recipes;  // Список доступных рецептов

    public bool CanCraft(CraftingRecipe recipe, Dictionary<ItemCategory, Item[]> itemSlots, Dictionary<Item, int> materialStorage)
    {
        foreach (ItemAmount ingredient in recipe.ingredients)
        {
            int itemCount = 0;

            // Проверяем в itemSlots (отображаемый инвентарь)
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

            // Проверяем в materialStorage ("склад")
            if (materialStorage.ContainsKey(ingredient.item))
            {
                itemCount += materialStorage[ingredient.item];
            }

            if (itemCount < ingredient.amount)
            {
                return false; // Недостаточно ингредиентов
            }
        }
        return true; // Достаточно ингредиентов
    }

    public void Craft(CraftingRecipe recipe, Dictionary<ItemCategory, Item[]> itemSlots, Dictionary<Item, int> materialStorage)
    {
        if (CanCraft(recipe, itemSlots, materialStorage))
        {
            // Удаляем ингредиенты
            foreach (ItemAmount ingredient in recipe.ingredients)
            {
                RemoveIngredients(ingredient.item, ingredient.amount, itemSlots, materialStorage);
            }

            // Добавляем результат крафта (как раньше)
            Debug.Log("Скрафчено: " + recipe.resultItem.name);
        }
        else
        {
            Debug.Log("Недостаточно ингредиентов для крафта: " + recipe.name);
        }
    }

    public void RemoveIngredients(Item itemToRemove, int amount, Dictionary<ItemCategory, Item[]> itemSlots, Dictionary<Item, int> materialStorage)
    {
        // 1. Удаляем из itemSlots (отображаемый инвентарь)
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

        // 2. Удаляем из materialStorage ("склад")
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