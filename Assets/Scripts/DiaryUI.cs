using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class DiaryUI : MonoBehaviour
{
    public static DiaryUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI textUI;

    private bool visivel = false;
    private bool bloqueado = false; // evita fechar no mesmo frame que abriu

    void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (!visivel || bloqueado) return;

        bool fechar = (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                   || (InputReader.Instance != null && InputReader.Instance.InteractPressed);

        if (fechar) Close();
    }

    public void ShowPage(string text)
    {
        panel.SetActive(true);
        textUI.text    = text;
        Time.timeScale = 0f;
        visivel        = true;

        StartCoroutine(BloquearPorUmFrame());
    }

    IEnumerator BloquearPorUmFrame()
    {
        bloqueado = true;
        yield return null;
        yield return null;
        bloqueado = false;
    }

    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        visivel        = false;
    }
}
