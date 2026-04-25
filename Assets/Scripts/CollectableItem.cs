// using UnityEngine;
// using UnityEngine.InputSystem;

// public class CollectableItem : MonoBehaviour
// {
//     [SerializeField] private Sprite itemIcon;

//     private bool playerPerto = false;

//     void Update()
//     {
//         if (playerPerto && Keyboard.current.fKey.wasPressedThisFrame)
//         {
//             Coletar();
//         }
//     }

//     void Coletar()
//     {
//         InventorySystem.Instance.AddItem(itemIcon);

//         Destroy(gameObject);
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Player"))
//         {
//             playerPerto = true;
//         }
//     }

//     private void OnTriggerExit2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Player"))
//         {
//             playerPerto = false;
//         }
//     }
// }



using UnityEngine;
using UnityEngine.InputSystem;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private bool playerPerto = false;

    void Update()
    {
        if (playerPerto && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Coletar();
        }
    }

    void Coletar()
    {
        InventorySystem.Instance.AddItem(itemData);

        PickupUI.Instance.HideText();

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = true;

            PickupUI.Instance.ShowText();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = false;

            PickupUI.Instance.HideText();
        }
    }
}