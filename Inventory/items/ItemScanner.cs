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
    public float scanInterval = 0.1f; // �������� ������������ � ��������

    private List<GameObject> radarIcons = new List<GameObject>();
    private bool isScannerUnlocked = false;
    private bool isScannerActive = false;

    void Start()
    {
        Debug.Log("ItemScanner.Start() called");

        // �������� Canvas � Start()
        if (radarImage != null && radarImage.canvas != null)
        {
            radarImage.canvas.gameObject.SetActive(true);
            Debug.Log("Radar Canvas enabled at start");
        }
        else
        {
            Debug.LogError("RadarImage �� �������� ��� Canvas RadarImage �� ������!");
        }
        isScannerUnlocked = true;

        // ��������� ������������ � ����������
        InvokeRepeating("ScanForItems", 0f, scanInterval);
    }

    void OnDestroy()
    {
        // ������������� ������������� �����, ����� �������� ������
        CancelInvoke("ScanForItems");
    }

    void Update()
    {
        // ���������, ������������� �� ������
        if (isScannerUnlocked)
        {
            // ��������/��������� ������������ �� ������� Q
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

        // ������ �� ����� �������� Canvas �����. �� ��� ������� � Start()
        Debug.Log("Canvas enabling moved to Start()");

        // �������� ������������ �� ���������
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
        // ������� ������ ������
        foreach (GameObject icon in radarIcons)
        {
            Destroy(icon);
        }
        radarIcons.Clear();

        // ���� ��� ItemPickup � �������
        Collider[] colliders = Physics.OverlapSphere(transform.position, scanRadius);
        foreach (Collider collider in colliders)
        {
            ItemPickup itemPickup = collider.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                // ������� ������ �� ������
                GameObject radarIcon = Instantiate(radarIconPrefab, radarImage.transform);
                radarIcons.Add(radarIcon);

                // ��������� ������� ������ �� ������
                Vector3 itemPosition = itemPickup.transform.position;
                Vector3 direction = itemPosition - transform.position;
                float distance = direction.magnitude;
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                // ����������� ���������� ���� � ���������� ������
                float radarX = distance * Mathf.Cos(angle * Mathf.Deg2Rad) / scanRadius;
                float radarY = distance * Mathf.Sin(angle * Mathf.Deg2Rad) / scanRadius;

                // ������������� ������� ������ �� ������
                radarIcon.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(
                        radarX * radarImage.rectTransform.rect.width / 2,
                        radarY * radarImage.rectTransform.rect.height / 2
                    );

                // �������� ������ �������� �� ItemPickup
                Image iconImage = radarIcon.GetComponent<Image>();
                if (iconImage != null)
                {
                    // �������� ������ �� ������� Item
                    if (itemPickup.item != null)
                    {
                        iconImage.sprite = radarIconSprite;
                        iconImage.SetNativeSize();
                    }
                    else
                    {
                        Debug.LogWarning("ItemPickup �� ����� ������������ Item!");
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