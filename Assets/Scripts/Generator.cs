using UnityEngine;
using UnityEngine.InputSystem;

public class Generator : MonoBehaviour
{
    [SerializeField] private ItemData keyItem;
    [SerializeField] private ItemData batteryItem;

    private bool playerNear;

    void Update()
    {
        if (!playerNear) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            bool hasKey = InventorySystem.Instance.HasItem(keyItem);
            bool hasBattery = InventorySystem.Instance.HasItem(batteryItem);

            if (hasKey && hasBattery)
            {
                InventorySystem.Instance.RemoveItem(keyItem);
                InventorySystem.Instance.RemoveItem(batteryItem);

                Debug.Log("GERADOR LIGADO");

                UIMessage.Instance.Show("Gerador ativado!", 2f);

                // 👉 aqui você coloca o final
            }
            else
            {
                UIMessage.Instance.Show("Falta a chave e a bateria", 2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            UIMessage.Instance.Show("Pressione F para ligar", 999f);
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