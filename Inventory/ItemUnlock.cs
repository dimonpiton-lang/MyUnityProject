using UnityEngine;

public class ItemUnlock : MonoBehaviour
{
    public enum UnlockType
    {
        Scanner,
        //  Можно добавить и другие типы разблокировок в будущем (например, Ability, Weapon, и т.д.)
    }

    public UnlockType unlockType; // Тип разблокировки
    public Sprite icon;         // Спрайт иконки для радара

    public void Unlock(Transform player)
    {
        switch (unlockType)
        {
            case UnlockType.Scanner:
                UnlockScanner(player);
                break;
                // Добавляем другие случаи для других типов разблокировок
        }

        // Показываем UI разблокировки
        ScannerUnlockUI[] unlockUIComponents = Resources.FindObjectsOfTypeAll<ScannerUnlockUI>();
        if (unlockUIComponents != null && unlockUIComponents.Length > 0)
        {
            // Проверяем, активен ли GameObject
            if (unlockUIComponents[0].gameObject.activeInHierarchy)
            {
                unlockUIComponents[0].ShowUnlockUI();
            }
            else
            {
                Debug.LogWarning("Объект ScannerUnlockUI не активен в иерархии.");
            }
        }
        else
        {
            Debug.LogWarning("Не найден компонент ScannerUnlockUI в сцене.");
        }
    }

    private void UnlockScanner(Transform player)
    {
        ItemScanner scanner = player.GetComponent<ItemScanner>();
        if (scanner != null)
        {
            scanner.enabled = true; // Включаем скрипт сканера
            scanner.ToggleScan();  // Включаем сканер
            SetRadarIcon(player); // Устанавливаем иконку для сканера
        }
        else
        {
            Debug.LogWarning("Не найден компонент ItemScanner на игроке.");
        }
    }

    private void SetRadarIcon(Transform player)
    {
        ItemScanner scanner = player.GetComponent<ItemScanner>();
        if (scanner != null)
        {
            // Теперь у нас есть доступ к icon из ItemUnlock
            // Но нам нужно передать эту иконку в ItemScanner, чтобы он использовал ее
            // Самый простой способ - добавить public Sprite icon в ItemScanner
            scanner.radarIconSprite = icon;
        }
        else
        {
            Debug.LogWarning("Не найден компонент ItemScanner на игроке.");
        }
    }
}