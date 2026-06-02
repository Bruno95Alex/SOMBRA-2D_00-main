using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlotSelectUI : MonoBehaviour
{
    public static SlotSelectUI Instance;
    public enum SlotMode { NovoJogo, Salvar, Carregar }

    [Header("SlotPanel")]
    [SerializeField] private GameObject      slotPanel;
    [SerializeField] private TextMeshProUGUI titulo;
    [SerializeField] private Button[]          botoesSlot;
    [SerializeField] private TextMeshProUGUI[] textosSlot;
    [SerializeField] private Button          btnConfirmar;
    [SerializeField] private TextMeshProUGUI btnConfirmarText;
    [SerializeField] private Button          btnVoltar;

    [Header("ConfirmPanel (Sim/Não)")]
    [SerializeField] private GameObject      confirmPanel;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private Button          btnSim;
    [SerializeField] private Button          btnNao;

    [Header("Cores")]
    [SerializeField] private Color corSelecionado = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private Color corVazio       = new Color(0.6f, 0.6f, 0.6f, 1f);
    [SerializeField] private Color corOcupado     = Color.white;

    private SlotMode modoAtual;
    private int      slotSelecionado = -1;
    private string   cenaJogo = "Scene1";

    void Awake()
    {
        Instance = this;

        if (slotPanel    != null) slotPanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);

        // conecta botões por código — garante que chamam esta instância
        if (btnConfirmar != null)
        {
            btnConfirmar.onClick.RemoveAllListeners();
            btnConfirmar.onClick.AddListener(Confirmar);
        }

        if (btnVoltar != null)
        {
            btnVoltar.onClick.RemoveAllListeners();
            btnVoltar.onClick.AddListener(Voltar);
        }

        if (btnSim != null)
        {
            btnSim.onClick.RemoveAllListeners();
            btnSim.onClick.AddListener(ConfirmarSim);
        }

        if (btnNao != null)
        {
            btnNao.onClick.RemoveAllListeners();
            btnNao.onClick.AddListener(ConfirmarNao);
        }

        for (int i = 0; i < botoesSlot.Length; i++)
        {
            int index = i;
            if (botoesSlot[i] != null)
            {
                botoesSlot[i].onClick.RemoveAllListeners();
                botoesSlot[i].onClick.AddListener(() => SelecionarSlot(index));
            }
        }
    }

    // ================================
    // ABRIR / FECHAR
    // ================================

    public void Abrir(SlotMode modo, string cena = "Scene1")
    {
        modoAtual       = modo;
        cenaJogo        = cena;
        slotSelecionado = -1;

        if (confirmPanel != null) confirmPanel.SetActive(false);
        if (slotPanel    != null) slotPanel.SetActive(true);

        if (titulo != null)
            titulo.text = modo == SlotMode.NovoJogo ? "Escolha o slot"
                        : modo == SlotMode.Salvar   ? "Salvar no slot"
                        :                             "Escolha o save";

        if (btnConfirmarText != null)
            btnConfirmarText.text = modo == SlotMode.NovoJogo ? "Começar"
                                  : modo == SlotMode.Salvar   ? "Salvar"
                                  :                             "Carregar";

        if (btnConfirmar != null) btnConfirmar.interactable = false;

        AtualizarSlots();
    }

    public void Fechar()
    {
        if (slotPanel    != null) slotPanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);
        slotSelecionado = -1;
    }

    // ================================
    // SLOTS
    // ================================

    void AtualizarSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            bool temSave = SaveSystem.Instance != null && SaveSystem.Instance.TemSave(i);

            if (i < textosSlot.Length && textosSlot[i] != null)
                textosSlot[i].text = temSave
                    ? $"Save {i+1:00}  —  {SaveSystem.Instance.DataDoSave(i)}"
                    : $"Save {i+1:00}  —  Vazio";

            if (i < botoesSlot.Length && botoesSlot[i] != null)
            {
                botoesSlot[i].interactable = modoAtual == SlotMode.Carregar ? temSave : true;
                var img = botoesSlot[i].GetComponent<Image>();
                if (img != null) img.color = temSave ? corOcupado : corVazio;
            }
        }
    }

    void SelecionarSlot(int index)
    {
        slotSelecionado = index;
        Debug.Log($"[Slot] Selecionado: {index} | modo: {modoAtual}");

        for (int i = 0; i < botoesSlot.Length; i++)
        {
            if (botoesSlot[i] == null) continue;
            bool temSave = SaveSystem.Instance != null && SaveSystem.Instance.TemSave(i);
            botoesSlot[i].GetComponent<Image>().color =
                i == index ? corSelecionado : (temSave ? corOcupado : corVazio);
        }

        if (btnConfirmar != null) btnConfirmar.interactable = true;
    }

    // ================================
    // CONFIRMAR
    // ================================

    void Confirmar()
    {
        if (slotSelecionado < 0) return;

        bool ocupado = SaveSystem.Instance != null &&
                       SaveSystem.Instance.TemSave(slotSelecionado);

        Debug.Log($"[Slot] Confirmar — slot={slotSelecionado} ocupado={ocupado} modo={modoAtual}");

        if (modoAtual == SlotMode.Carregar || !ocupado)
        {
            Executar(slotSelecionado, modoAtual, cenaJogo);
        }
        else
        {
            AbrirConfirmPanel();
        }
    }

    void AbrirConfirmPanel()
    {
        if (slotPanel    != null) slotPanel.SetActive(false);
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(true);
            confirmPanel.transform.SetAsLastSibling();
        }

        if (confirmText != null)
        {
            string data = SaveSystem.Instance.DataDoSave(slotSelecionado);
            string acao = modoAtual == SlotMode.NovoJogo
                ? "Deseja substituir e iniciar novo jogo?"
                : "Deseja substituir este save?";
            confirmText.text = $"Save {slotSelecionado+1:00} — {data}\n{acao}";
        }
    }

    // ================================
    // SIM / NÃO
    // ================================

    void ConfirmarSim()
    {
        int      slot = slotSelecionado;
        SlotMode modo = modoAtual;
        string   cena = cenaJogo;

        Debug.Log($"[Slot] ConfirmarSim — slot={slot} modo={modo}");

        Fechar();
        Executar(slot, modo, cena);
    }

    void ConfirmarNao()
    {
        Debug.Log("[Slot] ConfirmarNao");
        if (confirmPanel != null) confirmPanel.SetActive(false);
        if (slotPanel    != null) slotPanel.SetActive(true);
    }

    // ================================
    // EXECUTAR
    // ================================

    void Executar(int slot, SlotMode modo, string cena)
    {
        Debug.Log($"[Slot] Executar — slot={slot} modo={modo} cena={cena}");

        Time.timeScale = 1f;

        if (modo == SlotMode.NovoJogo)
        {
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.DeletarSave(slot);
                SaveSystem.Instance.SlotAtivoPublico = slot;
            }
            SceneManager.LoadScene(cena);
        }
        else if (modo == SlotMode.Salvar)
        {
            if (SaveSystem.Instance != null)
                SaveSystem.Instance.Salvar(slot);

            var cam = FindFirstObjectByType<CameraController>();
            if (cam != null) cam.StopAllCoroutines();

            SceneManager.LoadScene("Menu");
        }
        else if (modo == SlotMode.Carregar)
        {
            if (SaveSystem.Instance != null)
                SaveSystem.Instance.Carregar(slot);
        }
    }

    // ================================
    // VOLTAR
    // ================================

    void Voltar()
    {
        Fechar();

        if (modoAtual == SlotMode.NovoJogo || modoAtual == SlotMode.Carregar)
        {
            if (MainMenu.Instance != null)
                MainMenu.Instance.MostrarPainelPrincipal();
        }
        else if (modoAtual == SlotMode.Salvar)
        {
            if (PauseMenu.Instance != null)
                PauseMenu.Instance.MostrarPainelPrincipal();
        }
    }
}
