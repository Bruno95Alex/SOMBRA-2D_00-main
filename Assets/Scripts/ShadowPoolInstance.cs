using UnityEngine;
using System.Collections;

/// <summary>
/// Controla o ciclo de vida de uma poça de sombra individual.
/// Coloque este script no prefab ShadowPool_Prefab.
///
/// HIERARQUIA DO PREFAB:
///   ShadowPool_Prefab
///     ├── SpriteRenderer   (sprite circular escuro)
///     ├── CircleCollider2D (Is Trigger, raio ~0.7)
///     └── ShadowPoolInstance (este script)
/// </summary>
public class ShadowPoolInstance : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private SpriteRenderer   spriteRenderer;
    [SerializeField] private CircleCollider2D col;

    private static readonly Color COR_AVISO  = new Color(0.3f,  0f,  0.5f, 0.55f);
    private static readonly Color COR_PERIGO = new Color(0.05f, 0f,  0.1f, 0.85f);

    // flag que impede dano após desativação
    private bool desativada = false;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (col            == null) col            = GetComponent<CircleCollider2D>();

        // collider começa desativado — só liga na fase de perigo
        if (col != null) col.enabled = false;

        SetAlpha(0f);
    }

    public void Iniciar(float duracaoAviso, float duracaoPerigo,
                        float duracaoSumindo, float escalaInicial, float escalaFinal)
    {
        StartCoroutine(CicloDeVida(duracaoAviso, duracaoPerigo,
                                   duracaoSumindo, escalaInicial, escalaFinal));
    }

    /// <summary>
    /// Desativa imediatamente o collider e inicia o fade out.
    /// Chamado pelo ShadowPoolSpawner ao limpar as poças.
    /// </summary>
    public void DesativarImediatamente()
    {
        desativada = true;

        // desativa collider ANTES de qualquer outra coisa
        if (col != null) col.enabled = false;

        StopAllCoroutines();
        StartCoroutine(FadeOutEDestruir(0.3f));
    }

    IEnumerator CicloDeVida(float duracaoAviso, float duracaoPerigo,
                             float duracaoSumindo, float escalaInicial, float escalaFinal)
    {
        // ── FASE 1: AVISO — cresce pulsando em roxo ──
        transform.localScale = Vector3.one * escalaInicial;
        SetAlpha(0f);

        float t = 0f;
        while (t < duracaoAviso)
        {
            if (desativada) yield break;

            t += Time.deltaTime;
            float progresso = t / duracaoAviso;

            transform.localScale = Vector3.one * Mathf.Lerp(escalaInicial, escalaFinal, progresso);

            float pulse = 0.5f + 0.5f * Mathf.Sin(t * Mathf.PI * 4f);
            if (spriteRenderer != null)
                spriteRenderer.color = Color.Lerp(
                    new Color(COR_AVISO.r, COR_AVISO.g, COR_AVISO.b, 0.2f),
                    COR_AVISO, pulse);

            yield return null;
        }

        if (desativada) yield break;

        // ── FASE 2: PERIGO — preta, collider ativo ──
        if (col != null) col.enabled = true;
        transform.localScale = Vector3.one * escalaFinal;

        t = 0f;
        while (t < duracaoPerigo)
        {
            if (desativada) yield break;

            t += Time.deltaTime;
            float pulse = 0.85f + 0.15f * Mathf.Sin(t * Mathf.PI * 1.5f);
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(COR_PERIGO.r, COR_PERIGO.g,
                                                  COR_PERIGO.b, COR_PERIGO.a * pulse);
            yield return null;
        }

        if (desativada) yield break;

        // ── FASE 3: SUMINDO — fade out natural ──
        if (col != null) col.enabled = false;

        yield return StartCoroutine(FadeOutEDestruir(duracaoSumindo));
    }

    IEnumerator FadeOutEDestruir(float duracao)
    {
        Color corAtual = spriteRenderer != null ? spriteRenderer.color : Color.black;
        float t = 0f;
        while (t < duracao)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(corAtual.a, 0f, t / duracao);
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(corAtual.r, corAtual.g, corAtual.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // double-check: não mata se já desativada
        if (desativada) return;
        if (!other.CompareTag("Player")) return;
        if (PlayerController.Instance != null)
            PlayerController.Instance.Die();
    }

    void SetAlpha(float a)
    {
        if (spriteRenderer == null) return;
        Color c = spriteRenderer.color;
        c.a = a;
        spriteRenderer.color = c;
    }
}
