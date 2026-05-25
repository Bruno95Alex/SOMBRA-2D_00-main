using UnityEngine;

public class DiaryPage : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private bool playerNear;

    void Update()
    {
        if (!playerNear) return;

        // teclado F OU Triângulo via InputReader
        bool interact = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame;

        if (!interact) return;

        if (itemData == null) { Debug.LogError("DiaryPage sem ItemData!"); return; }

        InventorySystem.Instance.AddItem(itemData);

        if (DiaryUI.Instance != null)
            DiaryUI.Instance.ShowPage(itemData.description);
        else
            Debug.LogError("DiaryUI não encontrado!");

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = true;
        UIMessage.Instance.Show("Pressione F / botão para ler", 999f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = false;
        UIMessage.Instance.Hide();
    }
}
