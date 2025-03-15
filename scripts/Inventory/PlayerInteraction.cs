using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 3f;                          // Дистанция взаимодействия
    public KeyCode pickupKey = KeyCode.E;                            // Клавиша для подбора предметов
    public RadialInventory radialInventory;                          // Ссылка на радиальный инвентарь

    private List<ItemPickup> nearbyItems = new List<ItemPickup>(); // Список ближайших предметов

    void Start()
    {
        // Получаем ссылку на радиальный инвентарь, если она не была установлена вручную
        if (radialInventory == null)
        {
            radialInventory = GetComponentInChildren<RadialInventory>();
            if (radialInventory == null)
            {
                Debug.LogError("RadialInventory не найден в дочерних объектах PlayerInteraction.");
            }
        }
    }

    void Update()
    {
        FindNearbyItems(); // Ищем ближайшие предметы каждый кадр

        // Если нажата клавиша подбора
        if (Input.GetKey(pickupKey) && !radialInventory.IsOpen())
        {
            TryPickupItem();
        }
    }

    // Попытка подобрать ближайший предмет
    void TryPickupItem()
    {
        // Если рядом есть предметы и инвентарь существует
        if (nearbyItems.Count > 0 && radialInventory != null)
        {
            // Находим ближайший предмет
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

            // Если предмет найден, подбираем его
            if (closestItem != null)
            {
                bool pickedUp = closestItem.PickupItem(transform, radialInventory);
                if (pickedUp)
                {
                    nearbyItems.Remove(closestItem); //Удаляем из списка
                }
            }
        }
    }

    void FindNearbyItems()
    {
        nearbyItems.Clear(); // Очищаем список

        // Используем Physics.OverlapSphere для поиска всех коллайдеров в радиусе interactionDistance
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionDistance);

        foreach (Collider collider in colliders)
        {
            // Если коллайдер принадлежит ItemPickup
            ItemPickup itemPickup = collider.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                nearbyItems.Add(itemPickup);
            }
        }
    }
}
