using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Image transitionImage;
    [SerializeField] private TMP_Text transitionText;

    private void Awake()
    {
        Instance = this;

        panel.SetActive(false);
    }

    public IEnumerator ShowTransition(
        Sprite image,
        string text,
        float duration = 3f)
    {
        panel.SetActive(true);

        transitionImage.sprite = image;
        transitionText.text = text;

        RectTransform rt =
            transitionImage.rectTransform;

        rt.localScale = Vector3.one;

        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            rt.localScale =
                Vector3.Lerp(
                    Vector3.one,
                    Vector3.one * 1.15f,
                    t);

            yield return null;
        }

        panel.SetActive(false);
    }
}