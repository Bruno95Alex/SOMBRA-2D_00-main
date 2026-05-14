using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Referências")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("UI Botão")]
    [SerializeField] private TextMeshProUGUI viewButtonText;

    private ItemData selectedItem;
    private bool aberto = false;

    [SerializeField] private InventorySlotUI[] slots;

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
            ToggleInventory();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseAll();
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

        Vector2 mousePos = Mouse.current.position.ReadValue();
        optionsPanel.transform.position = mousePos;

        if (viewButtonText != null)
            viewButtonText.text = selectedItem.isDiaryPage ? "Ler" : "Examinar";
    }

    // ================================
    // BOTÃO — VER / USAR
    // ================================

    public void ViewItem()
    {
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
    // FECHAR TUDO
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
