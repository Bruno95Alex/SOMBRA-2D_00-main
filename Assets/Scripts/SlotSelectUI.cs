using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlotSelectUI : MonoBehaviour
{
    public static SlotSelectUI Instance;

    public enum SlotMode { NovoJogo, Salvar, Carregar }

    [Header("Painel principal")]
    [SerializeField] private GameObject painel;
    [SerializeField] private TextMeshProUGUI titulo;

    [Header("Slots — 3 botões")]
    [SerializeField] private Button[]          botoesSlot;
    [SerializeField] private TextMeshProUGUI[] textosSlot;

    [Header("Botões de ação")]
    [SerializeField] private Button          btnConfirmar;
    [SerializeField] private TextMeshProUGUI btnConfirmarText;
    [SerializeField] private Button          btnVoltar;

    [Header("Painel de confirmação (substituir)")]
    [SerializeField] private GameObject      painelConfirmacao;
    [SerializeField] private TextMeshProUGUI textoConfirmacao;

    [Header("Cores")]
    [SerializeField] private Color corSelecionado = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private Color corVazio       = new Color(0.6f, 0.6f, 0.6f, 1f);
    [SerializeField] private Color corOcupado     = new Color(1f, 1f, 1f, 1f);

    private SlotMode modoAtual;
    private int      slotSelecionado = -1;
    private string   cenaJogo = "Scene1";

    void Awake()
    {
        Instance = this;
        if (painel            != null) painel.SetActive(false);
        if (painelConfirmacao != null) painelConfirmacao.SetActive(false);
    }

    // ================================
    // ABRIR
    // ================================

    public void Abrir(SlotMode modo, string cena = "Scene1")
    {
        modoAtual       = modo;
        cenaJogo        = cena;
        slotSelecionado = -1;

        if (painelConfirmacao != null) painelConfirmacao.SetActive(false);
        if (painel            != null) painel.SetActive(true);

        // mostra todos os slots e botões
        MostrarElementos(true);

        // título e texto do botão confirmar
        if (titulo != null)
        {
            titulo.text = modo switch
            {
                SlotMode.NovoJogo => "Escolha o slot",
                SlotMode.Salvar   => "Salvar no slot",
                SlotMode.Carregar => "Escolha o save",
                _                 => "Slots"
            };
        }

        if (btnConfirmarText != null)
        {
            btnConfirmarText.text = modo switch
            {
                SlotMode.NovoJogo => "Começar",
                SlotMode.Salvar   => "Salvar",
                SlotMode.Carregar => "Carregar",
                _                 => "Confirmar"
            };
        }

        if (btnConfirmar != null) btnConfirmar.interactable = false;

        AtualizarSlots();

        // no modo Carregar, desabilita slots vazios
        if (modo == SlotMode.Carregar)
        {
            for (int i = 0; i < botoesSlot.Length; i++)
            {
                bool temSave = SaveSystem.Instance != null && SaveSystem.Instance.TemSave(i);
                if (botoesSlot[i] != null)
                    botoesSlot[i].interactable = temSave;
            }
        }
        else
        {
            foreach (var btn in botoesSlot)
                if (btn != null) btn.interactable = true;
        }
    }

    public void Fechar()
    {
        if (painel            != null) painel.SetActive(false);
        if (painelConfirmacao != null) painelConfirmacao.SetActive(false);
        slotSelecionado = -1;
    }

    // ================================
    // MOSTRAR / ESCONDER ELEMENTOS
    // ================================

    void MostrarElementos(bool mostrar)
    {
        foreach (var btn in botoesSlot)
            if (btn != null) btn.gameObject.SetActive(mostrar);

        if (btnConfirmar != null) btnConfirmar.gameObject.SetActive(mostrar);
        if (btnVoltar    != null) btnVoltar.gameObject.SetActive(mostrar);
        if (titulo       != null) titulo.gameObject.SetActive(mostrar);
    }

    // ================================
    // ATUALIZAR VISUAL DOS SLOTS
    // ================================

    void AtualizarSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            bool temSave = SaveSystem.Instance != null && SaveSystem.Instance.TemSave(i);

            if (i < textosSlot.Length && textosSlot[i] != null)
                textosSlot[i].text = temSave
                    ? $"Save {i + 1:00}  —  {SaveSystem.Instance.DataDoSave(i)}"
                    : $"Save {i + 1:00}  —  Vazio";

            if (i < botoesSlot.Length && botoesSlot[i] != null)
            {
                var img = botoesSlot[i].GetComponent<Image>();
                if (img != null)
                    img.color = temSave ? corOcupado : corVazio;
            }
        }
    }

    // ================================
    // SELECIONAR SLOT
    // ================================

    public void SelecionarSlot(int index)
    {
        // garante que usa sempre a instância correta
        if (Instance != this)
        {
            Instance.SelecionarSlot(index);
            return;
        }

        slotSelecionado = index;
        Debug.Log($"[SlotSelectUI] Slot selecionado: {index} | modo={modoAtual}");

        for (int i = 0; i < botoesSlot.Length; i++)
        {
            if (botoesSlot[i] == null) continue;
            var img = botoesSlot[i].GetComponent<Image>();
            if (img == null) continue;
            bool temSave = SaveSystem.Instance != null && SaveSystem.Instance.TemSave(i);
            img.color = i == index ? corSelecionado : (temSave ? corOcupado : corVazio);
        }

        if (btnConfirmar != null) btnConfirmar.interactable = true;
    }

    public void SelecionarSlot0() => SelecionarSlot(0);
    public void SelecionarSlot1() => SelecionarSlot(1);
    public void SelecionarSlot2() => SelecionarSlot(2);

    // ================================
    // CONFIRMAR
    // ================================

    public void Confirmar()
    {
        Debug.Log($"[SlotSelectUI] Confirmar chamado — slotSelecionado={slotSelecionado} modo={modoAtual}");

        if (slotSelecionado < 0)
        {
            Debug.LogWarning("[SlotSelectUI] Nenhum slot selecionado!");
            return;
        }

        bool slotOcupado = SaveSystem.Instance != null &&
                           SaveSystem.Instance.TemSave(slotSelecionado);

        Debug.Log($"[SlotSelectUI] slotOcupado={slotOcupado}");

        // Carregar: vai direto sem confirmação
        // NovoJogo/Salvar: pede confirmação só se slot ocupado
        if (modoAtual == SlotMode.Carregar)
        {
            ExecutarAcao();
        }
        else if (slotOcupado)
        {
            MostrarConfirmacao();
        }
        else
        {
            ExecutarAcao();
        }
    }

    void MostrarConfirmacao()
    {
        MostrarElementos(false);

        if (painelConfirmacao != null) painelConfirmacao.SetActive(true);

        if (textoConfirmacao != null)
        {
            string data = SaveSystem.Instance.DataDoSave(slotSelecionado);
            textoConfirmacao.text =
                $"Save {slotSelecionado + 1:00} — {data}\nDeseja substituir este save?";
        }
    }

    void RestaurarSlots()
    {
        if (painelConfirmacao != null) painelConfirmacao.SetActive(false);
        MostrarElementos(true);
        AtualizarSlots();
        slotSelecionado = -1;
        if (btnConfirmar != null) btnConfirmar.interactable = false;
    }

    public void ConfirmarSim()
    {
        // guarda slot antes de restaurar (RestaurarSlots reseta para -1)
        int slotParaUsar = slotSelecionado;

        if (painelConfirmacao != null) painelConfirmacao.SetActive(false);
        MostrarElementos(true);
        AtualizarSlots();

        slotSelecionado = slotParaUsar; // mantém o slot escolhido
        ExecutarAcao();
    }

    public void ConfirmarNao()
    {
        // volta para a lista de slots sem fazer nada
        RestaurarSlots();
    }

    // ================================
    // EXECUTAR AÇÃO
    // ================================

    void ExecutarAcao()
    {
        // captura ANTES de qualquer Fechar() ou Reset que possa zerar slotSelecionado
        int slot = slotSelecionado;
        SlotMode modo = modoAtual;

        Debug.Log($"[SlotSelectUI] ExecutarAcao — slot={slot} modo={modo}");

        if (slot < 0)
        {
            Debug.LogError("[SlotSelectUI] slot inválido em ExecutarAcao!");
            return;
        }

        Fechar();
        Time.timeScale = 1f;

        if (modo == SlotMode.NovoJogo)
        {
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.DeletarSave(slot);
                SaveSystem.Instance.SlotAtivoPublico = slot;
            }
            SceneManager.LoadScene(cenaJogo);
        }
        else if (modo == SlotMode.Salvar)
        {
            if (SaveSystem.Instance != null)
                SaveSystem.Instance.Salvar(slot);

            var camCtrl = FindFirstObjectByType<CameraController>();
            if (camCtrl != null) camCtrl.StopAllCoroutines();

            SceneManager.LoadScene("Menu");
        }
        else if (modo == SlotMode.Carregar)
        {
            if (SaveSystem.Instance != null)
                SaveSystem.Instance.Carregar(slot);
            else
                Debug.LogError("[SlotSelectUI] SaveSystem não encontrado!");
        }
    }

    // ================================
    // VOLTAR
    // ================================

    public void Voltar()
    {
        Fechar();

        if (modoAtual == SlotMode.NovoJogo && MainMenu.Instance != null)
            MainMenu.Instance.MostrarPainelPrincipal();
        else if (modoAtual == SlotMode.Salvar && PauseMenu.Instance != null)
            PauseMenu.Instance.MostrarPainelPrincipal();
        else if (modoAtual == SlotMode.Carregar && MainMenu.Instance != null)
            MainMenu.Instance.MostrarPainelPrincipal();
    }
}
