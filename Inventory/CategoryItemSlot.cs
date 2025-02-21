using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CategoryItemSlot : MonoBehaviour
{
    public Image itemIcon;
    public Text itemQuantity; // ���� ����� ���������� ����������

    public void SetItem(Item item)
    {
        if (itemIcon != null && item != null)
        {
            itemIcon.sprite = item.icon;
        }

        // ���� ����� ���������� ����������:
        // if (itemQuantity != null && item.���������� > 1)
        // {
        //   itemQuantity.text = item.����������.ToString();
        // } else {
        //   itemQuantity.text = ""; // ��� ������ Text ������
        // }
    }
}