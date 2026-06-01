using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private bool playerPerto = false;

    void Start()
    {
        // verifica se este item já foi coletado no save atual
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

        if (playerPerto && interact)
            Coletar();
    }

    void Coletar()
    {
        // registra no save que este item foi coletado
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.GetDados().coletaveisColetados.Add(gameObject.name);

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
