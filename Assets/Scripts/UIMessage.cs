using UnityEngine;
using TMPro;
using System.Collections;

public class UIMessage : MonoBehaviour
{
    public static UIMessage Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;

    private Coroutine rotinaAtual;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string msg, float tempo = 2f)
    {
        if (rotinaAtual != null)
            StopCoroutine(rotinaAtual);

        rotinaAtual = StartCoroutine(ShowRoutine(msg, tempo));
    }

    private IEnumerator ShowRoutine(string msg, float tempo)
    {
        panel.SetActive(true);
        text.text = msg;

        yield return new WaitForSecondsRealtime(tempo);

        panel.SetActive(false);
    }

    public void Hide()
    {
        if (rotinaAtual != null)
            StopCoroutine(rotinaAtual);

        panel.SetActive(false);
    }
}