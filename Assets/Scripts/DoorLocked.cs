// using UnityEngine;
// using UnityEngine.InputSystem;

// public class DoorLocked : MonoBehaviour
// {
//     [SerializeField] private ItemData keyItem;
//     [SerializeField] private Animator animator;
//     [SerializeField] private Collider2D doorCollider; // 👈 USAR ESSE

//     private bool playerNear;
//     private bool opened = false;

//     void Awake()
//     {
//         if (animator == null)
//             animator = GetComponent<Animator>();
//     }

//     void Update()
//     {
//         if (!playerNear || opened) return;

//         if (Keyboard.current.fKey.wasPressedThisFrame)
//         {
//             if (InventorySystem.Instance.HasItem(keyItem))
//             {
//                 InventorySystem.Instance.RemoveItem(keyItem);

//                 UIMessage.Instance.Show("Abrindo porta...", 1.5f);

//                 animator.SetTrigger("Open");

//                 opened = true;
//                 playerNear = false;

//                 UIMessage.Instance.Hide();

//                 // 🔥 DESATIVA COLISÃO CORRETA
//                 if (doorCollider != null)
//                     doorCollider.enabled = false;
//             }
//             else
//             {
//                 UIMessage.Instance.Show("Porta trancada", 2f);
//             }
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (opened) return;

//         if (other.CompareTag("Player"))
//         {
//             playerNear = true;

//             if (InventorySystem.Instance.HasItem(keyItem))
//                 UIMessage.Instance.Show("Pressione F para abrir", 999f);
//             else
//                 UIMessage.Instance.Show("Porta trancada", 999f);
//         }
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (opened) return;

//         if (other.CompareTag("Player"))
//         {
//             playerNear = false;
//             UIMessage.Instance.Hide();
//         }
//     }
// }



using UnityEngine;
using UnityEngine.InputSystem;

public class DoorLocked : MonoBehaviour
{
    [SerializeField] private ItemData keyItem;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D doorCollider;

    private bool playerNear;
    private bool opened = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!playerNear || opened) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (InventorySystem.Instance.HasItem(keyItem))
            {
                InventorySystem.Instance.RemoveItem(keyItem);

                UIMessage.Instance.Show("Abrindo porta...", 1.5f);

                animator.SetTrigger("Open");

                opened = true;
                playerNear = false;

                UIMessage.Instance.Hide();

                if (doorCollider != null)
                    doorCollider.enabled = false;
            }
            else
            {
                UIMessage.Instance.Show("Porta trancada", 2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return;

        if (other.CompareTag("Player"))
        {
            playerNear = true;

            if (InventorySystem.Instance.HasItem(keyItem))
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