using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public string description = "Add a Description";
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public ItemCategory category; // Добавляем категорию
    public ItemUnlockData unlockData;

    public GameObject itemPrefab; // **ДОБАВЛЯЕМ ЭТО ПОЛЕ**

    public void RemoveFromInventory()
    {
        //TODO: Implement logic to remove item from inventory.
    }
}