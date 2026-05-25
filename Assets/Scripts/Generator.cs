using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private ItemData keyItem;
    [SerializeField] private ItemData batteryItem;

    private bool playerNear;
    private bool activated;

    void Update()
    {
        if (!playerNear || activated) return;

        bool interact = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame;

        if (!interact) return;

        bool hasKey     = InventorySystem.Instance.HasItem(keyItem);
        bool hasBattery = InventorySystem.Instance.HasItem(batteryItem);

        if (hasKey && hasBattery)
        {
            InventorySystem.Instance.RemoveItem(keyItem);
            InventorySystem.Instance.RemoveItem(batteryItem);
            activated = true;
            UIMessage.Instance.Show("Gerador ativado! Corra para a saída!", 3f);
            Debug.Log("GERADOR LIGADO");
        }
        else if (!hasKey && !hasBattery)
            UIMessage.Instance.Show("Você precisa da chave e da bateria", 2f);
        else if (!hasKey)
            UIMessage.Instance.Show("Você precisa da chave do gerador", 2f);
        else
            UIMessage.Instance.Show("Você precisa da bateria", 2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated || !other.CompareTag("Player")) return;
        playerNear = true;
        UIMessage.Instance.Show("Pressione F / botão para ligar o gerador", 999f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = false;
        UIMessage.Instance.Hide();
    }
}
