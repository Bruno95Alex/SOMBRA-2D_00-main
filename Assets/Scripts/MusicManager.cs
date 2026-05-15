using UnityEngine;
using System.Collections;

/// <summary>
/// Toca música de fundo em loop com fade entre faixas.
/// Coloque num GameObject vazio chamado "MusicManager".
///
/// SETUP:
/// 1. Crie um GameObject vazio e adicione este script
/// 2. Arraste os clipes de música no campo "Tracks"
/// 3. Ajuste o volume e o tempo de fade no Inspector
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Músicas (tocam em sequência ou aleatório)")]
    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private bool randomOrder = false;

    [Header("Volume")]
    [SerializeField] [Range(0f, 1f)] private float volume = 0.4f;

    [Header("Fade")]
    [SerializeField] private float fadeInDuration  = 2f;
    [SerializeField] private float fadeOutDuration = 1.5f;

    private AudioSource audioSource;
    private int currentIndex = 0;
    private bool transitioning = false;

    void Awake()
    {
        // Singleton persistente entre cenas
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop   = false;
        audioSource.volume = 0f;
    }

    void Start()
    {
        if (tracks == null || tracks.Length == 0)
        {
            Debug.LogWarning("MusicManager: nenhuma faixa adicionada.");
            return;
        }

        PlayCurrentTrack();
    }

    void Update()
    {
        // quando a faixa termina, passa para a próxima
        if (!audioSource.isPlaying && !transitioning && tracks.Length > 0)
            StartCoroutine(NextTrack());
    }

    // =========================
    // TOCAR FAIXA ATUAL
    // =========================

    void PlayCurrentTrack()
    {
        if (tracks == null || tracks.Length == 0) return;

        audioSource.clip = tracks[currentIndex];
        audioSource.Play();

        StartCoroutine(FadeIn());
    }

    // =========================
    // PRÓXIMA FAIXA
    // =========================

    IEnumerator NextTrack()
    {
        transitioning = true;

        yield return StartCoroutine(FadeOut());

        if (randomOrder)
        {
            int next;
            do { next = Random.Range(0, tracks.Length); }
            while (next == currentIndex && tracks.Length > 1);
            currentIndex = next;
        }
        else
        {
            currentIndex = (currentIndex + 1) % tracks.Length;
        }

        PlayCurrentTrack();
        transitioning = false;
    }

    // =========================
    // FADE IN / OUT
    // =========================

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, t / fadeInDuration);
            yield return null;
        }
        audioSource.volume = volume;
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        float startVolume = audioSource.volume;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }

    // =========================
    // CONTROLES EXTERNOS
    // =========================

    /// <summary>Pausa a música com fade.</summary>
    public void Pause() => StartCoroutine(FadeOut());

    /// <summary>Retoma a música com fade.</summary>
    public void Resume()
    {
        audioSource.Play();
        StartCoroutine(FadeIn());
    }

    /// <summary>Muda o volume em tempo real.</summary>
    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        audioSource.volume = volume;
    }
}
