using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Reproduz o vídeo da cutscene de abertura e carrega a cena do jogo ao terminar.
/// O jogador pode pular pressionando qualquer tecla ou clicando na tela.
///
/// SETUP NA CENA "Cutscene":
///   1. Crie uma cena chamada "Cutscene" e adicione-a no Build Settings
///      ANTES da Scene1.
///   2. Hierarquia sugerida:
///      Canvas (Screen Space - Overlay, Sort Order 10)
///        ├── ImagemFundo       — Image preta (color #000000, preenche tela toda)
///        ├── RawImageVideo     — RawImage que exibe o vídeo (preenche tela toda)
///        ├── PainelFade        — Image preta com CanvasGroup (para fade in/out)
///        └── TextoPular        — TextMeshProUGUI: "Pressione qualquer tecla para pular"
///
///   3. No GameObject principal da cena adicione:
///      - Este script (CutsceneManager)
///      - VideoPlayer
///
///   4. Configure o VideoPlayer:
///      - Source       = Video Clip  →  arraste seu .mp4
///      - Render Mode  = Render Texture
///      - Target Texture = crie uma RenderTexture (Assets → Create → Render Texture)
///        resolução 1920x1080, e arraste na RawImageVideo também
///      - Play On Awake = FALSE (este script controla)
///      - Loop          = FALSE
///      - Audio Output Mode = Audio Source (adicione AudioSource no mesmo GameObject)
///
///   5. No MainMenu.cs, troque:
///      [SerializeField] private string cenaJogo = "Scene1";
///      por:
///      [SerializeField] private string cenaJogo = "Cutscene";
/// </summary>
public class CutsceneManager : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private VideoPlayer  videoPlayer;
    [SerializeField] private CanvasGroup  painelFade;   // Image preta com CanvasGroup
    [SerializeField] private CanvasGroup  textoPular;   // texto "pressione qualquer tecla"

    [Header("Configuração")]
    [SerializeField] private string cenaJogo     = "Scene1";
    [SerializeField] private float  duracaoFade  = 1f;    // segundos do fade in/out
    [SerializeField] private float  delayPular   = 2f;    // segundos antes de permitir pular

    // controle interno
    private bool   podePular    = false;
    private bool   pulando      = false;

    // =====================================================
    void Start()
    {
        // garante tela preta no início
        if (painelFade  != null) { painelFade.alpha  = 1f; painelFade.blocksRaycasts = false; }
        if (textoPular  != null)   textoPular.alpha   = 0f;

        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                Debug.LogError("[CutsceneManager] VideoPlayer não encontrado!");
                IrParaJogo();
                return;
            }
        }

        // registra callback de fim do vídeo
        videoPlayer.loopPointReached += AoTerminarVideo;

        StartCoroutine(IniciarCutscene());
    }

    // // =====================================================
    // void Update()
    // {
    //     if (!podePular || pulando) return;

    //     if (Input.anyKeyDown)
    //         StartCoroutine(PularCutscene());
    // }

    void Update()
{
    if (!podePular || pulando)
        return;

    var kb = UnityEngine.InputSystem.Keyboard.current;
    var mouse = UnityEngine.InputSystem.Mouse.current;
    var gp = UnityEngine.InputSystem.Gamepad.current;

    bool pressionou =
        (kb != null && kb.anyKey.wasPressedThisFrame)
        || (mouse != null && mouse.leftButton.wasPressedThisFrame)
        || (gp != null && gp.buttonSouth.wasPressedThisFrame);

    if (pressionou)
        StartCoroutine(PularCutscene());
}


    // =====================================================
    IEnumerator IniciarCutscene()
    {
        // prepara e começa o vídeo
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.Play();

        // fade in — tela preta some
        yield return StartCoroutine(FadePainel(1f, 0f, duracaoFade));

        // mostra o texto de pular com fade
        // if (textoPular != null)
        //     yield return StartCoroutine(FadeCanvasGroup(textoPular, 0f, 1f, 0.5f));
        if (textoPular != null)
        {
            yield return StartCoroutine(
                FadeCanvasGroup(textoPular, 0f, 1f, 0.5f)
            );

            StartCoroutine(PiscarTextoPular());
        }

        // aguarda o delay mínimo antes de permitir pular
        yield return new WaitForSeconds(delayPular);
        podePular = true;
    }

    // =====================================================
    void AoTerminarVideo(VideoPlayer vp)
    {
        if (pulando) return;
        StartCoroutine(EncerrarCutscene());
    }

    IEnumerator PularCutscene()
    {
        pulando   = true;
        podePular = false;
        yield return StartCoroutine(EncerrarCutscene());
    }

    IEnumerator EncerrarCutscene()
    {
        // esconde texto de pular
        if (textoPular != null)
            yield return StartCoroutine(FadeCanvasGroup(textoPular, textoPular.alpha, 0f, 0.3f));

        // fade out — tela escurece
        yield return StartCoroutine(FadePainel(painelFade.alpha, 1f, duracaoFade));

        videoPlayer.Stop();
        IrParaJogo();
    }

    void IrParaJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(cenaJogo);
    }

    // =====================================================
    // UTILITÁRIOS DE FADE
    // =====================================================

    IEnumerator FadePainel(float de, float para, float duracao)
    {
        if (painelFade == null) yield break;

        float t = 0f;
        while (t < duracao)
        {
            t += Time.deltaTime;
            painelFade.alpha = Mathf.Lerp(de, para, t / duracao);
            yield return null;
        }
        painelFade.alpha = para;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float de, float para, float duracao)
    {
        if (cg == null) yield break;

        float t = 0f;
        while (t < duracao)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(de, para, t / duracao);
            yield return null;
        }
        cg.alpha = para;
    }


IEnumerator PiscarTextoPular()
{
    while (true)
    {
        // fade out suave
        yield return StartCoroutine(
            FadeCanvasGroup(
                textoPular,
                1f,
                0.55f,
                0.45f
            )
        );

        // fade in suave
        yield return StartCoroutine(
            FadeCanvasGroup(
                textoPular,
                0.55f,
                1f,
                0.45f
            )
        );
    }
}


}
