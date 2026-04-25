// using UnityEngine;
// using UnityEngine.InputSystem;

// public class ItemPickup : MonoBehaviour
// {
//     [SerializeField] private ItemData itemData;

//     private bool playerNear;

//     void Update()
//     {
//         if (playerNear && Keyboard.current.fKey.wasPressedThisFrame)
//         {
//             InventorySystem.Instance.AddItem(itemData);

//             UIMessage.Instance.Show("Item coletado!", 1.5f);

//             Destroy(gameObject);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerNear = true;
//             UIMessage.Instance.Show("Pressione F para coletar", 999f);
//         }
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerNear = false;
//             UIMessage.Instance.Hide();
//         }
//     }
// }

using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private bool playerNear;

    void Update()
    {
        if (playerNear && Keyboard.current.fKey.wasPressedThisFrame)
        {
            InventorySystem.Instance.AddItem(itemData);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            UIMessage.Instance.Show("Pressione F para pegar", 999f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            UIMessage.Instance.Hide();
        }
    }
}