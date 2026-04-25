using UnityEngine;
using UnityEngine.InputSystem;

public class GavetaInterativa : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject itemDentro;

    private bool playerNear;
    private bool opened = false;

    void Start()
    {
        if (itemDentro != null)
            itemDentro.SetActive(false);
    }

    void Update()
    {
        if (!playerNear || opened) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            opened = true;

            UIMessage.Instance.Show("Abrindo gaveta...", 1.5f);

            animator.SetTrigger("Open");

            // 🔥 delay pra combinar com animação
            Invoke(nameof(MostrarItem), 0.5f);
        }
    }

    void MostrarItem()
    {
        if (itemDentro != null)
            itemDentro.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return;

        if (other.CompareTag("Player"))
        {
            playerNear = true;
            UIMessage.Instance.Show("Pressione F para abrir gaveta", 999f);
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