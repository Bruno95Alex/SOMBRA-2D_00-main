using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Menu principal com:
/// - Background paralax (movimento suave)
/// - Título animado (fade + flutuação)
/// - Botões: Iniciar, Continuar, Opções, Sair
/// - Navegação por teclado e controle
///
/// SETUP:
/// Veja o passo a passo abaixo do script.
/// </summary>
public class MainMenu : MonoBehaviour
{
    // ================================
    // INSPECTOR
    // ================================

    [Header("Background (paralax)")]
    [SerializeField] private RectTransform background;
    [SerializeField] private float bgMoveAmount = 15f;   // quanto o bg se move
    [SerializeField] private float bgMoveSpeed  = 0.8f;  // velocidade do movimento

    [Header("Título")]
    [SerializeField] private RectTransform titleRect;
    [SerializeField] private CanvasGroup   titleCanvas;
    [SerializeField] private float titleFloatAmount = 8f;
    [SerializeField] private float titleFloatSpeed  = 1.2f;

    [Header("Botões")]
    [SerializeField] private Button btnIniciar;
    [SerializeField] private Button btnContinuar;
    [SerializeField] private Button btnOpcoes;
    [SerializeField] private Button btnSair;

    [Header("Painel de Opções")]
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private Slider     sliderMusica;
    [SerializeField] private Slider     sliderSons;
    [SerializeField] private Button     btnFecharOpcoes;

    [Header("Cenas")]
    [SerializeField] private string cenaJogo = "Scene1"; // nome da primeira cena do jogo

    [Header("Navegação por controle")]
    [SerializeField] private Color corSelecionado = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private Color corNormal      = Color.white;

    // ================================
    // PRIVADOS
    // ================================

    private Button[] botoes;
    private int      botoeSelecionado = 0;
    private bool     opcoesAbertas    = false;
    private float    inputDelay       = 0f;
    private const float INPUT_DELAY   = 0.2f;

    private Vector2 bgPosOriginal;
    private Vector2 titlePosOriginal;
    private float   titlePhase;

    // ================================
    // AWAKE / START
    // ================================

    public static MainMenu Instance;

    void Awake()
    {
        Instance = this;
        botoes = new Button[] { btnIniciar, btnContinuar, btnOpcoes, btnSair };

        // habilita Continuar só se tiver save
        bool temSave = SaveSystem.Instance != null
            ? SaveSystem.Instance.TemQualquerSave()
            : PlayerPrefs.HasKey("SaveExists_0");

        if (btnContinuar != null)
            btnContinuar.interactable = temSave;

        if (painelOpcoes != null)
            painelOpcoes.SetActive(false);
    }

    void Start()
    {
        if (background  != null) bgPosOriginal    = background.anchoredPosition;
        if (titleRect   != null) titlePosOriginal = titleRect.anchoredPosition;

        titlePhase = Random.Range(0f, Mathf.PI * 2f);

        // fade in do título
        if (titleCanvas != null)
            StartCoroutine(FadeInTitulo());

        // seleciona primeiro botão
        SelecionarBotao(0);

        // carrega volumes salvos
        CarregarOpcoes();
    }

    // ================================
    // UPDATE
    // ================================

    void Update()
    {
        AnimarBackground();
        AnimarTitulo();

        if (opcoesAbertas)
            NavegerOpcoes();
        else
            NavegerMenu();
    }

    // ================================
    // ANIMAÇÃO BACKGROUND
    // ================================

    void AnimarBackground()
    {
        if (background == null) return;

        float t = Time.time * bgMoveSpeed;
        float x = Mathf.Sin(t * 0.7f) * bgMoveAmount;
        float y = Mathf.Sin(t * 0.5f) * (bgMoveAmount * 0.6f);

        background.anchoredPosition = bgPosOriginal + new Vector2(x, y);
    }

    // ================================
    // ANIMAÇÃO TÍTULO
    // ================================

    void AnimarTitulo()
    {
        if (titleRect == null) return;

        float t = Time.time * titleFloatSpeed + titlePhase;
        float y = Mathf.Sin(t) * titleFloatAmount;

        titleRect.anchoredPosition = titlePosOriginal + new Vector2(0f, y);
    }

    IEnumerator FadeInTitulo()
    {
        titleCanvas.alpha = 0f;
        yield return new WaitForSeconds(0.3f);

        float t = 0f;
        while (t < 1.5f)
        {
            t += Time.deltaTime;
            titleCanvas.alpha = Mathf.Clamp01(t / 1.5f);
            yield return null;
        }

        titleCanvas.alpha = 1f;
    }

    // ================================
    // NAVEGAÇÃO MENU
    // ================================

