using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Coloque este script em cada janela da cena.
/// O fade agora é controlado pelo LightningSystem (sem Coroutine local).
/// </summary>
public class WindowLight : MonoBehaviour
{
    [Header("Luz da janela")]
    [SerializeField] private Light2D windowLight2D;
    [SerializeField] private float flashIntensity = 1.4f;
    [SerializeField] private float fadeOutSpeed   = 6f;

    [Header("Sprite da janela (opcional)")]
    [SerializeField] private SpriteRenderer windowSprite;
    [SerializeField] private Color spriteFlashColor = new Color(0.8f, 0.85f, 1f);

    private Color spriteOriginalColor;

    // estado atual — o LightningSystem chama Update indiretamente via Tick()
    private bool fading = false;

    void Start()
    {
        if (windowLight2D == null)
            windowLight2D = GetComponent<Light2D>();
        if (windowLight2D == null)
            windowLight2D = GetComponentInChildren<Light2D>();

        if (windowLight2D != null)
            windowLight2D.intensity = 0f;

        if (windowSprite != null)
            spriteOriginalColor = windowSprite.color;
    }

    // =========================
    // CHAMADO PELO LightningSystem
    // =========================

    public void FlashOn()
    {
        fading = false;

        if (windowLight2D != null)
            windowLight2D.intensity = flashIntensity;

        if (windowSprite != null)
            windowSprite.color = spriteFlashColor;
    }

    public void FlashOff()
    {
        fading = true; // fade é feito via Tick() chamado pelo LightningSystem
    }

    /// <summary>
    /// Chamado pelo LightningSystem.Update() a cada frame.
    /// Assim o fade roda no objeto sempre ativo (LightningSystem),
    /// sem depender do estado do GameObject da janela.
    /// </summary>
    public void Tick(float deltaTime)
    {
        if (!fading || windowLight2D == null) return;

        windowLight2D.intensity = Mathf.MoveTowards(
            windowLight2D.intensity, 0f, fadeOutSpeed * deltaTime
        );

        if (windowSprite != null)
            windowSprite.color = Color.Lerp(
                windowSprite.color, spriteOriginalColor, fadeOutSpeed * deltaTime
            );

        if (windowLight2D.intensity <= 0.01f)
        {
            windowLight2D.intensity = 0f;
            if (windowSprite != null)
                windowSprite.color = spriteOriginalColor;
            fading = false;
        }
    }
}
