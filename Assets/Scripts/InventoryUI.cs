using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Painéis")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject optionsPanel;      // ItemOptionsPanel

    [Header("Botões do ItemOptionsPanel")]
    [SerializeField] private Button viewButton;            // ViewButton
    [SerializeField] private Button closeButton;          // CloseButton
    [SerializeField] private TextMeshProUGUI viewButtonText;

    [Header("Grade")]
    [SerializeField] private int colunasGrade = 4;

    [Header("Cor de seleção")]
    [SerializeField] private Color corSelecionado = new Color(1f, 0.85f, 0.2f, 1f);

    private ItemData selectedItem;
    private bool aberto = false;

    // navegação slots
    private int slotAtual = 0;
    private bool navegandoComControle = false;
    private float inputDelay = 0f;
    private const float INPUT_DELAY = 0.2f;

    // navegação opções (0 = ViewButton, 1 = CloseButton)
    private int opcaoAtual = 0;
    private bool opcoesAbertas = false;
    private float opcaoDelay = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (optionsPanel   != null) optionsPanel.SetActive(false);
    }

    void Update()
    {
        // ABRIR / FECHAR — Bola ou I
        bool togglePressed = InputReader.Instance != null
            ? InputReader.Instance.InventoryPressed
            : Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame;

        if (togglePressed)
        {
            if (opcoesAbertas) { FecharOpcoes(); return; }
            ToggleInventory();
        }

        if (!aberto) return;

        // ESCAPE fecha
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (opcoesAbertas) FecharOpcoes();
            else CloseAll();
            return;
        }

        if (opcoesAbertas)
        {
            // navegação dentro do ItemOptionsPanel
            NavegerOpcoes();
        }
        else
        {
            // navegação entre slots
            NavegerSlots();

            // TRIÂNGULO seleciona slot
            bool confirmar = InputReader.Instance != null && InputReader.Instance.InteractPressed;
            if (confirmar && navegandoComControle)
                AbrirOpcoesSlotAtual();
        }
    }

    // =========================
    // NAVEGAÇÃO SLOTS
    // =========================

    void NavegerSlots()
    {
        inputDelay -= Time.unscaledDeltaTime;
        if (inputDelay > 0f) return;

        var js = Joystick.current;
        var kb = Keyboard.current;
        float h = 0f, v = 0f;

        if (js != null)
        {
            Vector2 stick = js.stick.ReadValue();
            if (stick.magnitude > 0.5f) { h = stick.x; v = stick.y; }
        }

        if (h == 0f && v == 0f && kb != null)
        {
            if (kb.rightArrowKey.isPressed) h =  1f;
            if (kb.leftArrowKey.isPressed)  h = -1f;
            if (kb.upArrowKey.isPressed)    v =  1f;
            if (kb.downArrowKey.isPressed)  v = -1f;
        }

        if (h == 0f && v == 0f) return;

        int total    = InventorySystem.Instance.SlotCount;
        int novoSlot = slotAtual;

        if (Mathf.Abs(h) >= Mathf.Abs(v))
            novoSlot += h > 0f ? 1 : -1;
        else
            novoSlot += v < 0f ? colunasGrade : -colunasGrade;

        novoSlot = Mathf.Clamp(novoSlot, 0, total - 1);

        if (novoSlot != slotAtual)
        {
            RestaurarSlot(slotAtual);
            slotAtual = novoSlot;
            MarcarSlot(slotAtual);
            navegandoComControle = true;
            inputDelay = INPUT_DELAY;
        }
    }

    void MarcarSlot(int index)
    {
        var img = InventorySystem.Instance.GetSlotImage(index);
        if (img == null) return;

        // só destaca se tiver item
        if (InventorySystem.Instance.SlotHasItem(index))
            img.color = corSelecionado;
    }

    void RestaurarSlot(int index)
    {
        var img = InventorySystem.Instance.GetSlotImage(index);
        if (img == null) return;
        img.color = InventorySystem.Instance.GetSlotOriginalColor(index);
    }

    void AbrirOpcoesSlotAtual()
    {
        InventorySystem.Instance.SelectItem(slotAtual);
    }

    // =========================
    // NAVEGAÇÃO OPÇÕES (ViewButton / CloseButton)
    // =========================

    void NavegerOpcoes()
    {
        opcaoDelay -= Time.unscaledDeltaTime;
        if (opcaoDelay > 0f) return;

        var js = Joystick.current;
        var kb = Keyboard.current;
        float v = 0f;

        if (js != null)
        {
            float stick = js.stick.ReadValue().y;
            if (Mathf.Abs(stick) > 0.5f) v = stick;
        }

        if (v == 0f && kb != null)
        {
            if (kb.upArrowKey.isPressed)   v =  1f;
            if (kb.downArrowKey.isPressed) v = -1f;
        }

        if (v != 0f)
        {
            opcaoAtual = opcaoAtual == 0 ? 1 : 0; // alterna entre 0 e 1
            AtualizarDestaqueBotoes();
            opcaoDelay = INPUT_DELAY;
        }

        // TRIÂNGULO confirma opção
        bool confirmar = InputReader.Instance != null && InputReader.Instance.InteractPressed;
        if (confirmar)
        {
            if (opcaoAtual == 0) ViewItem();
            else FecharOpcoes();
        }
    }

    void AtualizarDestaqueBotoes()
    {
        if (viewButton  != null) viewButton.image.color  = opcaoAtual == 0 ? corSelecionado : Color.white;
        if (closeButton != null) closeButton.image.color = opcaoAtual == 1 ? corSelecionado : Color.white;
    }

    void FecharOpcoes()
    {
        opcoesAbertas = false;
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // restaura cores dos botões
        if (viewButton  != null) viewButton.image.color  = Color.white;
        if (closeButton != null) closeButton.image.color = Color.white;
    }

    // =========================
    // TOGGLE
    // =========================

    void ToggleInventory()
    {
        aberto = !aberto;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(aberto);

        Time.timeScale = aberto ? 0f : 1f;

        if (!aberto)
        {
            RestaurarSlot(slotAtual);
            FecharOpcoes();
        }
        else
        {
            // seleciona primeiro slot com item ao abrir
            slotAtual = 0;
            navegandoComControle = true;
            MarcarSlot(slotAtual);
        }
    }

    // =========================
    // MOSTRAR OPÇÕES DO ITEM
    // =========================

    public void ShowItemOptions(ItemData item)
    {
        if (item == null) return;

        selectedItem  = item;
        opcoesAbertas = true;
        opcaoAtual    = 0;

        if (optionsPanel != null)
            optionsPanel.SetActive(true);

        // posiciona perto do slot selecionado ou do mouse
        if (navegandoComControle)
        {
            var img = InventorySystem.Instance.GetSlotImage(slotAtual);
            if (img != null)
                optionsPanel.transform.position = img.transform.position + new Vector3(90f, 0f, 0f);
        }
        else if (Mouse.current != null)
        {
            optionsPanel.transform.position = (Vector3)Mouse.current.position.ReadValue();
        }

        AtualizarDestaqueBotoes();

        if (viewButtonText != null)
            viewButtonText.text = selectedItem.isDiaryPage ? "Ler" : "Examinar";
    }

    // =========================
    // VER ITEM
    // =========================

    public void ViewItem()
    {
        if (selectedItem == null) return;

        ItemData item = selectedItem;

        if (item.isDiaryPage)
        {
            // diário: fecha inventário e abre página
            CloseAll();
            if (DiaryUI.Instance != null)
                DiaryUI.Instance.ShowPage(item.description);
        }
        else
        {
            // item normal: só esconde os painéis visualmente mas mantém estado
            // ItemDescriptionUI fecha sozinho depois sem conflito
            EsconderPaineis();
            if (ItemDescriptionUI.Instance != null)
                ItemDescriptionUI.Instance.Show(item);
        }
    }

    // =========================
    // ESCONDER PAINÉIS SEM RESETAR ESTADO
    // usado quando abre ItemDescriptionUI — mantém inventário pronto para voltar
    // =========================

    void EsconderPaineis()
    {
        RestaurarSlot(slotAtual);
        FecharOpcoes();
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (optionsPanel   != null) optionsPanel.SetActive(false);
        selectedItem         = null;
        aberto               = false;
        navegandoComControle = false;
        Time.timeScale       = 1f;
    }

    // =========================
    // FECHAR TUDO
    // =========================

    public void CloseAll()
    {
        RestaurarSlot(slotAtual);
        FecharOpcoes();

        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        selectedItem         = null;
        aberto               = false;
        navegandoComControle = false;
        Time.timeScale       = 1f;
    }
}
