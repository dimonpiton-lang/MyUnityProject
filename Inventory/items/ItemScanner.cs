using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemScanner : MonoBehaviour
{
    public float scanRadius = 10f;
    public RawImage radarImage;
    public GameObject radarIconPrefab;
    public KeyCode toggleKey = KeyCode.Q;
    public Sprite radarIconSprite;
    public float scanInterval = 0.1f; // Интервал сканирования в секундах

    private List<GameObject> radarIcons = new List<GameObject>();
    private bool isScannerUnlocked = false;
    private bool isScannerActive = false;

    void Start()
    {
        Debug.Log("ItemScanner.Start() called");

        // Включаем Canvas в Start()
        if (radarImage != null && radarImage.canvas != null)
        {
            radarImage.canvas.gameObject.SetActive(true);
            Debug.Log("Radar Canvas enabled at start");
        }
        else
        {
            Debug.LogError("RadarImage не назначен или Canvas RadarImage не найден!");
        }
        isScannerUnlocked = true;

        // Запускаем сканирование с интервалом
        InvokeRepeating("ScanForItems", 0f, scanInterval);
    }

    void OnDestroy()
    {
        // Останавливаем повторяющийся вызов, чтобы избежать ошибок
        CancelInvoke("ScanForItems");
    }

    void Update()
    {
        // Проверяем, разблокирован ли сканер
        if (isScannerUnlocked)
        {
            // Включаем/выключаем сканирование по нажатию Q
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleScan();
            }
        }
    }

    public void UnlockScanner()
    {
        Debug.Log("ItemScanner.UnlockScanner() called");
        Debug.Log("isScannerUnlocked set to true");

        // Больше не нужно включать Canvas здесь. Он уже включен в Start()
        Debug.Log("Canvas enabling moved to Start()");

        // Включаем сканирование по умолчанию
        isScannerActive = true;
        Debug.Log("Scanning enabled by default");
    }

    public void ToggleScan()
    {
        Debug.Log("ItemScanner.ToggleScan() called");
        isScannerActive = !isScannerActive;
        Debug.Log("isScannerActive set to: " + isScannerActive);
    }

    void ScanForItems()
    {
        // Удаляем старые иконки
        foreach (GameObject icon in radarIcons)
        {
            Destroy(icon);
        }
        radarIcons.Clear();

        // Ищем все ItemPickup в радиусе
        Collider[] colliders = Physics.OverlapSphere(transform.position, scanRadius);
        foreach (Collider collider in colliders)
        {
            ItemPickup itemPickup = collider.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                // Создаем иконку на радаре
                GameObject radarIcon = Instantiate(radarIconPrefab, radarImage.transform);
                radarIcons.Add(radarIcon);

                // Вычисляем позицию иконки на радаре
                Vector3 itemPosition = itemPickup.transform.position;
                Vector3 direction = itemPosition - transform.position;
                float distance = direction.magnitude;
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                // Преобразуем координаты мира в координаты радара
                float radarX = distance * Mathf.Cos(angle * Mathf.Deg2Rad) / scanRadius;
                float radarY = distance * Mathf.Sin(angle * Mathf.Deg2Rad) / scanRadius;

                // Устанавливаем позицию иконки на радаре
                radarIcon.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(
                        radarX * radarImage.rectTransform.rect.width / 2,
                        radarY * radarImage.rectTransform.rect.height / 2
                    );

                // Получаем иконку предмета из ItemPickup
                Image iconImage = radarIcon.GetComponent<Image>();
                if (iconImage != null)
                {
                    // Получаем иконку из объекта Item
                    if (itemPickup.item != null)
                    {
                        iconImage.sprite = radarIconSprite;
                        iconImage.SetNativeSize();
                    }
                    else
                    {
                        Debug.LogWarning("ItemPickup не имеет назначенного Item!");
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }
}