// using UnityEngine;
// using UnityEngine.InputSystem;

// public class InventoryUI : MonoBehaviour
// {
//     public static InventoryUI Instance;

//     [Header("Referências")]
//     [SerializeField] private GameObject inventoryPanel;
//     [SerializeField] private GameObject optionsPanel;

//     private ItemData selectedItem;
//     private bool aberto = false;

//     void Awake()
//     {
//         // 🔒 garante apenas uma instância
//         if (Instance == null)
//             Instance = this;
//         else
//             Destroy(gameObject);

//         // 🔥 começa tudo fechado
//         inventoryPanel.SetActive(false);
//         optionsPanel.SetActive(false);
//     }

//     void Update()
//     {
//         // 🔓 abrir/fechar inventário
//         if (Keyboard.current.iKey.wasPressedThisFrame)
//         {
//             ToggleInventory();
//         }

//         // ❌ fechar opções com ESC
//         if (Keyboard.current.escapeKey.wasPressedThisFrame)
//         {
//             HideOptions();
//         }
//     }

//     void ToggleInventory()
//     {
//         aberto = !aberto;

//         inventoryPanel.SetActive(aberto);

//         // pausa jogo
//         Time.timeScale = aberto ? 0f : 1f;

//         // 🔥 sempre esconde opções ao fechar
//         if (!aberto)
//             HideOptions();
//     }

//     // ================================
//     // 📦 MENU DE OPÇÕES DO ITEM
//     // ================================

//     public void ShowItemOptions(ItemData item)
//     {
//         if (item == null) return;

//         selectedItem = item;

//         optionsPanel.SetActive(true);

//         // 🔥 opcional: aparece no mouse
//         Vector2 mousePos = Mouse.current.position.ReadValue();
//         optionsPanel.transform.position = mousePos;
//     }

//     public void HideOptions()
//     {
//         optionsPanel.SetActive(false);
//         selectedItem = null;
//     }

//     // ================================
//     // 🔥 AÇÕES DOS BOTÕES
//     // ================================

//     public void UseItem()
//     {
//         if (selectedItem == null) return;

//         if (selectedItem.isDiaryPage)
//         {
//             DiaryUI.Instance.ShowPage(selectedItem.description);
//         }
//         else
//         {
//             Debug.Log("Usando item: " + selectedItem.description);
//         }

//         HideOptions();
//     }

//     public void ExamineItem()
//     {
//         if (selectedItem == null) return;

//         Debug.Log("Examinando: " + selectedItem.description);
//     }

//     public void DropItem()
//     {
//         if (selectedItem == null) return;

//         Debug.Log("Descartado: " + selectedItem.description);

//         // 👉 se quiser remover de verdade:
//         InventorySystem.Instance.RemoveItem(selectedItem);

//         HideOptions();
//     }
// }




// using UnityEngine;
// using UnityEngine.InputSystem;

// public class InventoryUI : MonoBehaviour
// {
//     public static InventoryUI Instance;

//     [Header("Referências")]
//     [SerializeField] private GameObject inventoryPanel;
//     [SerializeField] private GameObject optionsPanel;

//     private ItemData selectedItem;
//     private bool aberto = false;

//     void Awake()
//     {
//         if (Instance == null)
//             Instance = this;
//         else
//             Destroy(gameObject);

//         inventoryPanel.SetActive(false);
//         optionsPanel.SetActive(false);
//     }

//     void Update()
//     {
//         if (Keyboard.current.iKey.wasPressedThisFrame)
//         {
//             ToggleInventory();
//         }

//         if (Keyboard.current.escapeKey.wasPressedThisFrame)
//         {
//             CloseAll();
//         }
//     }

//     void ToggleInventory()
//     {
//         aberto = !aberto;

//         inventoryPanel.SetActive(aberto);
//         Time.timeScale = aberto ? 0f : 1f;

//         if (!aberto)
//             optionsPanel.SetActive(false);
//     }

//     // ================================
//     // MENU DO ITEM
//     // ================================

//     public void ShowItemOptions(ItemData item)
//     {
//         if (item == null) return;

