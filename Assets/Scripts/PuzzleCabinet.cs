using UnityEngine;
using System.Collections;

public class PuzzleCabinet : MonoBehaviour
{
    public enum CabinetType { Lightning, Normal }

    [Header("Tipo")]
    [SerializeField] private CabinetType type = CabinetType.Normal;

    [Header("Animação")]
    [SerializeField] private Animator animator;

    [Header("Sons")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;
    [SerializeField] private AudioClip wrongClip;

    [Header("Dicas")]
    [SerializeField] private string hintNormal    = "Pressione F para abrir";
    [SerializeField] private string hintLightning = "Algo neste armário só responde à luz certa...";

    public bool IsOpen => isOpen;

    private bool playerNear = false;
    private bool isOpen     = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        if (!playerNear || isOpen) return;
        if (CabinetPuzzleManager.Instance != null && CabinetPuzzleManager.Instance.IsResetting()) return;

        bool interact = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame;

        if (interact)
        {
            Debug.Log($"[Cabinet] Tentando abrir: {gameObject.name} | isOpen={isOpen} | playerNear={playerNear}");
            TryOpen();
        }
    }

    // =========================
    // ABRIR
    // =========================

    void TryOpen()
    {
        if (type == CabinetType.Lightning)
        {
            if (LightningSystem.Instance == null || !LightningSystem.Instance.IsLightningActive())
            {
                UIMessage.Instance.Show(hintLightning, 2f);
                return;
            }
        }

        Open();
    }

    public void Open()
    {
        isOpen = true;

        PlayAnim("Open");

        if (audioSource != null && openClip != null)
            audioSource.PlayOneShot(openClip);

        UIMessage.Instance.Hide();

        // avisa o manager no próximo frame — garante que isOpen já está true
        StartCoroutine(NotificarManager());
    }

    IEnumerator NotificarManager()
    {
        yield return null;
        Debug.Log($"[Cabinet] Notificando manager: {gameObject.name} | isOpen={isOpen} | resetting={CabinetPuzzleManager.Instance.IsResetting()}");
        CabinetPuzzleManager.Instance.OnCabinetOpened(this);
    }

    // =========================
    // FECHAR
    // =========================

    public void ForceClose()
    {
        if (CabinetPuzzleManager.Instance != null && CabinetPuzzleManager.Instance.IsSolved())
            return;

        StopAllCoroutines();
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        PlayAnim("Close");

        if (audioSource != null)
            audioSource.PlayOneShot(wrongClip != null ? wrongClip : closeClip);

        yield return new WaitForSeconds(0.8f);

        isOpen = false;

        if (animator != null)
            animator.ResetTrigger("Close");
    }

    void PlayAnim(string trigger)
    {
        if (animator == null) return;
        animator.ResetTrigger("Open");
        animator.ResetTrigger("Close");
        animator.SetTrigger(trigger);
    }

    // =========================
    // TRIGGERS PLAYER
    // =========================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isOpen) return;
        playerNear = true;
        UIMessage.Instance.Show(
            type == CabinetType.Lightning ? hintLightning : hintNormal, 999f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = false;
        UIMessage.Instance.Hide();
    }
}
