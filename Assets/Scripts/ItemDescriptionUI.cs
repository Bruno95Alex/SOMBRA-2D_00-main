using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class ItemDescriptionUI : MonoBehaviour
{
    public static ItemDescriptionUI Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private Button closeButton;

    private bool visivel   = false;
    private bool bloqueado = false; // evita fechar no mesmo frame que abriu

    void Awake()
    {
        Instance = this;
        HideImmediate();
    }

    void Update()
    {
        if (!visivel || bloqueado) return;

        bool fechar = (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                   || (InputReader.Instance != null && InputReader.Instance.InventoryPressed)
                   || (InputReader.Instance != null && InputReader.Instance.InteractPressed);

        if (fechar) Hide();
    }

    public void Show(ItemData item)
    {
        if (item == null) { Debug.LogError("Item nulo na descrição"); return; }

        if (canvasGroup == null || itemImage == null || itemText == null)
        {
            Debug.LogError("Referências não ligadas no ItemDescriptionUI");
            return;
        }

        canvasGroup.alpha          = 1;
        canvasGroup.interactable   = true;
        canvasGroup.blocksRaycasts = true;

        itemImage.sprite = item.icon;
        itemText.text    = item.description;
        visivel          = true;

        if (closeButton != null)
            closeButton.image.color = new Color(1f, 0.85f, 0.2f, 1f);

        StartCoroutine(BloquearPorUmFrame());
    }

    IEnumerator BloquearPorUmFrame()
    {
        bloqueado = true;
        yield return null;
        yield return null;
        bloqueado = false;
    }

    public void Hide()
    {
        HideImmediate();
        visivel = false;
    }

    private void HideImmediate()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha          = 0;
        canvasGroup.interactable   = false;
        canvasGroup.blocksRaycasts = false;

        if (closeButton != null)
            closeButton.image.color = Color.white;
    }
}
