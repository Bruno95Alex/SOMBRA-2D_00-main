using UnityEngine;

public class DoorLocked : MonoBehaviour
{
    [Header("Itens Necessários (deixe vazio se a porta abre só por puzzle)")]
    [SerializeField] private ItemData[] requiredItems;

    [Header("Porta")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D doorCollider;

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

        bool interact = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame;

        if (!interact) return;

        if (requiredItems.Length == 0)
        {
            UIMessage.Instance.Show("Esta porta está trancada.", 2f);
            return;
        }

        if (HasAllItems()) OpenDoor();
        else UIMessage.Instance.Show("Faltam itens para poder sair", 2f);
    }

    bool HasAllItems()
    {
        foreach (ItemData item in requiredItems)
            if (!InventorySystem.Instance.HasItem(item))
                return false;
        return true;
    }

    void OpenDoor()
    {
        foreach (ItemData item in requiredItems)
            InventorySystem.Instance.RemoveItem(item);
        AbrirAnimacao();
    }

    public void UnlockByPuzzle()
    {
        if (opened) return;
        AbrirAnimacao();
    }

    void AbrirAnimacao()
    {
        UIMessage.Instance.Show("Abrindo porta...", 1.5f);
        if (animator != null) animator.SetTrigger("Open");
        opened     = true;
        playerNear = false;
        if (doorCollider != null) doorCollider.enabled = false;
        Invoke(nameof(HideMessage), 1.5f);
    }

    void HideMessage() => UIMessage.Instance.Hide();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (opened || !other.CompareTag("Player")) return;
        playerNear = true;
        UIMessage.Instance.Show(
            (requiredItems.Length == 0 || !HasAllItems())
                ? "Porta trancada"
                : "Pressione F / botão para abrir",
            999f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (opened || !other.CompareTag("Player")) return;
        playerNear = false;
        UIMessage.Instance.Hide();
    }
}
