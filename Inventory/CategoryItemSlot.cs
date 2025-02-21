using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CategoryItemSlot : MonoBehaviour
{
    public Image itemIcon;
    public Text itemQuantity; // Если нужно отображать количество

    public void SetItem(Item item)
    {
        if (itemIcon != null && item != null)
        {
            itemIcon.sprite = item.icon;
        }

        // Если нужно отображать количество:
        // if (itemQuantity != null && item.количество > 1)
        // {
        //   itemQuantity.text = item.количество.ToString();
        // } else {
        //   itemQuantity.text = ""; // Или скрыть Text объект
        // }
    }
}