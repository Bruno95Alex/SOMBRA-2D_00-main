using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Gerencia as luzes ao redor do player (área visível) e da lanterna (cone direcional).
/// 
/// SETUP NA CENA:
/// 1. No GameObject do player, adicione este script
/// 2. Crie um filho chamado "AmbientLight" com um componente Light 2D (Point/Radial)
///    → Inner Radius: 1.5  Outer Radius: 2.5  Intensity: 0.8  Color: branco levemente amarelado
/// 3. Na lanterna (filho do player que já existe), adicione um componente Light 2D (Spot/Parametric)
///    → Outer Spot Angle: 35  Inner Spot Angle: 20  Outer Radius: 6  Intensity: 1.2
/// 4. Arraste as referências no Inspector
/// 5. Global Light 2D na cena → Intensity: 0.04 (quase preto)
/// </summary>
public class PlayerLight : MonoBehaviour
{
    [Header("Luz ambiente do player (Point Light)")]
    [SerializeField] private Light2D ambientLight;
    [SerializeField] private float ambientIntensity    = 0.8f;
    [SerializeField] private float ambientInnerRadius  = 1.5f;
    [SerializeField] private float ambientOuterRadius  = 2.8f;

    [Header("Luz da lanterna (Spot Light)")]
    [SerializeField] private Light2D flashLight;
    [SerializeField] private float flashIntensityOn    = 1.2f;
    [SerializeField] private float flashIntensityOff   = 0f;
    [SerializeField] private float flashTransitionSpeed = 8f;

    [Header("Pulso suave na luz ambiente")]
    [SerializeField] private float pulseAmount = 0.08f; // variação de intensidade
    [SerializeField] private float pulseSpeed  = 1.2f;

    private float currentFlashIntensity;
    private bool flashlightOn = false;
    private float phase;

    void Start()
    {
        phase = Random.Range(0f, Mathf.PI * 2f);

        if (ambientLight != null)
        {
            ambientLight.intensity    = ambientIntensity;
            ambientLight.pointLightInnerRadius = ambientInnerRadius;
            ambientLight.pointLightOuterRadius = ambientOuterRadius;
        }
        else
        {
            Debug.LogWarning("PlayerLight: ambientLight não atribuído no Inspector.");
        }

        if (flashLight != null)
        {
            currentFlashIntensity  = 0f;
            flashLight.intensity   = 0f;
        }
        else
        {
            Debug.LogWarning("PlayerLight: flashLight não atribuído no Inspector.");
        }
    }

    void Update()
    {
        // detecta estado da lanterna pelo FlashlightController
        // (lê o estado do objeto filho que tem o Light2D da lanterna)
        if (flashLight != null)
            flashlightOn = flashLight.gameObject.activeSelf;

        AtualizarLuzAmbiente();
        AtualizarLanterna();
    }

    void AtualizarLuzAmbiente()
    {
        if (ambientLight == null) return;

        // pulso suave tipo respiração
        float pulse = Mathf.Sin(Time.time * pulseSpeed + phase) * pulseAmount;
        ambientLight.intensity = ambientIntensity + pulse;
    }

    void AtualizarLanterna()
    {
        if (flashLight == null) return;

        float target = flashlightOn ? flashIntensityOn : flashIntensityOff;
        currentFlashIntensity = Mathf.MoveTowards(
            currentFlashIntensity,
            target,
            flashTransitionSpeed * Time.deltaTime
        );

        flashLight.intensity = currentFlashIntensity;
    }
}
