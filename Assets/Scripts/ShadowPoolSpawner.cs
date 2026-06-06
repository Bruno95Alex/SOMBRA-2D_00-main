using UnityEngine;
using System.Collections.Generic;

public class ShadowPoolSpawner : MonoBehaviour
{
    public static ShadowPoolSpawner Instance;

    [Header("Prefab")]
    [SerializeField] private GameObject poolPrefab;

    [Header("Comportamento")]
    [SerializeField] private float intervaloInicial    = 3.5f;
    [SerializeField] private float intervaloMinimo     = 1.2f;
    [SerializeField] private float reducaoIntervalo    = 0.15f;
    [SerializeField] private float duracaoAviso        = 1.2f;
    [SerializeField] private float duracaoPerigo       = 4f;
    [SerializeField] private float duracaoSumindo      = 0.8f;

    [Header("Visual")]
    [SerializeField] private float tamanhoInicial      = 0.4f;
    [SerializeField] private float tamanhoFinal        = 1.8f;
    [SerializeField] private float offsetYSpawn        = 0f;

    [Header("Limite")]
    [SerializeField] private int maxPocasSimultaneas   = 5;

    [Header("Ativar/Desativar (Manual)")]
    [Tooltip("Deixe FALSE — as ShadowPoolZones ativam automaticamente.")]
    [SerializeField] private bool ativo = false;

    private float     intervaloAtual;
    private float     timer;
    private Transform player;
    private List<GameObject> pocasAtivas = new List<GameObject>();
    private int zonasAtivas = 0;

    void Awake() => Instance = this;

    void Start()
    {
        ResetarIntervalo();
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        bool deveRodar = ativo || zonasAtivas > 0;
        if (!deveRodar || player == null) return;
        if (PlayerController.Instance != null && !PlayerController.Instance.IsAlive) return;

        timer += Time.deltaTime;
        if (timer >= intervaloAtual)
        {
            timer = 0f;
            TentarSpawnar();
            intervaloAtual = Mathf.Max(intervaloMinimo, intervaloAtual - reducaoIntervalo);
        }
    }

    // ── chamado pelas zonas ───────────────────────
    public void EntrarNaZona()
    {
        zonasAtivas++;
        if (zonasAtivas == 1)
        {
            timer = 0f;
            Debug.Log("[ShadowPoolSpawner] Zona ativada.");
        }
    }

    public void SairDaZona()
    {
        zonasAtivas = Mathf.Max(0, zonasAtivas - 1);
        if (zonasAtivas == 0)
        {
            // FIX: desativa colliders imediatamente antes de qualquer Destroy
            LimparTodas();
            Debug.Log("[ShadowPoolSpawner] Zona desativada — poças limpas.");
        }
    }

    // ── api manual ────────────────────────────────
    public void Ativar()
    {
        ativo = true;
        ResetarIntervalo();
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    public void Desativar()
    {
        ativo       = false;
        zonasAtivas = 0;
        LimparTodas();
    }

    // ── spawn ─────────────────────────────────────
    void TentarSpawnar()
    {
        pocasAtivas.RemoveAll(p => p == null);
        if (pocasAtivas.Count >= maxPocasSimultaneas) return;
        if (poolPrefab == null)
        {
            Debug.LogError("[ShadowPoolSpawner] poolPrefab não configurado!");
            return;
        }

        Vector3 pos = player.position + new Vector3(0f, offsetYSpawn, 0f);
        pos.z = 0f;

        GameObject poca = Instantiate(poolPrefab, pos, Quaternion.identity);
        pocasAtivas.Add(poca);

        ShadowPoolInstance inst = poca.GetComponent<ShadowPoolInstance>();
        if (inst != null)
            inst.Iniciar(duracaoAviso, duracaoPerigo, duracaoSumindo, tamanhoInicial, tamanhoFinal);
    }

    public void LimparTodas()
    {
        foreach (var p in pocasAtivas)
        {
            if (p == null) continue;

            // FIX: desativa o collider imediatamente para garantir
            // que nenhuma poça cause dano após sair da zona
            ShadowPoolInstance inst = p.GetComponent<ShadowPoolInstance>();
            if (inst != null)
                inst.DesativarImediatamente();
            else
                Destroy(p);
        }
        pocasAtivas.Clear();
    }

    void ResetarIntervalo()
    {
        intervaloAtual = intervaloInicial;
        timer          = intervaloAtual * 0.5f;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0f, 0.4f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
