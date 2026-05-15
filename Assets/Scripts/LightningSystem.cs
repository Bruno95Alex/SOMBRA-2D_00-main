using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightningSystem : MonoBehaviour
{
    public static LightningSystem Instance;

    [Header("Intervalo entre relâmpagos (aleatório real)")]
    [SerializeField] private float intervalBase = 8f; // tempo médio — quanto maior, mais raro

    [Header("Luz global (Global Light 2D da cena)")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float globalLightNormal = 0.04f;
    [SerializeField] private float globalLightFlash  = 0.55f;

    [Header("Som do trovão")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] thunderClips;
    [SerializeField] private float thunderDelayMin = 0.4f;
    [SerializeField] private float thunderDelayMax = 1.2f;
    [SerializeField] [Range(0f, 1f)] private float thunderVolumeMin = 0.2f;
    [SerializeField] [Range(0f, 1f)] private float thunderVolumeMax = 0.9f;

    [Header("Flash")]
    [SerializeField] private float flashDuration = 0.08f;
    [SerializeField] private int   flashCount    = 3;
    [SerializeField] private float flashInterval = 0.12f;

    [Header("Janelas (arraste manualmente se necessário)")]
    [SerializeField] private WindowLight[] windowsManual;

    private WindowLight[] windows;
    private bool lightningActive = false;
    public bool IsLightningActive() => lightningActive;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BuscarJanelas();

        if (globalLight == null)
            Debug.LogWarning("LightningSystem: Global Light 2D não atribuído.");

        StartCoroutine(LightningLoop());
    }

    void BuscarJanelas()
    {
        windows = FindObjectsByType<WindowLight>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        if ((windows == null || windows.Length == 0) && windowsManual.Length > 0)
            windows = windowsManual;

        if (windows == null || windows.Length == 0)
            Debug.LogWarning("LightningSystem: nenhuma WindowLight encontrada.");
        else
            Debug.Log($"LightningSystem: {windows.Length} janela(s) encontrada(s).");
    }

    void Update()
    {
        if (windows == null) return;
        foreach (var w in windows)
            if (w != null) w.Tick(Time.deltaTime);
    }

    // =========================
    // LOOP — TEMPO ALEATÓRIO REAL
    // distribuição exponencial: sem min/max fixo,
    // às vezes vem logo, às vezes demora muito
    // =========================

    IEnumerator LightningLoop()
    {
        yield return null;
        BuscarJanelas();

        while (true)
        {
            // distribuição exponencial — simula trovões reais
            // -ln(random) * base dá tempos imprevisíveis naturais
            float wait = -Mathf.Log(Random.value) * intervalBase;
            wait = Mathf.Max(2f, wait); // mínimo de 2s para não sobrepor
            yield return new WaitForSeconds(wait);

            yield return StartCoroutine(DoLightning());
        }
    }

    IEnumerator DoLightning()
    {
        lightningActive = true;

        WindowLight[] targets = EscolherJanelas();

        for (int i = 0; i < flashCount; i++)
        {
            FlashOn(targets);
            yield return new WaitForSeconds(flashDuration);
            FlashOff(targets);

            if (i < flashCount - 1)
                yield return new WaitForSeconds(flashInterval);
        }

        lightningActive = false;

        // delay aleatório entre flash e trovão
        float delay = -Mathf.Log(Random.value) * ((thunderDelayMin + thunderDelayMax) / 2f);
        delay = Mathf.Clamp(delay, thunderDelayMin, thunderDelayMax * 2f);
        StartCoroutine(PlayThunder(delay));
    }

    void FlashOn(WindowLight[] targets)
    {
        if (globalLight != null) globalLight.intensity = globalLightFlash;
        foreach (var w in targets) if (w != null) w.FlashOn();
    }

    void FlashOff(WindowLight[] targets)
    {
        if (globalLight != null) globalLight.intensity = globalLightNormal;
        foreach (var w in targets) if (w != null) w.FlashOff();
    }

    WindowLight[] EscolherJanelas()
    {
        if (windows == null || windows.Length == 0) return new WindowLight[0];

        bool todas = Random.value > 0.4f;
        if (todas) return windows;

        int count = Random.Range(1, Mathf.Max(2, windows.Length / 2 + 1));
        WindowLight[] shuffled = (WindowLight[])windows.Clone();

        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        WindowLight[] result = new WindowLight[count];
        for (int i = 0; i < count; i++) result[i] = shuffled[i];
        return result;
    }

    IEnumerator PlayThunder(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (audioSource == null || thunderClips == null || thunderClips.Length == 0) yield break;

        // clipe e volume completamente aleatórios a cada trovão
        AudioClip clip = thunderClips[Random.Range(0, thunderClips.Length)];
        audioSource.volume = Random.Range(thunderVolumeMin, thunderVolumeMax);
        audioSource.PlayOneShot(clip);
    }
}
