using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    private ItemData item;

    public void SetItem(ItemData newItem)
    {
        item = newItem;
    }

    public ItemData GetItem() => item;

    public void OnClick()
    {
        if (item == null) return;
        InventoryUI.Instance.ShowItemOptions(item);
    }
}
