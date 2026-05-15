using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Coloque na mesa/papel com a pista do puzzle.
/// Ao pressionar F, abre o DiaryUI com o texto da pista.
/// </summary>
public class PuzzleNote : MonoBehaviour
{
    [Header("Pista")]
    [TextArea(4, 10)]
    [SerializeField] private string noteText =
        "A ordem correta abre o caminho.\n\n" +
        "Primeiro... aquele que recebe a luz da tormenta.\n" +
        "Depois... o que está no meio, sempre observando.\n" +
        "Por último... o mais escondido nas sombras.\n\n" +
        "Erre, e tudo recomeça.";

    private bool playerNear;

    void Update()
    {
        if (playerNear && Keyboard.current.fKey.wasPressedThisFrame)
            DiaryUI.Instance.ShowPage(noteText);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = true;
        UIMessage.Instance.Show("Pressione F para ler o papel", 999f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = false;
        UIMessage.Instance.Hide();
    }
}
