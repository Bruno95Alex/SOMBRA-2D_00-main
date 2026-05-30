using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Checkpoint que salva o jogo automaticamente quando o player passa.
/// Também permite salvar manualmente pressionando F/Triângulo.
///
/// SETUP:
/// 1. Crie um GameObject na cena
/// 2. Adicione Collider2D (Is Trigger)
/// 3. Adicione este script
/// 4. Opcional: adicione SpriteRenderer com ícone de save
/// </summary>
public class SavePoint : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField] private bool   salvaAutomatico   = true;
    [SerializeField] private bool   permiteManual     = true;
    [SerializeField] private string mensagemSave      = "Jogo salvo!";
    [SerializeField] private int    slot              = 0;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer icone;
    [SerializeField] private Color          corAtivo   = new Color(0.2f, 1f, 0.4f, 1f);
    [SerializeField] private Color          corInativo = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("Partículas (opcional)")]
    [SerializeField] private ParticleSystem particulasSave;

    private bool playerPerto = false;
    private bool foiUsado    = false;

    void Start()
    {
        if (icone != null)
            icone.color = corInativo;
    }

    void Update()
    {
        if (!playerPerto || !permiteManual) return;

        bool salvar = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame;

        if (salvar)
            Salvar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerPerto = true;

        // atualiza checkpoint do player
        PlayerController.Instance?.SetCheckpoint(transform.position);

        if (salvaAutomatico && !foiUsado)
            Salvar();
        else if (permiteManual && !foiUsado)
            UIMessage.Instance?.Show("Pressione F para salvar o jogo", 999f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = false;
        UIMessage.Instance?.Hide();
    }

    void Salvar()
    {
        foiUsado = true;

        SaveSystem.Instance?.Salvar(slot);

        if (icone != null) icone.color = corAtivo;
        if (particulasSave != null) particulasSave.Play();

        UIMessage.Instance?.Show(mensagemSave, 2f);
    }
}
