using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    public List<ItemAmount> ingredients; // ������ ����������� ������������
    public Item resultItem;           // �������, ������� ��������� � ���������� ������
    public int resultAmount = 1;      // ���������� ���������, ������� ���������
    public ItemCategory craftingCategory; // ��������� � ������� ��������� �������
}

[System.Serializable]
public class ItemAmount
{
    public Item item;
    public int amount;
}