    void NavegerMenu()
    {
        inputDelay -= Time.deltaTime;
        if (inputDelay > 0f) return;

        float v = 0f;

        var js = UnityEngine.InputSystem.Joystick.current;
        var kb = UnityEngine.InputSystem.Keyboard.current;
        var gp = UnityEngine.InputSystem.Gamepad.current;

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
            int novo = botoeSelecionado + (v > 0f ? -1 : 1);
            novo = Mathf.Clamp(novo, 0, botoes.Length - 1);

            // pula botões não interativos (ex: Continuar sem save)
            while (novo >= 0 && novo < botoes.Length && !botoes[novo].interactable)
                novo += v > 0f ? -1 : 1;

            if (novo >= 0 && novo < botoes.Length)
                SelecionarBotao(novo);

            inputDelay = INPUT_DELAY;
        }

        // confirmar com Enter, F, Espaço ou botões do controle
        bool confirmar = (kb != null && (kb.enterKey.wasPressedThisFrame ||
                                         kb.fKey.wasPressedThisFrame     ||
                                         kb.spaceKey.wasPressedThisFrame))
                      || (InputReader.Instance != null && InputReader.Instance.InteractPressed)
                      || (InputReader.Instance != null && InputReader.Instance.JumpPressed);

        if (confirmar)
            botoes[botoeSelecionado].onClick.Invoke();
    }

    void SelecionarBotao(int index)
    {
        // restaura cor de todos
        foreach (var btn in botoes)
            if (btn != null)
                btn.GetComponent<Image>().color = corNormal;

        botoeSelecionado = index;

        // destaca selecionado
        if (botoes[index] != null)
        {
            botoes[index].GetComponent<Image>().color = corSelecionado;
            botoes[index].Select(); // foco do Unity UI
        }
    }

    // ================================
    // NAVEGAÇÃO OPÇÕES
    // ================================

    void NavegerOpcoes()
    {
        var kb = UnityEngine.InputSystem.Keyboard.current;

        bool fechar = (kb != null && kb.escapeKey.wasPressedThisFrame)
                   || (InputReader.Instance != null && InputReader.Instance.InteractPressed);

        if (fechar) FecharOpcoes();
    }

    // ================================
    // BOTÕES DO MENU
    // ================================

    public void BtnIniciar()
    {
        bool temSave = SaveSystem.Instance != null && SaveSystem.Instance.TemQualquerSave();

        if (temSave)
        {
            if (ConfirmDialog.Instance == null)
            {
                Debug.LogError("ConfirmDialog não encontrado na cena!");
                return;
            }

            ConfirmDialog.Instance.Mostrar(
                "Iniciar novo jogo?",
                aoConfirmar: () => AbrirSelecaoSlot(),
                aoCancelar:  () => { }
            );
        }
        else
        {
            AbrirSelecaoSlot();
        }
    }

    void AbrirSelecaoSlot()
    {
        // esconde todos os botões do menu
        EsconderBotoes();

        if (SlotSelectUI.Instance == null)
        {
            Debug.LogError("[MainMenu] SlotSelectUI não encontrado!");
            MostrarPainelPrincipal(); // restaura se der erro
            return;
        }

        SlotSelectUI.Instance.Abrir(SlotSelectUI.SlotMode.NovoJogo, cenaJogo);
    }

    void EsconderBotoes()
    {
        if (btnIniciar   != null) btnIniciar.gameObject.SetActive(false);
        if (btnContinuar != null) btnContinuar.gameObject.SetActive(false);
        if (btnOpcoes    != null) btnOpcoes.gameObject.SetActive(false);
        if (btnSair      != null) btnSair.gameObject.SetActive(false);

        // esconde também o painel de opções se estiver aberto
        if (painelOpcoes != null) painelOpcoes.SetActive(false);
    }

    public void MostrarPainelPrincipal()
    {
        if (btnIniciar   != null) btnIniciar.gameObject.SetActive(true);
        if (btnContinuar != null) btnContinuar.gameObject.SetActive(
            SaveSystem.Instance != null && SaveSystem.Instance.TemQualquerSave());
        if (btnOpcoes    != null) btnOpcoes.gameObject.SetActive(true);
        if (btnSair      != null) btnSair.gameObject.SetActive(true);
    }

    public void BtnContinuar()
    {
        // abre seleção de slots no modo Carregar
        if (SlotSelectUI.Instance == null)
        {
            Debug.LogError("SlotSelectUI não encontrado!");
            return;
        }

        EsconderBotoes();
        SlotSelectUI.Instance.Abrir(SlotSelectUI.SlotMode.Carregar, cenaJogo);
    }

    public void BtnOpcoes()
    {
        opcoesAbertas = true;
        if (painelOpcoes != null) painelOpcoes.SetActive(true);
    }

    public void BtnSair()
    {
        Debug.Log("Saindo...");
        Application.Quit();
    }

    // ================================
    // OPÇÕES
    // ================================

    public void FecharOpcoes()
    {
        opcoesAbertas = false;
        if (painelOpcoes != null) painelOpcoes.SetActive(false);
        SalvarOpcoes();
    }

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

    // ================================
    // CARREGAR CENA COM FADE
    // ================================

    IEnumerator CarregarCena(string nomeCena)
    {
        // fade out se tiver UIFade
        if (UIFade.Instance != null)
            yield return StartCoroutine(UIFade.Instance.FadeOut());
        else
            yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene(nomeCena);
    }
}
