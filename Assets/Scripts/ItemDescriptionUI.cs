using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDescriptionUI : MonoBehaviour
{
    public static ItemDescriptionUI Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemText;

    void Awake()
    {
        Instance = this;

        // começa invisível sem quebrar o jogo
        HideImmediate();
    }

    // ================================
    // MOSTRAR DESCRIÇÃO
    // ================================
    public void Show(ItemData item)
    {
        if (item == null)
        {
            Debug.LogError("Item nulo na descrição");
            return;
        }

        if (canvasGroup == null || itemImage == null || itemText == null)
        {
            Debug.LogError("Referências não ligadas no ItemDescriptionUI");
            return;
        }

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        itemImage.sprite = item.icon;
        itemText.text = item.description;
    }

    // ================================
    // FECHAR (BOTÃO)
    // ================================
    public void Hide()
    {
        HideImmediate();

        // 🔥 IMPORTANTE: garante que o jogo volta ao normal
        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.CloseAll();
        }
        else
        {
            // fallback caso InventoryUI não exista
            Time.timeScale = 1f;
        }
    }

    // ================================
    // ESCONDER SEM LÓGICA EXTRA
    // ================================
    private void HideImmediate()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}