//         selectedItem = item;

//         optionsPanel.SetActive(true);

//         // aparece no mouse (opcional)
//         Vector2 mousePos = Mouse.current.position.ReadValue();
//         optionsPanel.transform.position = mousePos;
//     }

//     // ================================
//     // BOTÃO 1 — VER / USAR
//     // ================================

//     public void ViewItem()
//     {
//         if (selectedItem == null) return;

//         // abre diário ou descrição
//         if (selectedItem.isDiaryPage)
//         {
//             DiaryUI.Instance.ShowPage(selectedItem.description);
//         }
//         else
//         {
//             Debug.Log("Descrição: " + selectedItem.description);
//         }
//     }

//     // ================================
//     // BOTÃO 2 — FECHAR TUDO
//     // ================================

//     public void CloseAll()
//     {
//         optionsPanel.SetActive(false);
//         inventoryPanel.SetActive(false);

//         selectedItem = null;
//         aberto = false;

//         Time.timeScale = 1f;
//     }
// }




using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // 🔥 necessário para mudar texto do botão

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Referências")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("UI Botão")]
    [SerializeField] private TextMeshProUGUI viewButtonText; // 🔥 texto do botão

    private ItemData selectedItem;
    private bool aberto = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseAll();
        }
    }

    void ToggleInventory()
    {
        aberto = !aberto;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(aberto);

        Time.timeScale = aberto ? 0f : 1f;

        if (!aberto && optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    // ================================
    // MENU DO ITEM
    // ================================

    public void ShowItemOptions(ItemData item)
    {
        if (item == null) return;

        selectedItem = item;

        if (optionsPanel != null)
            optionsPanel.SetActive(true);

        // posição no mouse
        Vector2 mousePos = Mouse.current.position.ReadValue();
        optionsPanel.transform.position = mousePos;

        // 🔥 TEXTO DINÂMICO DO BOTÃO
        if (viewButtonText != null)
        {
            if (selectedItem.isDiaryPage)
                viewButtonText.text = "Ler";
            else
                viewButtonText.text = "Examinar";
        }
    }

    // ================================
    // BOTÃO 1 — VER / USAR
    // ================================

//     public void ViewItem()
// {
//     if (selectedItem == null) return;

//     // 🔥 ESCONDE O MENU ANTES DE ABRIR
//     if (optionsPanel != null)
//         optionsPanel.SetActive(false);

//     if (selectedItem.isDiaryPage)
//     {
//         DiaryUI.Instance.ShowPage(selectedItem.description);
//     }
//     else
//     {
//         Debug.Log("Descrição: " + selectedItem.description);
//     }
// }

// public void ViewItem()
// {
//     if (selectedItem == null) return;

//     // fecha inventário e menu
//     inventoryPanel.SetActive(false);
//     optionsPanel.SetActive(false);
//     aberto = false;

//     if (selectedItem.isDiaryPage)
//     {
//         DiaryUI.Instance.ShowPage(selectedItem.description);
//     }
//     else
//     {
//         ItemDescriptionUI.Instance.Show(selectedItem); // 🔥 NOVO
//     }
// }
public void ViewItem()
{
    Debug.Log("Clicou em ViewItem");

    if (selectedItem == null)
    {
        Debug.LogError("selectedItem está NULL");
        return;
    }

    inventoryPanel.SetActive(false);
    optionsPanel.SetActive(false);
    aberto = false;

    if (selectedItem.isDiaryPage)
    {
        if (DiaryUI.Instance == null)
        {
            Debug.LogError("DiaryUI está NULL");
            return;
        }

        DiaryUI.Instance.ShowPage(selectedItem.description);
    }
    else
    {
        if (ItemDescriptionUI.Instance == null)
        {
            Debug.LogError("ItemDescriptionUI está NULL");
            return;
        }

        ItemDescriptionUI.Instance.Show(selectedItem);
    }
}





    // ================================
    // BOTÃO 2 — FECHAR TUDO
    // ================================

    public void CloseAll()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        selectedItem = null;
        aberto = false;

        Time.timeScale = 1f;
    }
}