using UnityEngine;
using System.Collections;

/// <summary>
/// Mecânica da mão da sombra que sai da parede.
/// 
/// HIERARQUIA SUGERIDA:
/// ShadowHandTrap (este script + Collider2D trigger de detecção)
///   ├── Hand (SpriteRenderer + Animator — a mão que anima saindo)
///   └── DangerZone (Collider2D trigger — área que mata o player)
///
/// SETUP:
/// 1. Crie um GameObject "ShadowHandTrap" na parede
/// 2. Adicione um Collider2D grande (Is Trigger) — zona de detecção do player
/// 3. Crie filho "Hand" com SpriteRenderer e Animator
/// 4. Crie filho "DangerZone" com Collider2D pequeno (Is Trigger) — área da mão
/// 5. Configure o Animator com estados: Idle, Grab, Retract
/// </summary>
public class ShadowHandTrap : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Animator     handAnimator;   // animator da mão
    [SerializeField] private Collider2D   dangerZone;     // área que mata o player
    [SerializeField] private Transform    handTransform;  // posição da mão

    [Header("Comportamento")]
    [SerializeField] private float delayAntesDeGarrar  = 1.2f; // tempo após detectar player
    [SerializeField] private float duracaoGarrada      = 0.8f; // tempo com a mão estendida
    [SerializeField] private float duracaoRecolher     = 0.6f; // tempo recolhendo
    [SerializeField] private float cooldownEntreGarras = 2.5f; // tempo até próxima tentativa

    [Header("Aviso visual (opcional)")]
    [SerializeField] private GameObject warningIndicator; // seta ou efeito de aviso
    [SerializeField] private float      duracaoAviso = 0.8f;

    [Header("Sons")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   warnClip;   // som de aviso (raspar na parede)
    [SerializeField] private AudioClip   grabClip;   // som de pegar
    [SerializeField] private AudioClip   missClip;   // som de errar

    [Header("Câmera shake (opcional)")]
    [SerializeField] private bool shakeCameraOnGrab = true;

    // ================================
    // ESTADO
    // ================================

    private enum HandState { Waiting, Warning, Grabbing, Retracting }
    private HandState state = HandState.Waiting;

    private bool playerNaZona = false;
    private bool emCooldown   = false;

    void Start()
    {
        if (dangerZone   != null) dangerZone.enabled   = false;
        if (warningIndicator != null) warningIndicator.SetActive(false);
    }

    // ================================
    // DETECÇÃO DO PLAYER — zona grande
    // ================================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNaZona = true;

        if (state == HandState.Waiting && !emCooldown)
            StartCoroutine(SequenciaGarra());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNaZona = false;
    }

    // ================================
    // SEQUÊNCIA DA GARRA
    // ================================

    IEnumerator SequenciaGarra()
    {
        // FASE 1 — aviso: raspa na parede antes de sair
        state = HandState.Warning;

        if (warningIndicator != null) warningIndicator.SetActive(true);
        PlaySound(warnClip);

        if (handAnimator != null) handAnimator.SetTrigger("Warn");

        yield return new WaitForSeconds(duracaoAviso);

        if (warningIndicator != null) warningIndicator.SetActive(false);

        // FASE 2 — espera configurada antes de garrar
        yield return new WaitForSeconds(delayAntesDeGarrar - duracaoAviso);

        // FASE 3 — GARRA
        state = HandState.Grabbing;

        if (dangerZone   != null) dangerZone.enabled = true;
        if (handAnimator != null) handAnimator.SetTrigger("Grab");

        PlaySound(grabClip);

        yield return new WaitForSeconds(duracaoGarrada);

        // FASE 4 — recolhe
        state = HandState.Retracting;

        if (dangerZone   != null) dangerZone.enabled = false;
        if (handAnimator != null) handAnimator.SetTrigger("Retract");

        PlaySound(missClip);

        yield return new WaitForSeconds(duracaoRecolher);

        // FASE 5 — cooldown e verifica se player ainda está na zona
        state     = HandState.Waiting;
        emCooldown = true;

        yield return new WaitForSeconds(cooldownEntreGarras);

        emCooldown = false;

        // se player ainda estiver na zona, tenta de novo
        if (playerNaZona)
            StartCoroutine(SequenciaGarra());
    }

    // ================================
    // COLISÃO DA MÃO COM O PLAYER
    // ================================

    /// <summary>
    /// Coloque este método no DangerZone como um script separado
    /// ou use o ShadowHandDanger abaixo.
    /// </summary>
    public void OnPlayerCaught()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.Die();
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // utilitário para debug
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        var col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawCube(transform.position, col.bounds.size);
    }
}
