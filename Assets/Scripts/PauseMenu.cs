using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Menu de pausa com: Continuar, Voltar ao Menu, Opções, Sair.
/// Abre com Escape ou Start do controle.
///
/// SETUP:
/// 1. Crie um GameObject "PauseMenu" na cena do jogo
/// 2. Monte a UI conforme hierarquia abaixo
/// 3. Adicione este script e configure as referências
///
/// HIERARQUIA:
/// PauseMenu (este script)
///   └── PausePanel (Panel)
///       ├── Titulo (TextMeshPro — "PAUSADO")
///       ├── BtnContinuar (Button)
///       ├── BtnSalvarVoltar (Button)
///       ├── BtnOpcoes (Button)
///       └── BtnSair (Button)
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("Painel principal")]
    [SerializeField] private GameObject pausePanel;

    [Header("Botões")]
    [SerializeField] private Button btnContinuar;
    [SerializeField] private Button btnSalvarVoltar;
    [SerializeField] private Button btnOpcoes;
    [SerializeField] private Button btnSair;

    [Header("Painel de Opções")]
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private Slider     sliderMusica;
    [SerializeField] private Slider     sliderSons;

    [Header("Nome da cena do menu")]
    [SerializeField] private string cenaMenu = "Menu";

    [Header("Navegação por controle")]
    [SerializeField] private Color corSelecionado = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private Color corNormal      = Color.white;

    private bool pausado       = false;
    private bool opcoesAbertas = false;

    private Button[] botoes;
    private int      botaoAtual = 0;
    private float    inputDelay = 0f;
    private const float INPUT_DELAY = 0.2f;

    void Awake()
    {
        Instance = this;

        botoes = new Button[] { btnContinuar, btnSalvarVoltar, btnOpcoes, btnSair };

        if (pausePanel  != null) pausePanel.SetActive(false);
        if (painelOpcoes != null) painelOpcoes.SetActive(false);
    }

    void Start()
    {
        CarregarOpcoes();
    }

    // ================================
    // UPDATE — detecta Escape / Start
    // ================================

    void Update()
    {
        bool togglePausa = InputReader.Instance != null
            ? InputReader.Instance.MenuPressed
            : UnityEngine.InputSystem.Keyboard.current != null &&
              UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;

        if (togglePausa)
        {
            if (opcoesAbertas)
                FecharOpcoes();
            else if (pausado)
                BtnContinuar();
            else
                AbrirPausa();

            return;
        }

        if (!pausado) return;

        if (opcoesAbertas)
            return; // opções gerencia o próprio input

        NavegerMenu();
    }

    // ================================
    // ABRIR / FECHAR PAUSA
    // ================================

    public void AbrirPausa()
    {
        pausado = true;
        Time.timeScale = 0f;

        if (pausePanel != null) pausePanel.SetActive(true);

        botaoAtual = 0;
        SelecionarBotao(0);
    }

    public void FecharPausa()
    {
        pausado = false;
        Time.timeScale = 1f;

        if (pausePanel   != null) pausePanel.SetActive(false);
        if (painelOpcoes != null) painelOpcoes.SetActive(false);

        opcoesAbertas = false;
    }

    // ================================
    // NAVEGAÇÃO POR CONTROLE
    // ================================

    void NavegerMenu()
    {
        inputDelay -= Time.unscaledDeltaTime;
        if (inputDelay > 0f) return;

        var js = UnityEngine.InputSystem.Joystick.current;
        var kb = UnityEngine.InputSystem.Keyboard.current;
        var gp = UnityEngine.InputSystem.Gamepad.current;

        float v = 0f;

        if (js != null && js.stick.ReadValue().magnitude > 0.5f)
            v = js.stick.ReadValue().y;

        if (v == 0f && gp != null)
            v = gp.leftStick.ReadValue().y;

        if (v == 0f && kb != null)
        {
            if (kb.upArrowKey.isPressed   || kb.wKey.isPressed) v =  1f;
            if (kb.downArrowKey.isPressed || kb.sKey.isPressed) v = -1f;
        }

        if (v != 0f)
        {
            int novo = botaoAtual + (v > 0f ? -1 : 1);
            novo = Mathf.Clamp(novo, 0, botoes.Length - 1);
            SelecionarBotao(novo);
            inputDelay = INPUT_DELAY;
        }

        // confirmar com Triângulo / Enter / Espaço
        bool confirmar = (InputReader.Instance != null && InputReader.Instance.InteractPressed)
                      || (InputReader.Instance != null && InputReader.Instance.JumpPressed)
                      || (kb != null && (kb.enterKey.wasPressedThisFrame ||
                                        kb.spaceKey.wasPressedThisFrame));

        if (confirmar)
            botoes[botaoAtual].onClick.Invoke();
    }

    void SelecionarBotao(int index)
    {
        foreach (var btn in botoes)
            if (btn != null) btn.GetComponent<Image>().color = corNormal;

        botaoAtual = index;

        if (botoes[index] != null)
        {
            botoes[index].GetComponent<Image>().color = corSelecionado;
            botoes[index].Select();
        }
    }

    // ================================
    // BOTÕES
    // ================================

    public void BtnContinuar()
    {
        FecharPausa();
    }

    public void BtnSalvarVoltar()
    {
        // abre seleção de slot em vez de salvar direto
        MostrarPainelPrincipal(false); // esconde botões
        EsconderBotoes();

        if (SlotSelectUI.Instance != null)
            SlotSelectUI.Instance.Abrir(SlotSelectUI.SlotMode.Salvar);
    }

    void EsconderBotoes()
    {
        if (btnContinuar    != null) btnContinuar.gameObject.SetActive(false);
        if (btnSalvarVoltar != null) btnSalvarVoltar.gameObject.SetActive(false);
        if (btnOpcoes       != null) btnOpcoes.gameObject.SetActive(false);
        if (btnSair         != null) btnSair.gameObject.SetActive(false);
    }

    public void MostrarPainelPrincipal(bool mostrar = true)
    {
        if (btnContinuar    != null) btnContinuar.gameObject.SetActive(mostrar);
        if (btnSalvarVoltar != null) btnSalvarVoltar.gameObject.SetActive(mostrar);
        if (btnOpcoes       != null) btnOpcoes.gameObject.SetActive(mostrar);
        if (btnSair         != null) btnSair.gameObject.SetActive(mostrar);

        if (mostrar) SelecionarBotao(0);
    }

    public void BtnOpcoes()
    {
        opcoesAbertas = true;

        // esconde botões principais
        if (btnContinuar    != null) btnContinuar.gameObject.SetActive(false);
        if (btnSalvarVoltar != null) btnSalvarVoltar.gameObject.SetActive(false);
        if (btnOpcoes       != null) btnOpcoes.gameObject.SetActive(false);
        if (btnSair         != null) btnSair.gameObject.SetActive(false);

        if (painelOpcoes != null) painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        opcoesAbertas = false;

        if (painelOpcoes != null) painelOpcoes.SetActive(false);

        // restaura botões principais
        if (btnContinuar    != null) btnContinuar.gameObject.SetActive(true);
        if (btnSalvarVoltar != null) btnSalvarVoltar.gameObject.SetActive(true);
        if (btnOpcoes       != null) btnOpcoes.gameObject.SetActive(true);
        if (btnSair         != null) btnSair.gameObject.SetActive(true);

        SalvarOpcoes();

        // reseleciona o botão de opções
        SelecionarBotao(2);
    }

    public void BtnSair()
    {
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.Salvar();

        Debug.Log("Saindo...");
        Application.Quit();
    }

    // ================================
    // OPÇÕES — volume
    // ================================

    public void OnMusicaChanged(float valor)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetVolume(valor);
    }

    public void OnSonsChanged(float valor)
    {
        if (RainAmbience.Instance != null)
            RainAmbience.Instance.SetVolume(valor);
    }

    void SalvarOpcoes()
    {
        if (sliderMusica != null) PlayerPrefs.SetFloat("VolMusica", sliderMusica.value);
        if (sliderSons   != null) PlayerPrefs.SetFloat("VolSons",   sliderSons.value);
        PlayerPrefs.Save();
    }

    void CarregarOpcoes()
    {
        if (sliderMusica != null)
        {
            sliderMusica.value = PlayerPrefs.GetFloat("VolMusica", 0.4f);
            OnMusicaChanged(sliderMusica.value);
        }

        if (sliderSons != null)
        {
            sliderSons.value = PlayerPrefs.GetFloat("VolSons", 0.35f);
            OnSonsChanged(sliderSons.value);
        }
    }

    public bool EstaPausado() => pausado;
}
