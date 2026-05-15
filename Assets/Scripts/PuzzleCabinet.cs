using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Coloque em cada armário do puzzle.
/// Tipos:
///   Lightning → só abre durante um relâmpago
///   Normal    → abre normalmente com F
/// </summary>
public class PuzzleCabinet : MonoBehaviour
{
    public enum CabinetType { Lightning, Normal }

    [Header("Tipo")]
    [SerializeField] private CabinetType type = CabinetType.Normal;

    [Header("Animação")]
    [SerializeField] private Animator animator;         // trigger "Open" e "Close"
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openClip;        // clique metálico ao abrir
    [SerializeField] private AudioClip closeClip;       // som de fechar

    [Header("Dica extra (só no tipo Lightning)")]
    [SerializeField] private string hintNormal    = "Pressione F para abrir";
    [SerializeField] private string hintLightning = "Algo neste armário só aparece com a luz certa...";

    private bool playerNear = false;
    private bool isOpen     = false;

    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        if (!playerNear || isOpen) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
            TryOpen();
    }

    // =========================
    // TENTATIVA DE ABRIR
    // =========================

    void TryOpen()
    {
        if (type == CabinetType.Lightning)
        {
            // só abre se estiver relampejando agora
            if (!LightningSystem.Instance.IsLightningActive())
            {
                UIMessage.Instance.Show(hintLightning, 2f);
                return;
            }
        }

        Open();
    }

    // =========================
    // ABRIR
    // =========================

    void Open()
    {
        isOpen = true;

        if (animator != null)
            animator.SetTrigger("Open");

        if (audioSource != null && openClip != null)
            audioSource.PlayOneShot(openClip);

        UIMessage.Instance.Hide();

        // avisa o manager
        CabinetPuzzleManager.Instance.OnCabinetOpened(this);
    }

    // =========================
    // FECHAR (chamado pelo manager no erro)
    // =========================

    public void ForceClose()
    {
        if (!isOpen) return;

        isOpen = false;

        if (animator != null)
            animator.SetTrigger("Close");

        if (audioSource != null && closeClip != null)
            audioSource.PlayOneShot(closeClip);
    }

    // =========================
    // TRIGGER PLAYER
    // =========================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isOpen) return;

        playerNear = true;
        UIMessage.Instance.Show(
            type == CabinetType.Lightning ? hintLightning : hintNormal,
            999f
        );
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNear = false;
        UIMessage.Instance.Hide();
    }
}
