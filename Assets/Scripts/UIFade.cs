using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFade : MonoBehaviour
{
    public static UIFade Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float speed = 2f;

    private void Awake()
    {
        Instance = this;

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    // =========================
    // FADE OUT (transparente → preto)
    // =========================

    public IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;

        float alpha = fadeImage.color.a;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * speed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
    }

    // =========================
    // FADE IN (preto → transparente)
    // =========================

    public IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float alpha = fadeImage.color.a;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * speed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
    }
}
