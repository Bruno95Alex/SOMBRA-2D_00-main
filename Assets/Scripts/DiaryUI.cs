using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DiaryUI : MonoBehaviour
{
    public static DiaryUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI textUI;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!panel.activeSelf) return;

        // fecha com Escape OU Bola (InventoryPressed) OU Triângulo
        bool fechar = (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                   || (InputReader.Instance != null && InputReader.Instance.InventoryPressed)
                   || (InputReader.Instance != null && InputReader.Instance.InteractPressed);

        if (fechar) Close();
    }

    public void ShowPage(string text)
    {
        panel.SetActive(true);
        textUI.text = text;
        Time.timeScale = 0f;
    }

    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
