using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Porta simples já destrancada.
/// Pressione F para abrir.
/// </summary>
public class DoorUnlocked : MonoBehaviour
{
    [Header("Porta")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private string hintText = "Pressione F para abrir";

    private bool playerNear;
    private bool opened;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!playerNear || opened) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
            Open();
    }

    void Open()
    {
        opened = true;
        playerNear = false;

        if (animator != null)
            animator.SetTrigger("Open");

        if (doorCollider != null)
            doorCollider.enabled = false;

        UIMessage.Instance.Hide();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (opened || !other.CompareTag("Player")) return;
        playerNear = true;
        UIMessage.Instance.Show(hintText, 999f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = false;
        UIMessage.Instance.Hide();
    }
}
