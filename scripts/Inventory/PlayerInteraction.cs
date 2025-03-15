using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 3f;                          // ��������� ��������������
    public KeyCode pickupKey = KeyCode.E;                            // ������� ��� ������� ���������
    public RadialInventory radialInventory;                          // ������ �� ���������� ���������

    private List<ItemPickup> nearbyItems = new List<ItemPickup>(); // ������ ��������� ���������

    void Start()
    {
        // �������� ������ �� ���������� ���������, ���� ��� �� ���� ����������� �������
        if (radialInventory == null)
        {
            radialInventory = GetComponentInChildren<RadialInventory>();
            if (radialInventory == null)
            {
                Debug.LogError("RadialInventory �� ������ � �������� �������� PlayerInteraction.");
            }
        }
    }

    void Update()
    {
        FindNearbyItems(); // ���� ��������� �������� ������ ����

        // ���� ������ ������� �������
        if (Input.GetKey(pickupKey) && !radialInventory.IsOpen())
        {
            TryPickupItem();
        }
    }

    // ������� ��������� ��������� �������
    void TryPickupItem()
    {
        // ���� ����� ���� �������� � ��������� ����������
        if (nearbyItems.Count > 0 && radialInventory != null)
        {
            // ������� ��������� �������
            ItemPickup closestItem = null;
            float closestDistance = Mathf.Infinity;
            foreach (ItemPickup item in nearbyItems)
            {
                float distance = Vector3.Distance(transform.position, item.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = item;
                }
            }

            // ���� ������� ������, ��������� ���
            if (closestItem != null)
            {
                bool pickedUp = closestItem.PickupItem(transform, radialInventory);
                if (pickedUp)
                {
                    nearbyItems.Remove(closestItem); //������� �� ������
                }
            }
        }
    }

    void FindNearbyItems()
    {
        nearbyItems.Clear(); // ������� ������

        // ���������� Physics.OverlapSphere ��� ������ ���� ����������� � ������� interactionDistance
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionDistance);

        foreach (Collider collider in colliders)
        {
            // ���� ��������� ����������� ItemPickup
            ItemPickup itemPickup = collider.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                nearbyItems.Add(itemPickup);
            }
        }
    }
}
