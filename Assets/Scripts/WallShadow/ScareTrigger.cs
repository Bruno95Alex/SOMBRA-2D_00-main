using UnityEngine;

/// <summary>
/// Coloque em qualquer lugar da cena para disparar um susto
/// quando o player entrar na área.
///
/// SETUP:
/// 1. Crie um GameObject vazio
/// 2. Adicione Collider2D (Is Trigger)
/// 3. Adicione este script
/// 4. Configure o tipo de susto e se é único ou repetível
/// </summary>
public class ScareTrigger : MonoBehaviour
{
    [Header("Susto")]
    [SerializeField] private ScareType tipo      = ScareType.Full;
    [SerializeField] private bool      umaVez    = true;  // dispara só uma vez
    [SerializeField] private float     delay     = 0f;    // delay antes do susto

    [Header("Sombras na parede (opcional)")]
    [SerializeField] private WallShadow[] sombrasDaArea; // sombras que aparecem junto

    private bool disparado = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (umaVez && disparado) return;

        disparado = true;

        if (delay > 0f)
            Invoke(nameof(Disparar), delay);
        else
            Disparar();
    }

    void Disparar()
    {
        if (ScareManager.Instance != null)
            ScareManager.Instance.TriggerScare(tipo);

        // ativa sombras da área instantaneamente
        if (sombrasDaArea != null)
            foreach (var s in sombrasDaArea)
                if (s != null) s.AparecerInstantaneo();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0.5f, 0.25f);
        var col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawCube(transform.position, col.bounds.size);

        Gizmos.color = new Color(1f, 0f, 0.5f, 0.8f);
        Gizmos.DrawWireCube(transform.position,
            col != null ? col.bounds.size : Vector3.one);
    }
}
