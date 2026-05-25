using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [SerializeField] private List<Image> slots = new List<Image>();

    private List<ItemData> itens = new List<ItemData>();

    void Awake()
    {
        Instance = this;
    }

    // ================================
    // ADICIONAR ITEM
    // ================================

    public void AddItem(ItemData item)
    {
        if (item == null) { Debug.LogError("Item NULL!"); return; }
        if (itens.Count >= slots.Count) { Debug.Log("Inventário cheio"); return; }

        itens.Add(item);
        int index = itens.Count - 1;

        if (slots[index] == null) { Debug.LogError("Slot não atribuído!"); return; }
        if (item.icon == null) Debug.LogError("Item sem ícone: " + item.itemName);

        slots[index].sprite = item.icon;
        slots[index].color  = Color.white;

        Button btn = slots[index].GetComponent<Button>();
        if (btn != null)
        {
            int i = index;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SelectItem(i));
        }
    }

    // ================================
    // VERIFICAR / REMOVER
    // ================================

    public bool HasItem(ItemData item)   => itens.Contains(item);

    public void RemoveItem(ItemData item)
    {
        if (!itens.Contains(item)) return;
        itens.Remove(item);
        UpdateUI();
    }

    // ================================
    // ATUALIZAR UI
    // ================================

    void UpdateUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;

            if (i < itens.Count && itens[i] != null)
            {
                slots[i].sprite = itens[i].icon;
                slots[i].color  = Color.white;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].color  = new Color(1, 1, 1, 0);
            }
        }
    }

    // ================================
    // SELECIONAR ITEM (chamado pelo botão OU pelo InventoryUI via controle)
    // ================================

    public void SelectItem(int index)
    {
        if (index < 0 || index >= itens.Count) return;
        InventoryUI.Instance.ShowItemOptions(itens[index]);
    }

    // ================================
    // UTILITÁRIOS para o InventoryUI
    // ================================

    public int SlotCount  => slots.Count;
    public int ItemCount  => itens.Count;

    public Image GetSlotImage(int index)
    {
        if (index < 0 || index >= slots.Count) return null;
        return slots[index];
    }

    public bool SlotHasItem(int index) => index >= 0 && index < itens.Count;

    public Color GetSlotOriginalColor(int index)
    {
        // slots vazios têm alpha 0, slots com item têm alpha 1
        return SlotHasItem(index) ? Color.white : new Color(1, 1, 1, 0);
    }
}
