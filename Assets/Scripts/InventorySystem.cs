// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class InventorySystem : MonoBehaviour
// {
//     public static InventorySystem Instance;

//     [SerializeField] private List<Image> slots = new List<Image>();

//     private List<ItemData> itens = new List<ItemData>();

//     private int selectedIndex = -1;

//     void Awake()
//     {
//         Instance = this;
//     }

//     public void AddItem(ItemData item)
//     {
//         if (itens.Count >= slots.Count)
//         {
//             Debug.Log("Inventário cheio");
//             return;
//         }

//         itens.Add(item);

//         int index = itens.Count - 1;

//         slots[index].sprite = item.icon;
//         slots[index].color = Color.white;

//         Button btn = slots[index].GetComponent<Button>();

//         if (btn != null)
//         {
//             int slotIndex = index;

//             btn.onClick.RemoveAllListeners();
//            //btn.onClick.AddListener(() => UseItem(slotIndex));
//             btn.onClick.AddListener(() => SelectItem(slotIndex));
//         }
//     }

//     void UseItem(int index)
//     {
//         ItemData item = itens[index];

//         if (item.isDiaryPage)
//         {
//             DiaryUI.Instance.ShowPage(item.description);
//         }
//     }

//     // 🔑 NOVO HAS ITEM
//     public bool HasItem(ItemData item)
//     {
//         return itens.Contains(item);
//     }

//     // 🔑 NOVO REMOVE ITEM
//     public void RemoveItem(ItemData item)
//     {
//         if (!itens.Contains(item)) return;

//         itens.Remove(item);

//         // atualiza UI
//         for (int i = 0; i < slots.Count; i++)
//         {
//             if (i < itens.Count)
//             {
//                 slots[i].sprite = itens[i].icon;
//                 slots[i].color = Color.white;
//             }
//             else
//             {
//                 slots[i].sprite = null;
//                 slots[i].color = new Color(1,1,1,0);
//             }
//         }
//     }


//     public void SelectItem(int index)
//     {
//         selectedIndex = index;

//         Debug.Log("Selecionado: " + itens[index].description);

//         InventoryUI.Instance.ShowItemOptions(itens[index]);
//     }


// }


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
    // public void AddItem(ItemData item)
    // {
    //     if (itens.Count >= slots.Count)
    //     {
    //         Debug.Log("Inventário cheio");
    //         return;
    //     }

    //     itens.Add(item);

    //     int index = itens.Count - 1;

    //     slots[index].sprite = item.icon;
    //     slots[index].color = Color.white;

    //     Button btn = slots[index].GetComponent<Button>();

    //     if (btn != null)
    //     {
    //         int slotIndex = index;

    //         btn.onClick.RemoveAllListeners();
    //         btn.onClick.AddListener(() => SelectItem(slotIndex));
    //     }
    // }
        public void AddItem(ItemData item)
{
    if (item == null)
    {
        Debug.LogError("Item NULL!");
        return;
    }

    if (itens.Count >= slots.Count)
    {
        Debug.Log("Inventário cheio");
        return;
    }

    itens.Add(item);

    int index = itens.Count - 1;

    if (slots[index] == null)
    {
        Debug.LogError("Slot não atribuído no Inspector!");
        return;
    }

    if (item.icon == null)
    {
        Debug.LogError("Item sem ícone!");
    }

    slots[index].sprite = item.icon;
    slots[index].color = Color.white;

    Button btn = slots[index].GetComponent<Button>();

    if (btn != null)
    {
        int slotIndex = index;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => SelectItem(slotIndex));
    }
}





    // ================================
    // VERIFICAR ITEM
    // ================================
    public bool HasItem(ItemData item)
    {
        return itens.Contains(item);
    }

    // ================================
    // REMOVER ITEM
    // ================================
    public void RemoveItem(ItemData item)
    {
        if (!itens.Contains(item)) return;

        itens.Remove(item);

        UpdateUI();
    }

    // ================================
    // ATUALIZAR UI
    // ================================
    // void UpdateUI()
    // {
    //     for (int i = 0; i < slots.Count; i++)
    //     {
    //         if (i < itens.Count)
    //         {
    //             slots[i].sprite = itens[i].icon;
    //             slots[i].color = Color.white;
    //         }
    //         else
    //         {
    //             slots[i].sprite = null;
    //             slots[i].color = new Color(1,1,1,0);
    //         }
    //     }
    // }

        void UpdateUI()
{
    for (int i = 0; i < slots.Count; i++)
    {
        if (slots[i] == null)
        {
            Debug.LogError("Slot NULL no índice: " + i);
            continue;
        }

        if (i < itens.Count)
        {
            if (itens[i] == null)
            {
                Debug.LogError("Item NULL no índice: " + i);
                continue;
            }

            slots[i].sprite = itens[i].icon;
            slots[i].color = Color.white;
        }
        else
        {
            slots[i].sprite = null;
            slots[i].color = new Color(1,1,1,0);
        }
    }
}


    // ================================
    // SELECIONAR ITEM
    // ================================
    public void SelectItem(int index)
    {
        if (index < 0 || index >= itens.Count) return;

        InventoryUI.Instance.ShowItemOptions(itens[index]);
    }
}