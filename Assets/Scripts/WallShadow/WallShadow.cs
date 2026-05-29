using UnityEngine;
using System.Collections;

/// <summary>
/// Sombra que desliza e pulsa pela parede.
/// Coloque num GameObject com SpriteRenderer na parede.
///
/// SETUP:
/// 1. Crie um GameObject filho da parede
/// 2. Adicione SpriteRenderer com sprite de silhueta escura
/// 3. Adicione este script
/// 4. Configure o tipo de movimento no Inspector
/// </summary>
public class WallShadow : MonoBehaviour
{
    public enum ShadowType { Sliding, Pulsing, Both }

    [Header("Tipo")]
    [SerializeField] private ShadowType tipo = ShadowType.Both;

    [Header("Deslizar")]
    [SerializeField] private Vector2 direcao       = Vector2.right;
    [SerializeField] private float   distancia     = 4f;    // quanto desliza
    [SerializeField] private float   velocidade    = 1.5f;
    [SerializeField] private bool    voltaAoInicio = true;  // vai e volta ou loop

    [Header("Pulsar (aparecer e sumir)")]
    [SerializeField] private float alphaMin    = 0f;
    [SerializeField] private float alphaMax    = 0.75f;
    [SerializeField] private float pulseSpeed  = 1.2f;

    [Header("Delay inicial (aleatoriza fase)")]
    [SerializeField] private float delayInicio = 0f;

    [Header("Ativar só perto do player")]
    [SerializeField] private bool  apenasProximoDoPlayer = true;
    [SerializeField] private float raioAtivacao          = 8f;

    private SpriteRenderer sr;
    private Vector3 posOriginal;
    private float   phase;
    private bool    ativo = false;

    void Start()
    {
        sr          = GetComponent<SpriteRenderer>();
        posOriginal = transform.localPosition;
        phase       = Random.Range(0f, Mathf.PI * 2f);

        if (sr != null)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);

        StartCoroutine(IniciarComDelay());
    }

    IEnumerator IniciarComDelay()
    {
        yield return new WaitForSeconds(delayInicio > 0 ? delayInicio : Random.Range(0f, 2f));
        ativo = true;
    }

    void Update()
    {
        if (!ativo || sr == null) return;

        // verifica distância do player
        if (apenasProximoDoPlayer && PlayerController.Instance != null)
        {
            float dist = Vector2.Distance(transform.position,
                         PlayerController.Instance.transform.position);
            if (dist > raioAtivacao) { FadeAlpha(0f); return; }
        }

        float t = Time.time * pulseSpeed + phase;

        // PULSAR
        if (tipo == ShadowType.Pulsing || tipo == ShadowType.Both)
        {
            float alpha = Mathf.Lerp(alphaMin, alphaMax,
                          (Mathf.Sin(t) + 1f) * 0.5f);
            FadeAlpha(alpha);
        }

        // DESLIZAR
        if (tipo == ShadowType.Sliding || tipo == ShadowType.Both)
        {
            float slide;
            if (voltaAoInicio)
                slide = Mathf.Sin(t * velocidade * 0.5f) * distancia;
            else
                slide = Mathf.PingPong(Time.time * velocidade, distancia) - distancia * 0.5f;

            transform.localPosition = posOriginal +
                (Vector3)(direcao.normalized * slide);
        }
    }

    void FadeAlpha(float target)
    {
        Color c = sr.color;
        c.a     = Mathf.MoveTowards(c.a, target, Time.deltaTime * 3f);
        sr.color = c;
    }

    // chamado pelo ScareManager para susto instantâneo
    public void AparecerInstantaneo()
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a      = alphaMax;
        sr.color = c;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, raioAtivacao);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position,
                        transform.position + (Vector3)direcao.normalized * distancia);
    }
}
