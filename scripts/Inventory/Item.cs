using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public string description = "Add a Description";
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public ItemCategory category; // Добавляем категорию
    public bool isCraftingMaterial = false; // Является ли предмет материалом для крафта
    public ItemUnlockData unlockData;

    public GameObject itemPrefab; // **ДОБАВЛЯЕМ ЭТО ПОЛЕ**

    public virtual void Equip()
    {
        Debug.Log("Экипировано: " + name);
        //Реализуйте базовую логику экипировки здесь, если необходимо.
        //Например, можно вызвать событие, которое будет прослушиваться другими скриптами.
    }

    public virtual void Unequip()
    {
        Debug.Log("Снято: " + name);
        //Реализуйте базовую логику снятия экипировки здесь, если необходимо.
    }

    public virtual void Use()
    {
        Debug.Log("Использовано: " + name);
    }
    public void RemoveFromInventory()
    {
        //TODO: Implement logic to remove item from inventory.
    }
}