using UnityEngine;
using UnityEngine.InputSystem;

public class DoorLocked : MonoBehaviour
{
    [Header("Itens Necessários")]
    [SerializeField] private ItemData[] requiredItems;

    [Header("Porta")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D doorCollider;

    private bool playerNear;
    private bool opened;

    // =========================

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // =========================

    void Update()
    {
        if (!playerNear || opened) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (HasAllItems())
                OpenDoor();
            else
                UIMessage.Instance.Show("Faltam itens para poder sair", 2f);
        }
    }

    // =========================

    bool HasAllItems()
    {
        foreach (ItemData item in requiredItems)
        {
            if (!InventorySystem.Instance.HasItem(item))
                return false;
        }
        return true;
    }

    // =========================

    void OpenDoor()
    {
        foreach (ItemData item in requiredItems)
            InventorySystem.Instance.RemoveItem(item);

        UIMessage.Instance.Show("Abrindo porta...", 1.5f);

        if (animator != null)
            animator.SetTrigger("Open");

        opened = true;
        playerNear = false;

        if (doorCollider != null)
            doorCollider.enabled = false;

        Invoke(nameof(HideMessage), 1.5f);
    }

    void HideMessage()
    {
        UIMessage.Instance.Hide();
    }

    // =========================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return;

        if (other.CompareTag("Player"))
        {
            playerNear = true;

            if (HasAllItems())
                UIMessage.Instance.Show("Pressione F para abrir", 999f);
            else
                UIMessage.Instance.Show("Porta trancada", 999f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (opened) return;

        if (other.CompareTag("Player"))
        {
            playerNear = false;
            UIMessage.Instance.Hide();
        }
    }
}
