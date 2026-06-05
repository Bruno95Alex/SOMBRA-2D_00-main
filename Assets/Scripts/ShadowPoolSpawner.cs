using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawna poças de sombra na posição do player periodicamente.
/// Coloque um GameObject "ShadowPoolSpawner" na cena da Fase 3.
///
/// SETUP:
///   1. Crie um GameObject vazio chamado "ShadowPoolSpawner"
///   2. Adicione este script
///   3. Crie o prefab ShadowPool_Prefab e arraste no campo "Pool Prefab"
///   4. Deixe "Ativo" desmarcado — chame Ativar() quando o jogador entrar na sala
///   5. Chame Desativar() quando o gerador for ligado
/// </summary>
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

    [Header("Ativar/Desativar")]
    [SerializeField] private bool ativo                = false;

    private float intervaloAtual;
    private float timer;
    private Transform player;
    private List<GameObject> pocasAtivas = new List<GameObject>();

    void Awake() => Instance = this;

    void Start()
    {
        intervaloAtual = intervaloInicial;
        timer          = intervaloAtual * 0.5f;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (!ativo || player == null) return;

        if (PlayerController.Instance != null && !PlayerController.Instance.IsAlive) return;

        timer += Time.deltaTime;

        if (timer >= intervaloAtual)
        {
            timer = 0f;
            TentarSpawnar();
            intervaloAtual = Mathf.Max(intervaloMinimo, intervaloAtual - reducaoIntervalo);
        }
    }

    void TentarSpawnar()
    {
        pocasAtivas.RemoveAll(p => p == null);

        if (pocasAtivas.Count >= maxPocasSimultaneas) return;

        if (poolPrefab == null)
        {
            Debug.LogError("[ShadowPoolSpawner] poolPrefab não configurado!");
            return;
        }

        Vector3 spawnPos  = player.position + new Vector3(0f, offsetYSpawn, 0f);
        spawnPos.z        = 0f;

        GameObject poca   = Instantiate(poolPrefab, spawnPos, Quaternion.identity);
        pocasAtivas.Add(poca);

        ShadowPoolInstance inst = poca.GetComponent<ShadowPoolInstance>();
        if (inst != null)
            inst.Iniciar(duracaoAviso, duracaoPerigo, duracaoSumindo,
                         tamanhoInicial, tamanhoFinal);
    }

    public void Ativar()
    {
        ativo          = true;
        intervaloAtual = intervaloInicial;
        timer          = intervaloAtual * 0.5f;

        var playerObj  = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    public void Desativar()
    {
        ativo = false;
        LimparTodas();
    }

    public void LimparTodas()
    {
        foreach (var p in pocasAtivas)
            if (p != null) Destroy(p);
        pocasAtivas.Clear();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0f, 0.4f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
