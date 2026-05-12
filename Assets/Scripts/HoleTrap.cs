using UnityEngine;
using System.Collections;

public class HoleTrap : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D killZone;

    [Header("Configuração")]
    [SerializeField] private float tempoParaAbrir = 1.2f; // tempo até matar

    private bool ativado = false;

    private void Awake()
    {
        if (killZone != null)
            killZone.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ativado) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(AbrirBuraco());
        }
    }

    IEnumerator AbrirBuraco()
    {
        ativado = true;

        // 🔥 inicia animação
        if (animator != null)
            animator.SetTrigger("Open");

        // ⏳ tempo para jogador reagir
        yield return new WaitForSeconds(tempoParaAbrir);

        // 💀 ativa zona de morte
        if (killZone != null)
            killZone.enabled = true;
    }
}