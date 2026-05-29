using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Collections;

/// <summary>
/// Sistema de sustos combinado:
/// 1. Som alto repentino (jumpscare sound)
/// 2. Flash branco/vermelho na tela
/// 3. Imagem assustadora por 1 segundo
/// 4. Sombra aparece perto do player
///
/// SETUP:
/// 1. Crie um GameObject "ScareManager" na cena
/// 2. Configure as referências no Inspector
/// 3. Crie triggers de susto com o script ScareTrigger
/// </summary>
public class ScareManager : MonoBehaviour
{
    public static ScareManager Instance;

    [Header("Flash na tela")]
    [SerializeField] private Image     flashImage;       // Image no Canvas, cobre tela toda
    [SerializeField] private Color     flashColor        = new Color(1f, 1f, 1f, 0.9f);
    [SerializeField] private float     flashDuration     = 0.08f;
    [SerializeField] private float     flashFadeSpeed    = 4f;

    [Header("Imagem assustadora")]
    [SerializeField] private Image     scareImage;       // Image no Canvas com sprite de susto
    [SerializeField] private Sprite[]  scareSprites;     // sprites assustadores variados
    [SerializeField] private float     scareImageDuration = 0.8f;

    [Header("Sons")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] scareSounds;    // sons de susto variados
    [SerializeField] [Range(0f, 1f)] private float scareVolume = 0.9f;

    [Header("Sombra perto do player")]
    [SerializeField] private GameObject shadowPrefab;    // prefab da sombra que aparece
    [SerializeField] private float      shadowDuration   = 1.5f;
    [SerializeField] private float      shadowOffset     = 2f; // distância do player

    [Header("Câmera shake")]
    [SerializeField] private float shakeDuration  = 0.3f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    [Header("Cooldown entre sustos")]
    [SerializeField] private float cooldownMinimo = 15f;

    private float  ultimoSusto    = -999f;
    private Camera cam;

    void Awake()
    {
        Instance = this;
        cam      = Camera.main;

        if (flashImage  != null) flashImage.color  = Color.clear;
        if (scareImage  != null) scareImage.color  = Color.clear;
    }

    // ================================
    // SUSTO COMPLETO
    // ================================

    public void TriggerScare(ScareType tipo = ScareType.Full)
    {
        if (Time.time - ultimoSusto < cooldownMinimo) return;

        ultimoSusto = Time.time;
        StartCoroutine(ScareRoutine(tipo));
    }

    IEnumerator ScareRoutine(ScareType tipo)
    {
        // executa em paralelo
        if (tipo == ScareType.Full || tipo == ScareType.SoundAndFlash)
        {
            StartCoroutine(PlayFlash());
            PlayScareSound();
        }

        if (tipo == ScareType.Full || tipo == ScareType.ImageAndShadow)
        {
            StartCoroutine(ShowScareImage());
            SpawnShadowNearPlayer();
        }

        if (tipo == ScareType.Full)
            StartCoroutine(ShakeCamera());

        yield return null;
    }

    // ================================
    // FLASH
    // ================================

    IEnumerator PlayFlash()
    {
        if (flashImage == null) yield break;

        flashImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * flashFadeSpeed;
            flashImage.color = Color.Lerp(flashColor, Color.clear, t);
            yield return null;
        }

        flashImage.color = Color.clear;
    }

    // ================================
    // SOM
    // ================================

    void PlayScareSound()
    {
        if (audioSource == null || scareSounds == null || scareSounds.Length == 0) return;

        AudioClip clip = scareSounds[Random.Range(0, scareSounds.Length)];
        audioSource.volume = scareVolume;
        audioSource.PlayOneShot(clip);
    }

    // ================================
    // IMAGEM ASSUSTADORA
    // ================================

    IEnumerator ShowScareImage()
    {
        if (scareImage == null || scareSprites == null || scareSprites.Length == 0) yield break;

        // escolhe sprite aleatório
        scareImage.sprite = scareSprites[Random.Range(0, scareSprites.Length)];
        scareImage.color  = Color.white;

        yield return new WaitForSeconds(scareImageDuration);

        // some rapidamente
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            scareImage.color = Color.Lerp(Color.white, Color.clear, t);
            yield return null;
        }

        scareImage.color = Color.clear;
    }

    // ================================
    // SOMBRA PERTO DO PLAYER
    // ================================

    void SpawnShadowNearPlayer()
    {
        if (shadowPrefab == null || PlayerController.Instance == null) return;

        // spawna atrás do player
        Vector3 playerPos  = PlayerController.Instance.transform.position;
        Vector3 offset     = new Vector3(
            Random.Range(-shadowOffset, shadowOffset),
            Random.Range(-shadowOffset, shadowOffset), 0f);

        GameObject shadow = Instantiate(shadowPrefab, playerPos + offset, Quaternion.identity);
        Destroy(shadow, shadowDuration);
    }

    // ================================
    // CÂMERA SHAKE
    // ================================

    IEnumerator ShakeCamera()
    {
        if (cam == null) yield break;

        Vector3 posOriginal = cam.transform.localPosition;
        float   elapsed     = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            cam.transform.localPosition = posOriginal + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = posOriginal;
    }
}

public enum ScareType
{
    Full,           // tudo junto
    SoundAndFlash,  // só som + flash
    ImageAndShadow  // só imagem + sombra
}
