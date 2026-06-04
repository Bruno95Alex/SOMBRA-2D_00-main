using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Sistema de tutorial por popups — aparece uma dica de cada vez
/// conforme o jogador realiza as ações ou entra em zonas específicas.
///
/// ═══════════════════════════════════════════════════════
/// HIERARQUIA NA CENA (filho do Canvas principal do jogo):
///
///   TutorialPopup  (GameObject vazio — este script aqui)
///     └── PainelTutorial  (Image com fundo semi-transparente)
///           ├── IconeTecla   (Image — ícone da tecla/botão)
///           ├── TituloTexto  (TextMeshProUGUI — ex: "Movimento")
///           └── DescTexto    (TextMeshProUGUI — ex: "Use WASD...")
///
/// ═══════════════════════════════════════════════════════
/// SETUP:
///  1. Adicione este script num GameObject na cena Scene1.
///  2. Conecte os campos no Inspector.
///  3. Crie TutorialTriggerZone em pontos do mapa e configure
///     o ID da dica (ver enum TutorialStep abaixo).
///  4. O tutorial só aparece UMA VEZ por save — depois de visto
///     fica salvo no PlayerPrefs e não aparece mais.
///
/// PARA TESTAR: Menu → Clear Tutorial (chame ResetarTutorial())
/// ═══════════════════════════════════════════════════════
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
    }

    // ── Inspector ──────────────────────────────────────
    [Header("UI — Popup")]
    [SerializeField] private CanvasGroup painelTutorial;
    [SerializeField] private Image       iconeTecla;
    [SerializeField] private TextMeshProUGUI tituloTexto;
    [SerializeField] private TextMeshProUGUI descTexto;
    [SerializeField] private TextMeshProUGUI textoFechar;  // "Pressione F para fechar"

    [Header("Ícones das teclas (arraste na ordem do enum)")]
    [SerializeField] private Sprite[] iconesKeyboard;     // ícones para teclado
    [SerializeField] private Sprite[] iconesGamepad;      // ícones para controle

    [Header("Configuração")]
    [SerializeField] private float duracaoFade  = 0.35f;
    [SerializeField] private float tempoAutoFecha = 6f;   // fecha sozinho após X segundos
                                                           // (0 = não fecha sozinho)

    // ── Privados ───────────────────────────────────────
    private bool   popupAberto  = false;
    private bool   pausado      = false;
    private Coroutine rotinaAtual;
    private TutorialStep stepAtual;

    // chave base no PlayerPrefs — uma por step
    private const string PREFS_KEY = "Tutorial_Visto_";

    // ── Conteúdo de cada step ─────────────────────────
    private static readonly string[] Titulos = {
        "Movimentação",
        "Lanterna",
        "Coletar Itens",
        "Interagir",
    };

    // Texto para teclado
    private static readonly string[] DescsTeclado = {
        "Use <b>WASD</b> ou as <b>Setas</b>\npara mover o personagem.",
        "Pressione <b>E</b> para ligar\ne desligar a lanterna.\nAponte com o <b>mouse</b>.",
        "Chegue perto do item\ne pressione <b>F</b> para coletar.",
        "Pressione <b>F</b> para interagir\ncom objetos e personagens.",
    };

    // Texto para controle/joystick
    private static readonly string[] DescsControle = {
        "Use o <b>analógico esquerdo</b>\nou o <b>D-pad</b> para mover.",
        "Pressione <b>□ / X</b> para ligar\ne desligar a lanterna.\nAponte com o <b>analógico direito</b>.",
        "Chegue perto do item\ne pressione <b>△ / Y</b> para coletar.",
        "Pressione <b>△ / Y</b> para interagir\ncom objetos e personagens.",
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
    }

    void Start()
{
    ResetarTutorial(); // remova depois dos testes
}

    void Update()
    {
        if (!popupAberto) return;

        // Qualquer tecla de interação fecha o popup
        bool fechar = InputReader.Instance != null
            ? (InputReader.Instance.InteractPressed || InputReader.Instance.JumpPressed)
            : UnityEngine.InputSystem.Keyboard.current != null &&
              (UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame ||
               UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame);

        if (fechar)
            FecharPopup();
    }

    // ══════════════════════════════════════════════════
    // API PÚBLICA
    // ══════════════════════════════════════════════════

    /// <summary>
    /// Tenta mostrar um step do tutorial.
    /// Se já foi visto antes, ignora silenciosamente.
    /// </summary>
    public void MostrarStep(TutorialStep step)
    {
        // já foi visto? ignora
        if (JaFoiVisto(step)) return;

        // popup já aberto? aguarda (pode enfileirar se quiser)
        if (popupAberto) return;

        stepAtual = step;
        MarcarComoVisto(step);

        if (rotinaAtual != null) StopCoroutine(rotinaAtual);
        rotinaAtual = StartCoroutine(RotinaMostrar(step));
    }

    /// <summary>
    /// Fecha o popup imediatamente (com fade out).
    /// </summary>
    public void FecharPopup()
    {
        if (!popupAberto) return;
        if (rotinaAtual != null) StopCoroutine(rotinaAtual);
        rotinaAtual = StartCoroutine(RotinaFechar());
    }

    /// <summary>
    /// Reseta todos os steps — útil para testes.
    /// </summary>
    public void ResetarTutorial()
    {
        foreach (TutorialStep s in System.Enum.GetValues(typeof(TutorialStep)))
            PlayerPrefs.DeleteKey(PREFS_KEY + (int)s);
        PlayerPrefs.Save();
        Debug.Log("[TutorialManager] Tutorial resetado.");
    }

    // ══════════════════════════════════════════════════
    // ROTINAS INTERNAS
    // ══════════════════════════════════════════════════

    IEnumerator RotinaMostrar(TutorialStep step)
    {
        popupAberto = true;

        // pausa o jogo enquanto o popup está aberto
        Time.timeScale = 0f;
        pausado        = true;

        // preenche o conteúdo
        PreencherConteudo(step);

        // fade in
        yield return StartCoroutine(FadePanel(0f, 1f));

        // ativa interação após aparecer
        painelTutorial.interactable   = true;
        painelTutorial.blocksRaycasts = true;

        // fecha automaticamente após X segundos (usa unscaled pois o jogo está pausado)
        if (tempoAutoFecha > 0f)
            yield return new WaitForSecondsRealtime(tempoAutoFecha);
        else
            yield break; // espera o jogador fechar manualmente via Update()

        // se chegou aqui é porque o tempo esgotou
        yield return StartCoroutine(RotinaFechar());
    }

    IEnumerator RotinaFechar()
    {
        painelTutorial.interactable   = false;
        painelTutorial.blocksRaycasts = false;

        yield return StartCoroutine(FadePanel(1f, 0f));

        popupAberto    = false;
        Time.timeScale = 1f;
        pausado        = false;
    }

    IEnumerator FadePanel(float de, float para)
    {
        if (painelTutorial == null) yield break;

        // usa unscaled pois o jogo pode estar pausado
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

        if (tituloTexto != null)
            tituloTexto.text = Titulos[idx];

        if (descTexto != null)
            descTexto.text = usaControle ? DescsControle[idx] : DescsTeclado[idx];

        if (textoFechar != null)
            textoFechar.text = usaControle
                ? "Pressione △ / Y para fechar"
                : "Pressione F ou Espaço para fechar";

        // ícone da tecla
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
    // PLAYERPREFS
    // ══════════════════════════════════════════════════

    bool JaFoiVisto(TutorialStep step) =>
        PlayerPrefs.GetInt(PREFS_KEY + (int)step, 0) == 1;

    void MarcarComoVisto(TutorialStep step) =>
        PlayerPrefs.SetInt(PREFS_KEY + (int)step, 1);
}
