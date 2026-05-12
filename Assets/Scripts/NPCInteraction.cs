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

    // private void Update()
    // {
    //     if (!playerNear || player == null)
    //         return;

    //     LookAtPlayer();

    //     if (Keyboard.current.fKey.wasPressedThisFrame)
    //     {
    //         Talk();
    //     }
    // }

        private void Update()
    {
        if (!playerNear || player == null)
            return;

        LookAtPlayer();

        // 🔥 NÃO deixa abrir outro diálogo
        if (DialogueUI.Instance != null)
        {
            if (DialogueUI.Instance.IsDialogueActive())
                return;

            // 🔥 impede reabrir instantaneamente
            if (DialogueUI.Instance.JustClosedDialogue())
                return;
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Talk();
        }
    }


    // =========================
    // VIRAR PARA PLAYER
    // =========================

    void LookAtPlayer()
{
    Vector2 dir = player.position - transform.position;

    // decide direção dominante
    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
    {
        // LADO

        animator.SetFloat("moveY", 0);

        if (dir.x > 0)
        {
            // DIREITA
            animator.SetFloat("moveX", 1);

            // sprite original olhando pra DIREITA
            spriteRenderer.flipX = false;
        }
        else
        {
            // ESQUERDA
            animator.SetFloat("moveX", 1);

            spriteRenderer.flipX = true;
        }
    }
    else
    {
        // CIMA / BAIXO

        animator.SetFloat("moveX", 0);

        if (dir.y > 0)
        {
            // CIMA
            animator.SetFloat("moveY", 1);
        }
        else
        {
            // BAIXO
            animator.SetFloat("moveY", -1);
        }
    }
}

    // =========================
    // DIÁLOGO
    // =========================

//     void Talk()
// {
//     if (DialogueUI.Instance.IsDialogueActive())
//         return;

//     string[] lines =
//     {
//         "Algo estranho está acontecendo...",
//         "A energia caiu no campus.",
//         "Procure a chave do gerador.",
//         "Ela está no laboratório 6."
//     };

//     DialogueUI.Instance.StartDialogue("Vigia", lines);
// }

void Talk()
{
    if (DialogueUI.Instance == null)
    {
        Debug.LogError("DialogueUI não encontrado!");
        return;
    }

    // 🔥 ESCONDE O TEXTO DE INTERAÇÃO
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

    // =========================
    // TRIGGER
    // =========================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            player = other.transform;

            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            player = null;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}