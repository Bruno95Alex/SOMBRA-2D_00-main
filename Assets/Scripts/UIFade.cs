using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFade : MonoBehaviour
{
    public static UIFade Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float speed = 2f;

    void Awake()
    {
        Instance = this;

        if (fadeImage == null)
        {
            Debug.LogError("❌ FadeImage NÃO foi atribuído no Inspector!");
        }
    }

    public IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float t = 1;

        while (t > 0)
        {
            t -= Time.deltaTime * speed;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }
}