using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

/// <summary>
/// Coloque este script em cada janela da cena.
/// Requer um Light 2D no mesmo GameObject ou num filho.
///
/// SETUP DA JANELA:
/// 1. No GameObject da janela, adicione um Light 2D
///    → Light Type: Spot (Parametric) ou Freeform
///    → Aponte na direção que a luz entraria pelo vidro
///    → Outer Radius: 4-6  Intensity: 0 (começa apagado)
///    → Color: azul-branco frio (#C8D8FF) para luz de relâmpago
/// 2. Adicione este script no mesmo GameObject
/// </summary>
public class WindowLight : MonoBehaviour
{
    [Header("Luz da janela")]
    [SerializeField] private Light2D windowLight2D;
    [SerializeField] private float flashIntensity = 1.4f;  // intensidade no flash
    [SerializeField] private float fadeOutSpeed   = 6f;    // queda suave após o flash

    [Header("Sprite da janela (opcional)")]
    [SerializeField] private SpriteRenderer windowSprite;  // se quiser clarear o sprite também
    [SerializeField] private Color spriteFlashColor = new Color(0.8f, 0.85f, 1f); // azulado

    private Color spriteOriginalColor;
    private bool isFlashing = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (windowLight2D == null)
            windowLight2D = GetComponent<Light2D>();

        if (windowLight2D == null)
            windowLight2D = GetComponentInChildren<Light2D>();

        if (windowLight2D == null)
            Debug.LogWarning("WindowLight: Light2D não encontrado em " + gameObject.name);
        else
            windowLight2D.intensity = 0f;

        if (windowSprite != null)
            spriteOriginalColor = windowSprite.color;
    }

    // =========================
    // CHAMADO PELO LIGHTNINSYSTEM
    // =========================

    public void FlashOn()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (windowLight2D != null)
            windowLight2D.intensity = flashIntensity;

        if (windowSprite != null)
            windowSprite.color = spriteFlashColor;
    }

    public void FlashOff()
    {
        // não apaga instantaneamente — fade suave
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    // =========================
    // FADE SUAVE
    // =========================

    IEnumerator FadeOut()
    {
        while (windowLight2D != null && windowLight2D.intensity > 0.01f)
        {
            windowLight2D.intensity = Mathf.MoveTowards(
                windowLight2D.intensity,
                0f,
                fadeOutSpeed * Time.deltaTime
            );

            if (windowSprite != null)
                windowSprite.color = Color.Lerp(
                    windowSprite.color,
                    spriteOriginalColor,
                    fadeOutSpeed * Time.deltaTime
                );

            yield return null;
        }

        if (windowLight2D != null)
            windowLight2D.intensity = 0f;

        if (windowSprite != null)
            windowSprite.color = spriteOriginalColor;
    }
}
