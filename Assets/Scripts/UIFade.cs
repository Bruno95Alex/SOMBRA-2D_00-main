// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class UIFade : MonoBehaviour
// {
//     public static UIFade Instance;

//     [SerializeField] private Image fadeImage;
//     [SerializeField] private float speed = 2f;

//     void Awake()
//     {
//         Instance = this;

//         if (fadeImage == null)
//         {
//             Debug.LogError("❌ FadeImage NÃO foi atribuído no Inspector!");
//             return;
//         }

//         // 🔥 começa invisível
//         Color c = fadeImage.color;
//         c.a = 0;
//         fadeImage.color = c;
//     }

//     public IEnumerator FadeOut()
//     {
//         if (fadeImage == null) yield break;

//         float t = 0;

//         while (t < 1)
//         {
//             t += Time.deltaTime * speed;

//             Color c = fadeImage.color;
//             c.a = t;
//             fadeImage.color = c;

//             yield return null;
//         }

//         // garante 100% preto
//         Color final = fadeImage.color;
//         final.a = 1;
//         fadeImage.color = final;
//     }

//     public IEnumerator FadeIn()
//     {
//         if (fadeImage == null) yield break;

//         float t = 1;

//         while (t > 0)
//         {
//             t -= Time.deltaTime * speed;

//             Color c = fadeImage.color;
//             c.a = t;
//             fadeImage.color = c;

//             yield return null;
//         }

//         // garante invisível
//         Color final = fadeImage.color;
//         final.a = 0;
//         fadeImage.color = final;
//     }
// }


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
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;
        }
    }

    // =========================
    // FADE OUT
    // =========================

    public IEnumerator FadeOut()
    {
        if (fadeImage == null)
            yield break;

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
    // FADE IN
    // =========================

    public IEnumerator FadeIn()
    {
        if (fadeImage == null)
            yield break;

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