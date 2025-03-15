using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;            // Ссылка на объект Item
    public float pickupDistance = 2f;

    void Start() { }

    public bool PickupItem(Transform player, RadialInventory radialInventory)
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= pickupDistance)
        {
            //Если предмет имеет данные разблокировки и isUnlockItem == true
            if (item.unlockData != null && item.unlockData.isUnlockItem)
            {
                //Разблокируем сканер
                if (item.unlockData.prefabUnlockItem != null)
                {
                    ItemUnlock itemUnlock = item.unlockData.prefabUnlockItem.GetComponent<ItemUnlock>();
                    if (itemUnlock != null)
                    {
                        itemUnlock.Unlock(player);
                    }
                }
                else
                {
                    Debug.LogWarning("prefabUnlockItem не назначен в ItemUnlockData для предмета " + item.name);
                }

                Destroy(gameObject); // Удаляем предмет из сцены
                return true; // Предмет был "поднят", даже если не попал в инвентарь
            }
            else
            {
                //Если предмет не разблокирующий, добавляем его в инвентарь
                bool wasPickedUp = radialInventory.AddItem(item);

                if (wasPickedUp)
                {
                    Debug.Log("Подняли предмет: " + item.name);
                    Destroy(gameObject);
                    return true;
                }
                else
                {
                    Debug.Log("Инвентарь полон!");
                    return false;
                }
            }
        }

        return false;
    }
}