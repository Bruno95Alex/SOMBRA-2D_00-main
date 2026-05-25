using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private bool playerPerto = false;

    void Update()
    {
        bool interact = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame;

        if (playerPerto && interact)
            Coletar();
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
