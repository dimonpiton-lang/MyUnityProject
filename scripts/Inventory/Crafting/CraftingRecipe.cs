using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    public List<ItemAmount> ingredients; // Список необходимых ингредиентов
    public Item resultItem;           // Предмет, который получится в результате крафта
    public int resultAmount = 1;      // Количество предметов, которое получится
    public ItemCategory craftingCategory; // Категория в которой крафтится предмет
}

[System.Serializable]
public class ItemAmount
{
    public Item item;
    public int amount;
}