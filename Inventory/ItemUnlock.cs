using UnityEngine;

public class ItemUnlock : MonoBehaviour
{
    public enum UnlockType
    {
        Scanner,
        //  ����� �������� � ������ ���� ������������� � ������� (��������, Ability, Weapon, � �.�.)
    }

    public UnlockType unlockType; // ��� �������������
    public Sprite icon;         // ������ ������ ��� ������

    public void Unlock(Transform player)
    {
        switch (unlockType)
        {
            case UnlockType.Scanner:
                UnlockScanner(player);
                break;
                // ��������� ������ ������ ��� ������ ����� �������������
        }

        // ���������� UI �������������
        ScannerUnlockUI[] unlockUIComponents = Resources.FindObjectsOfTypeAll<ScannerUnlockUI>();
        if (unlockUIComponents != null && unlockUIComponents.Length > 0)
        {
            // ���������, ������� �� GameObject
            if (unlockUIComponents[0].gameObject.activeInHierarchy)
            {
                unlockUIComponents[0].ShowUnlockUI();
            }
            else
            {
                Debug.LogWarning("������ ScannerUnlockUI �� ������� � ��������.");
            }
        }
        else
        {
            Debug.LogWarning("�� ������ ��������� ScannerUnlockUI � �����.");
        }
    }

    private void UnlockScanner(Transform player)
    {
        ItemScanner scanner = player.GetComponent<ItemScanner>();
        if (scanner != null)
        {
            scanner.enabled = true; // �������� ������ �������
            scanner.ToggleScan();  // �������� ������
            SetRadarIcon(player); // ������������� ������ ��� �������
        }
        else
        {
            Debug.LogWarning("�� ������ ��������� ItemScanner �� ������.");
        }
    }

    private void SetRadarIcon(Transform player)
    {
        ItemScanner scanner = player.GetComponent<ItemScanner>();
        if (scanner != null)
        {
            // ������ � ��� ���� ������ � icon �� ItemUnlock
            // �� ��� ����� �������� ��� ������ � ItemScanner, ����� �� ����������� ��
            // ����� ������� ������ - �������� public Sprite icon � ItemScanner
            scanner.radarIconSprite = icon;
        }
        else
        {
            Debug.LogWarning("�� ������ ��������� ItemScanner �� ������.");
        }
    }
}