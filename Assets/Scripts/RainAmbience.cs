using UnityEngine;
using System.Collections;

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

        // AudioSource separado do MusicManager — cada um tem o seu próprio componente
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop        = true;
        audioSource.volume      = 0f;
        audioSource.playOnAwake = false;

        // garante que o clip serializado é atribuído depois do AddComponent
        audioSource.clip = rainClip;
    }

    void Start()
    {
        if (rainClip == null)
        {
            Debug.LogWarning("RainAmbience: nenhum clipe de chuva atribuído.");
            return;
        }

        // re-atribui o clip aqui por segurança
        audioSource.clip = rainClip;
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

    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        audioSource.volume = volume;
    }
}
