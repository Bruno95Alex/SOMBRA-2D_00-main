using UnityEngine;
using System.Collections;

/// <summary>
/// Toca som de chuva em loop contínuo.
/// Separado do MusicManager para controle de volume independente.
/// Persiste entre cenas.
///
/// SETUP:
/// 1. Crie um GameObject vazio chamado "RainAmbience"
/// 2. Adicione este script
/// 3. Arraste o clipe de chuva no campo "Rain Clip"
/// </summary>
public class RainAmbience : MonoBehaviour
{
    public static RainAmbience Instance;

    [Header("Som")]
    [SerializeField] private AudioClip rainClip;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.35f;

    [Header("Fade")]
    [SerializeField] private float fadeInDuration = 3f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip   = rainClip;
        audioSource.loop   = true;
        audioSource.volume = 0f;
    }

    void Start()
    {
        if (rainClip == null)
        {
            Debug.LogWarning("RainAmbience: nenhum clipe de chuva atribuído.");
            return;
        }

        audioSource.Play();
        StartCoroutine(FadeIn());
    }

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

    /// <summary>Muda o volume em tempo real (ex: dentro de casa = mais baixo).</summary>
    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        audioSource.volume = volume;
    }
}
