using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public string description = "Add a Description";
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public ItemCategory category; // ��������� ���������
    public bool isCraftingMaterial = false; // �������� �� ������� ���������� ��� ������
    public ItemUnlockData unlockData;

    public GameObject itemPrefab; // **��������� ��� ����**

    public virtual void Equip()
    {
        Debug.Log("�����������: " + name);
        //���������� ������� ������ ���������� �����, ���� ����������.
        //��������, ����� ������� �������, ������� ����� �������������� ������� ���������.
    }

    public virtual void Unequip()
    {
        Debug.Log("�����: " + name);
        //���������� ������� ������ ������ ���������� �����, ���� ����������.
    }

    public virtual void Use()
    {
        Debug.Log("������������: " + name);
    }
    public void RemoveFromInventory()
    {
        //TODO: Implement logic to remove item from inventory.
    }
}