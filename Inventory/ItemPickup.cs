using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;            // ������ �� ������ Item
    public float pickupDistance = 2f;

    void Start() { }

    public bool PickupItem(Transform player, RadialInventory radialInventory)
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= pickupDistance)
        {
            //���� ������� ����� ������ ������������� � isUnlockItem == true
            if (item.unlockData != null && item.unlockData.isUnlockItem)
            {
                //������������ ������
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
                    Debug.LogWarning("prefabUnlockItem �� �������� � ItemUnlockData ��� �������� " + item.name);
                }

                Destroy(gameObject); // ������� ������� �� �����
                return true; // ������� ��� "������", ���� ���� �� ����� � ���������
            }
            else
            {
                //���� ������� �� ��������������, ��������� ��� � ���������
                bool wasPickedUp = radialInventory.AddItem(item);

                if (wasPickedUp)
                {
                    Debug.Log("������� �������: " + item.name);
                    Destroy(gameObject);
                    return true;
                }
                else
                {
                    Debug.Log("��������� �����!");
                    return false;
                }
            }
        }

        return false;
    }
}