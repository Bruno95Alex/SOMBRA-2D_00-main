using UnityEngine;

/// <summary>
/// Coloque este script em zonas invisíveis do mapa (BoxCollider2D trigger).
/// Quando o jogador entra na zona, dispara o step do tutorial configurado.
///
/// SETUP:
///  1. Crie um GameObject vazio no mapa (ex: "TutorialZona_Movimento")
///  2. Adicione BoxCollider2D → marque "Is Trigger"
///  3. Adicione este script
///  4. No Inspector, escolha qual TutorialStep disparar
///  5. Posicione a zona no local correto do mapa (veja sugestões abaixo)
///
/// SUGESTÕES DE POSICIONAMENTO:
///  • Movimento   → logo na posição inicial do jogador (dispara imediatamente)
///  • Lanterna    → na entrada do primeiro corredor escuro
///  • ColetarItem → perto da chave ou bateria (antes do item em si)
///  • Interagir   → perto do Vigia ou da primeira porta
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TutorialTriggerZone : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Qual dica do tutorial esta zona vai disparar?")]
    [SerializeField] private TutorialManager.TutorialStep step;

    [Tooltip("Se verdadeiro, destroi o objeto após disparar (evita re-trigger).")]
    [SerializeField] private bool destruirAposDisparar = true;

    void Start()
    {
        // garante que o collider é trigger
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (TutorialManager.Instance == null) return;

        TutorialManager.Instance.MostrarStep(step);

        if (destruirAposDisparar)
            Destroy(gameObject);
    }

#if UNITY_EDITOR
    // visualiza a zona no editor como um retângulo azul semitransparente
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.25f);
        var col = GetComponent<BoxCollider2D>();
        if (col != null)
            Gizmos.DrawCube(transform.position + (Vector3)col.offset,
                            col.size);

        Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.8f);
        if (col != null)
            Gizmos.DrawWireCube(transform.position + (Vector3)col.offset,
                                col.size);
    }

    void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = new Color(0.2f, 0.5f, 1f, 1f);
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.6f,
            $"Tutorial: {step}");
    }
#endif
}
