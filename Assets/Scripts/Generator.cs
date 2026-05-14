using UnityEngine;
using UnityEngine.InputSystem;

public class Generator : MonoBehaviour
{
    [SerializeField] private ItemData keyItem;
    [SerializeField] private ItemData batteryItem;

    private bool playerNear;
    private bool activated;

    void Update()
    {
        if (!playerNear || activated) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            bool hasKey = InventorySystem.Instance.HasItem(keyItem);
            bool hasBattery = InventorySystem.Instance.HasItem(batteryItem);

            if (hasKey && hasBattery)
            {
                InventorySystem.Instance.RemoveItem(keyItem);
                InventorySystem.Instance.RemoveItem(batteryItem);

                activated = true;

                UIMessage.Instance.Show("Gerador ativado! Corra para a saída!", 3f);

                Debug.Log("GERADOR LIGADO");

                // TODO: acionar condição de vitória aqui (ex: VictoryManager.Instance.Win())
            }
            else if (!hasKey && !hasBattery)
            {
                UIMessage.Instance.Show("Você precisa da chave e da bateria", 2f);
            }
            else if (!hasKey)
            {
                UIMessage.Instance.Show("Você precisa da chave do gerador", 2f);
            }
            else
            {
                UIMessage.Instance.Show("Você precisa da bateria", 2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            playerNear = true;
            UIMessage.Instance.Show("Pressione F para ligar o gerador", 999f);
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
