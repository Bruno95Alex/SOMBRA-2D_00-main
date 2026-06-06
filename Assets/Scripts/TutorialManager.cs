using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Sistema de tutorial por popups — versão atualizada com:
///   • Step de Pulo adicionado
///   • Step de Poça de Sombra adicionado
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    // ── Cada etapa do tutorial ─────────────────────────
    public enum TutorialStep
    {
        Movimento    = 0,
        Lanterna     = 1,
        ColetarItem  = 2,
        Interagir    = 3,
        Pulo         = 4,
        PocaDeSombra = 5,
    }

    // ── Inspector ──────────────────────────────────────
    [Header("UI — Popup")]
    [SerializeField] private CanvasGroup     painelTutorial;
    [SerializeField] private Image           iconeTecla;
    [SerializeField] private TextMeshProUGUI tituloTexto;
    [SerializeField] private TextMeshProUGUI descTexto;
    [SerializeField] private TextMeshProUGUI textoFechar;

    [Header("Ícones das teclas (arraste na ordem do enum)")]
    [SerializeField] private Sprite[] iconesKeyboard;   // 6 sprites: Movimento, Lanterna, Coletar, Interagir, Pulo, PocaDeSombra
    [SerializeField] private Sprite[] iconesGamepad;    // mesma ordem

    [Header("Configuração")]
    [SerializeField] private float duracaoFade    = 0.35f;
    [SerializeField] private float tempoAutoFecha = 6f;

    // ── Privados ───────────────────────────────────────
    private bool      popupAberto = false;
    private Coroutine rotinaAtual;
    private TutorialStep stepAtual;
    private const string PREFS_KEY = "Tutorial_Visto_";

    // ── Conteúdo ───────────────────────────────────────
    private static readonly string[] Titulos = {
        "Movimento",
        "Lanterna",
        "Coletar Itens",
        "Interagir",
        "Pulo",
        "Poça de Sombra",
    };

    private static readonly string[] DescsTeclado = {
        "Use <b>WASD</b> ou as <b>Setas</b>\npara mover o personagem.",
        "Pressione <b>E</b> para ligar e desligar a lanterna.\nAponte com o <b>mouse</b>.",
        "Chegue perto do item\ne pressione <b>F</b> para coletar.",
        "Pressione <b>F</b> para interagir com objetos e personagens.",
        "Pressione <b>Espaço</b> para pular.\nUse o pulo para passar por cima de obstáculos.",
        "Uma mancha escura vai aparecer\nonde você está!\n<b>Saia de dentro dela</b> antes\nque fique preta — ou você morre.",
    };

    private static readonly string[] DescsControle = {
        "Use o <b>analógico esquerdo</b>\nou o <b>D-pad</b> para mover.",
        "Pressione <b>□ / X</b> para ligar e desligar a lanterna.\nAponte com o <b>analógico direito</b>.",
        "Chegue perto do item\ne pressione <b>△ / Y</b> para coletar.",
        "Pressione <b>△ / Y</b> para interagir com objetos e personagens.",
        "Pressione <b>✕ / A</b> para pular.\nUse o pulo para passar\npor cima de obstáculos.",
        "Uma mancha escura vai aparecer onde você está!\n<b>Saia de dentro dela</b> antes que fique preta — ou você morre.",
    };

    // ══════════════════════════════════════════════════
    void Awake()
    {
        Instance = this;

        if (painelTutorial != null)
        {
            painelTutorial.alpha          = 0f;
            painelTutorial.interactable   = false;
            painelTutorial.blocksRaycasts = false;
        }

        ValidarSetup();
    }

    void ValidarSetup()
    {
        if (painelTutorial == null) Debug.LogError("[TutorialManager] PainelTutorial não conectado!");
        if (tituloTexto    == null) Debug.LogError("[TutorialManager] TituloTexto não conectado!");
        if (descTexto      == null) Debug.LogError("[TutorialManager] DescTexto não conectado!");
        if (textoFechar    == null) Debug.LogWarning("[TutorialManager] TextoFechar não conectado.");
    }

    void Update()
    {
        if (!popupAberto) return;

        bool fechar = InputReader.Instance != null
            ? (InputReader.Instance.InteractPressed || InputReader.Instance.JumpPressed)
            : UnityEngine.InputSystem.Keyboard.current != null &&
              (UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame ||
               UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame);

        if (fechar)
            FecharPopup();
    }

    void Start()
{
    ResetarTutorial(); // remova depois dos testes
}

    // ══════════════════════════════════════════════════
    // API PÚBLICA
    // ══════════════════════════════════════════════════

    public void MostrarStep(TutorialStep step)
    {
        if (JaFoiVisto(step)) return;
        if (popupAberto)      return;

        stepAtual = step;
        MarcarComoVisto(step);

        if (rotinaAtual != null) StopCoroutine(rotinaAtual);
        rotinaAtual = StartCoroutine(RotinaMostrar(step));
    }

    public void FecharPopup()
    {
        if (!popupAberto) return;
        if (rotinaAtual != null) StopCoroutine(rotinaAtual);
        rotinaAtual = StartCoroutine(RotinaFechar());
    }

    public void ResetarTutorial()
    {
        foreach (TutorialStep s in System.Enum.GetValues(typeof(TutorialStep)))
            PlayerPrefs.DeleteKey(PREFS_KEY + (int)s);
        PlayerPrefs.Save();
        Debug.Log("[TutorialManager] Tutorial resetado.");
    }

    public bool JaViuTodos()
    {
        foreach (TutorialStep s in System.Enum.GetValues(typeof(TutorialStep)))
            if (!JaFoiVisto(s)) return false;
        return true;
    }

    // ══════════════════════════════════════════════════
    // ROTINAS
    // ══════════════════════════════════════════════════

    IEnumerator RotinaMostrar(TutorialStep step)
    {
        popupAberto    = true;
        Time.timeScale = 0f;

        PreencherConteudo(step);

        yield return StartCoroutine(FadePanel(0f, 1f));

        painelTutorial.interactable   = true;
        painelTutorial.blocksRaycasts = true;

        if (tempoAutoFecha > 0f)
            yield return new WaitForSecondsRealtime(tempoAutoFecha);
        else
            yield break;

        yield return StartCoroutine(RotinaFechar());
    }

    IEnumerator RotinaFechar()
    {
        painelTutorial.interactable   = false;
        painelTutorial.blocksRaycasts = false;

        yield return StartCoroutine(FadePanel(1f, 0f));

        popupAberto    = false;
        Time.timeScale = 1f;
    }

    IEnumerator FadePanel(float de, float para)
    {
        if (painelTutorial == null) yield break;
        float t = 0f;
        while (t < duracaoFade)
        {
            t += Time.unscaledDeltaTime;
            painelTutorial.alpha = Mathf.Lerp(de, para, t / duracaoFade);
            yield return null;
        }
        painelTutorial.alpha = para;
    }

    // ══════════════════════════════════════════════════
    // CONTEÚDO
    // ══════════════════════════════════════════════════

    void PreencherConteudo(TutorialStep step)
    {
        int idx = (int)step;
        bool usaControle = InputReader.Instance != null && InputReader.Instance.IsUsingController;

        if (tituloTexto != null) tituloTexto.text = Titulos[idx];
        if (descTexto   != null) descTexto.text   = usaControle ? DescsControle[idx] : DescsTeclado[idx];

        if (textoFechar != null)
            textoFechar.text = usaControle
                ? "Pressione △ / Y para fechar"
                : "Pressione F ou Espaço para fechar";

        if (iconeTecla != null)
        {
            Sprite[] icones = usaControle ? iconesGamepad : iconesKeyboard;
            if (icones != null && idx < icones.Length && icones[idx] != null)
            {
                iconeTecla.sprite  = icones[idx];
                iconeTecla.enabled = true;
            }
            else
            {
                iconeTecla.enabled = false;
            }
        }
    }

    // ══════════════════════════════════════════════════
    bool JaFoiVisto(TutorialStep step) =>
        PlayerPrefs.GetInt(PREFS_KEY + (int)step, 0) == 1;

    void MarcarComoVisto(TutorialStep step) =>
        PlayerPrefs.SetInt(PREFS_KEY + (int)step, 1);
}
