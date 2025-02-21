using UnityEngine;

[CreateAssetMenu(fileName = "New ItemUnlockData", menuName = "Inventory/Item Unlock Data")]
public class ItemUnlockData : ScriptableObject
{
    public bool isUnlockItem = false;
    public GameObject prefabUnlockItem; // —юда кидаем префаб с ItemUnlock
}