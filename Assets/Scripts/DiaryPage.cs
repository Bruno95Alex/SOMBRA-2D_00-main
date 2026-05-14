using UnityEngine;
using UnityEngine.InputSystem;

public class DiaryPage : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

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

            InventorySystem.Instance.AddItem(itemData);

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
