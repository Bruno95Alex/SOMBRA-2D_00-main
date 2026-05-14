using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

/// <summary>
/// Sistema de relâmpagos com luz entrando por janelas e som de trovão.
///
/// SETUP:
/// 1. Crie um GameObject vazio na cena chamado "LightningSystem"
/// 2. Adicione este script nele
/// 3. Em cada janela da cena, adicione o script "WindowLight"
/// 4. Arraste as referências no Inspector
/// </summary>
public class LightningSystem : MonoBehaviour
{
    public static LightningSystem Instance;

    [Header("Intervalo entre relâmpagos")]
    [SerializeField] private float intervalMin = 4f;
    [SerializeField] private float intervalMax = 12f;

    [Header("Luz global do relâmpago (Global Light 2D da cena)")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float globalLightNormal    = 0.04f; // intensidade base da noite
    [SerializeField] private float globalLightFlash     = 0.55f; // intensidade do flash

    [Header("Som")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] thunderClips;  // coloque 2-3 sons diferentes
    [SerializeField] private float thunderDelayMin = 0.4f; // delay entre flash e trovão
    [SerializeField] private float thunderDelayMax = 1.2f;

    [Header("Flash")]
    [SerializeField] private float flashDuration   = 0.08f; // duração de cada piscada
    [SerializeField] private int   flashCount      = 3;     // quantas piscadas por relâmpago
    [SerializeField] private float flashInterval   = 0.12f; // intervalo entre piscadas

    // janelas registradas na cena
    private WindowLight[] windows;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        windows = FindObjectsByType<WindowLight>(FindObjectsSortMode.None);

        if (globalLight == null)
            Debug.LogWarning("LightningSystem: Global Light 2D não atribuído.");

        StartCoroutine(LightningLoop());
    }

    // =========================
    // LOOP PRINCIPAL
    // =========================

    IEnumerator LightningLoop()
    {
        while (true)
        {
            float wait = Random.Range(intervalMin, intervalMax);
            yield return new WaitForSeconds(wait);

            yield return StartCoroutine(DoLightning());
        }
    }

    // =========================
    // RELÂMPAGO
    // =========================

    IEnumerator DoLightning()
    {
        // escolhe uma janela aleatória (ou todas)
        WindowLight[] targets = EscolherJanelas();

        // sequência de piscadas
        for (int i = 0; i < flashCount; i++)
        {
            FlashOn(targets);
            yield return new WaitForSeconds(flashDuration);

            FlashOff(targets);

            if (i < flashCount - 1)
                yield return new WaitForSeconds(flashInterval);
        }

        // trovão com delay (distância simulada)
        float delay = Random.Range(thunderDelayMin, thunderDelayMax);
        StartCoroutine(PlayThunder(delay));
    }

    // =========================
    // FLASH ON / OFF
    // =========================

    void FlashOn(WindowLight[] targets)
    {
        if (globalLight != null)
            globalLight.intensity = globalLightFlash;

        foreach (var w in targets)
            w.FlashOn();
    }

    void FlashOff(WindowLight[] targets)
    {
        if (globalLight != null)
            globalLight.intensity = globalLightNormal;

        foreach (var w in targets)
            w.FlashOff();
    }

    // =========================
    // ESCOLHER JANELAS
    // =========================

    WindowLight[] EscolherJanelas()
    {
        if (windows == null || windows.Length == 0)
            return new WindowLight[0];

        // chance de iluminar todas ou só algumas
        bool iluminarTodas = Random.value > 0.4f;

        if (iluminarTodas)
            return windows;

        // escolhe entre 1 e metade das janelas aleatoriamente
        int count = Random.Range(1, Mathf.Max(2, windows.Length / 2 + 1));
        WindowLight[] shuffled = (WindowLight[])windows.Clone();

        // embaralha
        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        WindowLight[] result = new WindowLight[count];
        for (int i = 0; i < count; i++)
            result[i] = shuffled[i];

        return result;
    }

    // =========================
    // TROVÃO
    // =========================

    IEnumerator PlayThunder(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (audioSource == null || thunderClips == null || thunderClips.Length == 0)
            yield break;

        AudioClip clip = thunderClips[Random.Range(0, thunderClips.Length)];
        audioSource.volume = Random.Range(0.7f, 1f);
        audioSource.PlayOneShot(clip);
    }
}
