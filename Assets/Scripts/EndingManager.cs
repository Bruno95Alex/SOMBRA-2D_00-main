using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Encerramento do jogo após ligar o gerador.
///
/// SETUP NA CENA:
///   1. GameObject "EndingManager" com este script + VideoPlayer + AudioSource
///
///   2. EndingCanvas (Canvas, Screen Space Overlay, Sort Order 99):
///        ├── PainelFade    → Image preta, preenche tela toda, CanvasGroup
///        └── VideoDisplay  → RawImage, preenche tela toda
///
///   3. No VideoPlayer:
///        Source        = Video Clip → seu .mp4
///        Render Mode   = Render Texture
///        Target Texture = crie uma RenderTexture 1920x1080
///                         (Assets → Create → Render Texture)
///                         arraste na RawImage (campo Texture) E no VideoPlayer
///        Play On Awake = FALSE
///        Loop          = FALSE
///        Audio Output  = Audio Source
///
///   4. No Generator, conecte o campo "Ending Manager"
/// </summary>
public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    [Header("Vídeo")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage    videoDisplay;  // RawImage com a RenderTexture

    [Header("UI")]
    [SerializeField] private CanvasGroup painelFade;

    [Header("Timing")]
    [SerializeField] private float delayAposLuzes = 2f;
    [SerializeField] private float duracaoFade    = 1.2f;

    [Header("Cena")]
    [SerializeField] private string cenaMenu = "Menu";

    // ══════════════════════════════════════════════
    void Awake()
    {
        Instance = this;

        // tudo começa invisível
        if (painelFade != null)
        {
            painelFade.alpha          = 0f;
            painelFade.blocksRaycasts = false;
        }

        // RawImage começa invisível até o vídeo estar pronto
        if (videoDisplay != null)
            videoDisplay.gameObject.SetActive(false);

        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
    }

    // ══════════════════════════════════════════════
    public void IniciarEncerramento()
    {
        StartCoroutine(RotinaEncerramento());
    }

    // ══════════════════════════════════════════════
    IEnumerator RotinaEncerramento()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.SetMovementLocked(true);

        if (ShadowPoolSpawner.Instance != null)
            ShadowPoolSpawner.Instance.Desativar();

        // ── 1. DELAY APÓS LUZES ──────────────────
        yield return new WaitForSeconds(delayAposLuzes);

        // ── 2. FADE PARA PRETO ───────────────────
        yield return StartCoroutine(Fade(0f, 1f, duracaoFade));

        // ── 3. PREPARA VÍDEO ─────────────────────
        if (videoPlayer == null)
        {
            Debug.LogError("[EndingManager] VideoPlayer não configurado!");
            SalvarEVoltar();
            yield break;
        }

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        // ativa a RawImage antes de começar
        if (videoDisplay != null)
            videoDisplay.gameObject.SetActive(true);

        videoPlayer.loopPointReached += AoTerminarVideo;
        videoPlayer.Play();

        // ── 4. FADE PARA MOSTRAR VÍDEO ───────────
        yield return StartCoroutine(Fade(1f, 0f, duracaoFade));
    }

    // ══════════════════════════════════════════════
    void AoTerminarVideo(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= AoTerminarVideo;
        StartCoroutine(EncerrarAposVideo());
    }

    IEnumerator EncerrarAposVideo()
    {
        // ── 5. FADE PARA PRETO FINAL ─────────────
        yield return StartCoroutine(Fade(0f, 1f, duracaoFade));

        videoPlayer.Stop();

        if (videoDisplay != null)
            videoDisplay.gameObject.SetActive(false);

        // ── 6. SALVAR E VOLTAR ───────────────────
        SalvarEVoltar();
    }

    void SalvarEVoltar()
    {
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.Salvar();
        else
            Debug.LogWarning("[EndingManager] SaveSystem não encontrado.");

        Time.timeScale = 1f;
        SceneManager.LoadScene(cenaMenu);
    }

    // ══════════════════════════════════════════════
    IEnumerator Fade(float de, float para, float duracao)
    {
        if (painelFade == null) yield break;

        painelFade.blocksRaycasts = true;
        float t = 0f;
        while (t < duracao)
        {
            t += Time.deltaTime;
            painelFade.alpha = Mathf.Lerp(de, para, t / duracao);
            yield return null;
        }
        painelFade.alpha = para;
    }
}
