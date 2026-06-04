using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Menu principal — versão atualizada com:
///   • Cutscene antes da Scene1  (cenaJogo agora aponta para "Cutscene")
///   • Confirmação de saída usando o mesmo ConfirmDialog do projeto
/// </summary>
public class MainMenu : MonoBehaviour
{
    // ================================
    // INSPECTOR
    // ================================

    [Header("Background (paralax)")]
    [SerializeField] private RectTransform background;
    [SerializeField] private float bgMoveAmount = 15f;
    [SerializeField] private float bgMoveSpeed  = 0.8f;

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

    [Header("Cenas")]
    // ► Aponte para "Cutscene" se quiser a cutscene antes do jogo,
    //   ou para "Scene1" para ir direto.
    [SerializeField] private string cenaJogo = "Cutscene";

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
        if (background != null) bgPosOriginal    = background.anchoredPosition;
        if (titleRect  != null) titlePosOriginal = titleRect.anchoredPosition;

        titlePhase = Random.Range(0f, Mathf.PI * 2f);

        if (titleCanvas != null)
            StartCoroutine(FadeInTitulo());

        SelecionarBotao(0);
        CarregarOpcoes();
        MostrarPainelPrincipal();
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
    // ANIMAÇÕES
    // ================================

    void AnimarBackground()
    {
        if (background == null) return;
        float t = Time.time * bgMoveSpeed;
        background.anchoredPosition = bgPosOriginal + new Vector2(
            Mathf.Sin(t * 0.7f) * bgMoveAmount,
            Mathf.Sin(t * 0.5f) * (bgMoveAmount * 0.6f));
    }

    void AnimarTitulo()
    {
        if (titleRect == null) return;
        float y = Mathf.Sin(Time.time * titleFloatSpeed + titlePhase) * titleFloatAmount;
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

        var kb = UnityEngine.InputSystem.Keyboard.current;
        var gp = UnityEngine.InputSystem.Gamepad.current;

        if (gp != null) v = gp.leftStick.ReadValue().y;

        if (v == 0f && kb != null)
        {
            if (kb.upArrowKey.isPressed   || kb.wKey.isPressed) v =  1f;
            if (kb.downArrowKey.isPressed || kb.sKey.isPressed) v = -1f;
        }

        if (v != 0f)
        {
            int novo = botoeSelecionado + (v > 0f ? -1 : 1);
            novo = Mathf.Clamp(novo, 0, botoes.Length - 1);
            while (novo >= 0 && novo < botoes.Length && !botoes[novo].interactable)
                novo += v > 0f ? -1 : 1;
            if (novo >= 0 && novo < botoes.Length)
                SelecionarBotao(novo);
            inputDelay = INPUT_DELAY;
        }

        bool confirmar = kb != null && (kb.enterKey.wasPressedThisFrame ||
                                        kb.fKey.wasPressedThisFrame     ||
                                        kb.spaceKey.wasPressedThisFrame);
        if (confirmar)
            botoes[botoeSelecionado].onClick.Invoke();
    }

    void SelecionarBotao(int index)
    {
        foreach (var btn in botoes)
            if (btn != null) btn.GetComponent<Image>().color = corNormal;
        botoeSelecionado = index;
        if (botoes[index] != null)
        {
            botoes[index].GetComponent<Image>().color = corSelecionado;
            botoes[index].Select();
        }
    }

    void NavegerOpcoes()
    {
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame) FecharOpcoes();
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
                Debug.LogError("[MainMenu] ConfirmDialog não encontrado!");
                return;
            }

            EsconderBotoes();

            ConfirmDialog.Instance.Mostrar(
                "Iniciar novo jogo?\nO progresso atual será perdido.",
                aoConfirmar: () => AbrirSelecaoSlot(),
                aoCancelar:  () => MostrarPainelPrincipal()
            );
        }
        else
        {
            AbrirSelecaoSlot();
        }
    }

    void AbrirSelecaoSlot()
    {
        EsconderBotoes();

        if (SlotSelectUI.Instance == null)
        {
            Debug.LogError("[MainMenu] SlotSelectUI não encontrado!");
            MostrarPainelPrincipal();
            return;
        }

        SlotSelectUI.Instance.Abrir(SlotSelectUI.SlotMode.NovoJogo, cenaJogo);
    }

    public void BtnContinuar()
    {
        if (SlotSelectUI.Instance == null)
        {
            Debug.LogError("[MainMenu] SlotSelectUI não encontrado!");
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

    /// <summary>
    /// Exibe confirmação antes de fechar o jogo.
    /// Usa o mesmo ConfirmDialog do projeto para manter consistência visual.
    /// </summary>
    public void BtnSair()
    {
        if (ConfirmDialog.Instance == null)
        {
            // fallback: sai direto se o dialog não estiver na cena
            Debug.LogWarning("[MainMenu] ConfirmDialog não encontrado — saindo sem confirmação.");
            Application.Quit();
            return;
        }

        // FIX: esconde os botões antes de mostrar o dialog
        EsconderBotoes();

        ConfirmDialog.Instance.Mostrar(
            "Deseja sair do jogo?",
            aoConfirmar: () =>
            {
                Debug.Log("[MainMenu] Usuário confirmou saída.");
                Application.Quit();

                // no Editor o Quit não fecha — para testar use:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            },
            aoCancelar: () => MostrarPainelPrincipal()  // restaura botões se cancelar
        );
    }

    // ================================
    // MOSTRAR / ESCONDER BOTÕES
    // ================================

    void EsconderBotoes()
    {
        if (btnIniciar   != null) btnIniciar.gameObject.SetActive(false);
        if (btnContinuar != null) btnContinuar.gameObject.SetActive(false);
        if (btnOpcoes    != null) btnOpcoes.gameObject.SetActive(false);
        if (btnSair      != null) btnSair.gameObject.SetActive(false);
        if (painelOpcoes != null) painelOpcoes.SetActive(false);
    }

    public void MostrarPainelPrincipal()
    {
        if (btnIniciar   != null) btnIniciar.gameObject.SetActive(true);
        if (btnContinuar != null) btnContinuar.gameObject.SetActive(
            SaveSystem.Instance != null && SaveSystem.Instance.TemQualquerSave());
        if (btnOpcoes    != null) btnOpcoes.gameObject.SetActive(true);
        if (btnSair      != null) btnSair.gameObject.SetActive(true);

        if (SlotSelectUI.Instance != null)
            SlotSelectUI.Instance.Fechar();
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
}
