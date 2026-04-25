// using UnityEngine;
// using UnityEngine.InputSystem;

// public class DiaryPage : MonoBehaviour
// {
//     [TextArea(5, 10)]
//     public string pageText;

//     public Sprite pageIcon;

//     private bool playerNear;

//     void Update()
//     {
//         if (!playerNear) return;

//         if (Keyboard.current.fKey.wasPressedThisFrame)
//         {
//             // adiciona no inventário
//             if (InventorySystem.Instance != null && pageIcon != null)
//                 InventorySystem.Instance.AddItem(pageIcon);

//             // mostra texto
//             if (DiaryUI.Instance != null)
//                 DiaryUI.Instance.ShowPage(pageText);

//             // esconde mensagem
//             if (UIMessage.Instance != null)
//                 UIMessage.Instance.Hide();

//             Destroy(gameObject);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (!other.CompareTag("Player")) return;

//         playerNear = true;

//         if (UIMessage.Instance != null)
//             UIMessage.Instance.Show("Pressione F para ler", 999f);
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (!other.CompareTag("Player")) return;

//         playerNear = false;

//         if (UIMessage.Instance != null)
//             UIMessage.Instance.Hide();
//     }
// }


// using UnityEngine;
// using UnityEngine.InputSystem;

// public class DiaryPage : MonoBehaviour
// {
//     public ItemData itemData;

//     private bool playerNear;

//     void Update()
//     {
//         if (!playerNear) return;

//         if (Keyboard.current.fKey.wasPressedThisFrame)
//         {
//             InventorySystem.Instance.AddItem(itemData);

//             DiaryUI.Instance.ShowPage(itemData.description);

//             UIMessage.Instance.Hide();

//             Destroy(gameObject);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (!other.CompareTag("Player")) return;

//         playerNear = true;
//         UIMessage.Instance.Show("Pressione F para ler", 999f);
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (!other.CompareTag("Player")) return;

//         playerNear = false;
//         UIMessage.Instance.Hide();
//     }
// }


using UnityEngine;
using UnityEngine.InputSystem;

public class DiaryPage : MonoBehaviour
{
    [SerializeField] private ItemData itemData; // 🔥 AGORA É ITEMDATA

    private bool playerNear;

    void Update()
    {
        if (playerNear && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (itemData == null)
            {
                Debug.LogError("DiaryPage sem ItemData!");
                return;
            }

            // adiciona no inventário
            InventorySystem.Instance.AddItem(itemData);

            // mostra texto
            if (DiaryUI.Instance != null)
                DiaryUI.Instance.ShowPage(itemData.description);
            else
                Debug.LogError("DiaryUI não encontrado!");

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            UIMessage.Instance.Show("Pressione F para ler", 999f);
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