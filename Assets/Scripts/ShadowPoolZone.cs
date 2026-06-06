using UnityEngine;

/// <summary>
/// Define uma área do mapa onde a mecânica de Poça de Sombra fica ativa.
/// Quando o player entra na zona → mecânica liga.
/// Quando o player sai → mecânica desliga e poças somem.
///
/// SETUP (faça isso para cada área que quiser):
///   1. Crie um GameObject vazio (ex: "ZonaSombra_Gerador")
///   2. Adicione BoxCollider2D → marque "Is Trigger"
///   3. Adicione este script
///   4. Redimensione o BoxCollider2D para cobrir a área desejada
///
/// DICA: No editor as zonas aparecem como retângulos roxos.
/// Você pode ter quantas zonas quiser no mapa — cada uma é independente.
/// Se o player estiver em duas zonas ao mesmo tempo, a mecânica continua
/// ativa até sair de ambas.
///
/// EXEMPLOS DE USO:
///   • Sala do Gerador inteira → uma zona cobrindo toda a sala
///   • Corredor específico → zona no corredor
///   • Área após um puzzle → zona só depois da porta
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ShadowPoolZone : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Nome desta zona — aparece no console para facilitar o debug.")]
    [SerializeField] private string nomeZona = "Zona Sombra";

    [Tooltip("Se falso, esta zona nunca ativa a mecânica (útil para desativar temporariamente).")]
    [SerializeField] private bool zonaAtiva = true;

    [Tooltip("Mostra o contorno roxo no editor mesmo quando não selecionado.")]
    [SerializeField] private bool mostrarGizmoSempre = true;

    private bool playerDentro = false;

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!zonaAtiva)           return;
        if (playerDentro)         return;
        if (!other.CompareTag("Player")) return;
        if (ShadowPoolSpawner.Instance == null)
        {
            Debug.LogWarning("[ShadowPoolZone] ShadowPoolSpawner não encontrado na cena!");
            return;
        }

        playerDentro = true;
        ShadowPoolSpawner.Instance.EntrarNaZona();
        Debug.Log($"[ShadowPoolZone] Player entrou em: {nomeZona}");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!playerDentro)              return;
        if (!other.CompareTag("Player")) return;

        playerDentro = false;

        if (ShadowPoolSpawner.Instance != null)
            ShadowPoolSpawner.Instance.SairDaZona();

        Debug.Log($"[ShadowPoolZone] Player saiu de: {nomeZona}");
    }

    /// <summary>
    /// Desativa esta zona em tempo real (ex: ao ligar o gerador).
    /// Se o player estiver dentro, avisa o spawner que saiu.
    /// </summary>
    public void DesativarZona()
    {
        if (!zonaAtiva) return;
        zonaAtiva = false;

        if (playerDentro && ShadowPoolSpawner.Instance != null)
        {
            playerDentro = false;
            ShadowPoolSpawner.Instance.SairDaZona();
        }
    }

    public void AtivarZona()  => zonaAtiva = true;
    public bool PlayerDentro  => playerDentro;

    // ── gizmos no editor ─────────────────────────
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!mostrarGizmoSempre) return;
        DesenharGizmo(0.18f);
    }

    void OnDrawGizmosSelected() => DesenharGizmo(0.45f);

    void DesenharGizmo(float alpha)
    {
        Color cor = zonaAtiva
            ? new Color(0.4f, 0f, 0.7f, alpha)
            : new Color(0.4f, 0.4f, 0.4f, alpha * 0.5f);

        var col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        Vector3 center = transform.position + (Vector3)col.offset;
        Vector3 size   = new Vector3(col.size.x * transform.lossyScale.x,
                                     col.size.y * transform.lossyScale.y, 0.1f);

        Gizmos.color = cor;
        Gizmos.DrawCube(center, size);

        Gizmos.color = new Color(cor.r, cor.g, cor.b, 1f);
        Gizmos.DrawWireCube(center, size);

        UnityEditor.Handles.color = new Color(1f, 1f, 1f, 0.9f);
        UnityEditor.Handles.Label(center + Vector3.up * (size.y * 0.5f + 0.3f),
            zonaAtiva ? $"☁ {nomeZona}" : $"✗ {nomeZona} (inativa)");
    }
#endif
}
