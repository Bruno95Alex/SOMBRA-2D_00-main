using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject interactUI;

    [Header("NPC")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool playerNear;
    private Transform player;

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Update()
    {
        if (!playerNear || player == null) return;

        LookAtPlayer();

        if (DialogueUI.Instance != null)
        {
            if (DialogueUI.Instance.IsDialogueActive())  return;
            if (DialogueUI.Instance.JustClosedDialogue()) return;
        }

        // teclado F OU Triângulo no controle via InputReader
        bool interact = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame;

        if (interact)
            Talk();
    }

    void LookAtPlayer()
    {
        Vector2 dir = player.position - transform.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            animator.SetFloat("moveY", 0);
            animator.SetFloat("moveX", 1);
            spriteRenderer.flipX = dir.x < 0;
        }
        else
        {
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", dir.y > 0 ? 1 : -1);
        }
    }

    void Talk()
    {
        if (DialogueUI.Instance == null) { Debug.LogError("DialogueUI não encontrado!"); return; }

        if (interactUI != null)
            interactUI.SetActive(false);

        string[] lines =
        {
            "Algo estranho está acontecendo...",
            "A energia caiu no campus.",
            "Procure a chave do gerador.",
            "Ela está no laboratório 6."
        };

        DialogueUI.Instance.StartDialogue("Vigia", lines);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = true;
        player = other.transform;
        if (interactUI != null) interactUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = false;
        player = null;
        if (interactUI != null) interactUI.SetActive(false);
    }
}
