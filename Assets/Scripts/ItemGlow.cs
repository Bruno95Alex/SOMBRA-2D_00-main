using UnityEngine;

/// <summary>
/// Brilho pulsante visível em cenas escuras com URP + Global Light 2D.
/// SETUP: SpriteRenderer → Material → "Sprite-Lit-Default"
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ItemGlow : MonoBehaviour
{
    [Header("Flutuação")]
    [SerializeField] private float floatAmount = 0.07f;
    [SerializeField] private float floatSpeed  = 1.2f;

    [Header("Cor do brilho")]
    [SerializeField] private Color glowColor   = new Color(1f, 0.9f, 0.2f); // amarelo dourado

    [Header("Brilho (HDR — valores acima de 1 emitem luz)")]
    [SerializeField] private float brightnessMin = 1.0f;
    [SerializeField] private float brightnessMax = 3.5f;
    [SerializeField] private float pulseSpeed    = 1.8f;

    [Header("Com lanterna")]
    [SerializeField] private float brightnessLit = 5.0f;
    [SerializeField] private float litSpeed      = 8f;

    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 startLocalPos;
    private float phase;
    private bool isLit;
    private float currentBrightness;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void Start()
    {
        startLocalPos = transform.localPosition;
        phase = Random.Range(0f, Mathf.PI * 2f);
        currentBrightness = brightnessMin;

        if (sr.sharedMaterial != null &&
            sr.sharedMaterial.name.Contains("Default-Sprite"))
        {
            Debug.LogWarning(
                "ItemGlow: material 'Default-Sprite' detectado. " +
                "Troque para 'Sprite-Lit-Default' para o brilho funcionar em cenas escuras."
            );
        }
    }

    void Update()
    {
        float t = Time.time + phase;

        // flutuação vertical
        float y = Mathf.Sin(t * floatSpeed) * floatAmount;
        transform.localPosition = startLocalPos + new Vector3(0f, y, 0f);

        // brilho alvo
        float targetBrightness;

        if (isLit)
        {
            targetBrightness = brightnessLit;
        }
        else
        {
            float sin = (Mathf.Sin(t * pulseSpeed) + 1f) * 0.5f;
            targetBrightness = Mathf.Lerp(brightnessMin, brightnessMax, sin);
        }

        float vel = isLit ? litSpeed : pulseSpeed * 2f;
        currentBrightness = Mathf.MoveTowards(currentBrightness, targetBrightness, vel * Time.deltaTime);

        // interpola entre cor original e cor do brilho conforme intensidade
        float t01 = Mathf.InverseLerp(brightnessMin, brightnessMax, currentBrightness);
        Color baseColor = Color.Lerp(originalColor, glowColor, t01);

        sr.color = baseColor * currentBrightness;
    }

    public void SetLit(bool lit)
    {
        isLit = lit;
    }

    void OnDisable()
    {
        if (sr != null) sr.color = originalColor;
        transform.localPosition = startLocalPos;
    }
}
