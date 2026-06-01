using UnityEngine;

public class DiaryPage : MonoBehaviour
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
        if (!playerNear) return;

        bool interact = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame;

        if (!interact) return;

        if (itemData == null) { Debug.LogError("DiaryPage sem ItemData!"); return; }

        if (SaveSystem.Instance != null)
            SaveSystem.Instance.GetDados().coletaveisColetados.Add(gameObject.name);

        InventorySystem.Instance.AddItem(itemData);

        if (DiaryUI.Instance != null)
            DiaryUI.Instance.ShowPage(itemData.description);

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
