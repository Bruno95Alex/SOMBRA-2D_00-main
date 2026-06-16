using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controla o Shadow Caster 2D da porta:
///   - Porta fechada → Shadow Caster ATIVO   → luz bloqueada
///   - Porta aberta  → Shadow Caster INATIVO → luz passa
///
/// SETUP:
///   1. Adicione este script no mesmo GameObject da porta
///   2. Certifique-se que a porta tem um Shadow Caster 2D
///   3. O script detecta automaticamente o estado do Animator
///
/// HIERARQUIA SUGERIDA:
///   Porta (Animator + DoorLightBlocker)
///     ├── SpriteRenderer
///     ├── Collider2D
///     └── Shadow Caster 2D   ← bloqueador de luz
/// </summary>
[RequireComponent(typeof(Animator))]
public class DoorLightBlocker : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Deixe vazio — detecta automaticamente no GameObject.")]
    [SerializeField] private ShadowCaster2D shadowCaster;

    [Header("Configuração")]
    [Tooltip("Nome exato do estado de abertura no Animator.")]
    [SerializeField] private string estadoAberta = "Open";

    [Tooltip("Se verdadeiro, também desativa o Collider2D ao abrir.")]
    [SerializeField] private bool desativarColliderAoAbrir = false;

    private Animator     animator;
    private Collider2D   col;
    private bool         estaAberta = false;

    // ══════════════════════════════════════════════
    void Awake()
    {
        animator = GetComponent<Animator>();

        if (shadowCaster == null)
            shadowCaster = GetComponent<ShadowCaster2D>();

        if (shadowCaster == null)
            Debug.LogWarning($"[DoorLightBlocker] Shadow Caster 2D não encontrado em {gameObject.name}!");

        if (desativarColliderAoAbrir)
            col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (animator == null || shadowCaster == null) return;

        // verifica se o Animator está no estado de porta aberta
        bool abertaAgora = animator.GetCurrentAnimatorStateInfo(0)
                                   .IsName(estadoAberta);

        // só atualiza quando o estado muda
        if (abertaAgora == estaAberta) return;

        estaAberta = abertaAgora;
        AtualizarLuz();
    }

    void AtualizarLuz()
    {
        // porta aberta → luz passa (shadow caster desativado)
        // porta fechada → luz bloqueada (shadow caster ativado)
        shadowCaster.enabled = !estaAberta;

        if (desativarColliderAoAbrir && col != null)
            col.enabled = !estaAberta;

        Debug.Log($"[DoorLightBlocker] {gameObject.name} — " +
                  $"Sombra: {(shadowCaster.enabled ? "ATIVA" : "INATIVA")} " +
                  $"| Porta: {(estaAberta ? "ABERTA" : "FECHADA")}");
    }

    // ── API pública para chamar por código se precisar ──
    public void AbrirLuz()  { shadowCaster.enabled = false; estaAberta = true; }
    public void FecharLuz() { shadowCaster.enabled = true;  estaAberta = false; }
}
