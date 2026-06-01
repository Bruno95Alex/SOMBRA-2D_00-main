using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private bool playerNear;

    void Start()
    {
        if (SaveSystem.Instance != null &&
            SaveSystem.Instance.GetDados().coletaveisColetados.Contains(gameObject.name))
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        bool interact = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame;

        if (playerNear && interact)
        {
            if (SaveSystem.Instance != null)
                SaveSystem.Instance.GetDados().coletaveisColetados.Add(gameObject.name);

            InventorySystem.Instance.AddItem(itemData);
            UIMessage.Instance.Hide();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            UIMessage.Instance.Show("Pressione F / botão para pegar", 999f);
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
