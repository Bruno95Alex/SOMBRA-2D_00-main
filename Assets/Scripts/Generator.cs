using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Mecânica do gerador em 3 fases:
///   Fase 1 — sem itens: mostra o que falta
///   Fase 2 — itens instalados: pede para ligar
///   Fase 3 — ligado: executa animação e aciona vitória
/// </summary>
public class Generator : MonoBehaviour
{
    // ================================
    // INSPECTOR
    // ================================

    [Header("Itens necessários")]
    [SerializeField] private ItemData keyItem;
    [SerializeField] private ItemData batteryItem;

    [Header("Animação")]
    [SerializeField] private Animator animator; // trigger "TurnOn"

    [Header("Luzes")]
    [SerializeField] private GameObject lightOff; // visual apagado
    [SerializeField] private GameObject lightOn;  // visual ligado

    [Header("Sons")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   installClip;  // som de instalar item
    [SerializeField] private AudioClip   startupClip;  // som de ligar o gerador

    [Header("Partículas (opcional)")]
    [SerializeField] private ParticleSystem sparks;

    [Header("Vitória")]
    [SerializeField] private float delayVitoria = 2f; // tempo após ligar para acionar vitória

    // ================================
    // ESTADO
    // ================================

    private enum GeneratorState { Broken, Ready, On }
    private GeneratorState state = GeneratorState.Broken;

    private bool keyInstalled      = false;
    private bool batteryInstalled  = false;
    private bool playerNear        = false;

    // ================================
    // AWAKE
    // ================================

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();

        if (lightOn  != null) lightOn.SetActive(false);
        if (lightOff != null) lightOff.SetActive(true);
    }

    // ================================
    // UPDATE
    // ================================

    void Update()
    {
        if (!playerNear || state == GeneratorState.On) return;

        bool interact = InputReader.Instance != null
            ? InputReader.Instance.InteractPressed
            : Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame;

        if (!interact) return;

        switch (state)
        {
            case GeneratorState.Broken: TentarInstalarItens(); break;
            case GeneratorState.Ready:  Ligar();               break;
        }
    }

    // ================================
    // FASE 1 — instalar itens
    // ================================

    void TentarInstalarItens()
    {
        bool temChave   = InventorySystem.Instance.HasItem(keyItem);
        bool temBateria = InventorySystem.Instance.HasItem(batteryItem);

        bool instalouAlgo = false;

        // instala chave se tiver e ainda não instalou
        if (!keyInstalled && temChave)
        {
            keyInstalled = true;
            InventorySystem.Instance.RemoveItem(keyItem);
            UIMessage.Instance.Show("Chave do gerador instalada!", 2f);
            PlaySound(installClip);
            instalouAlgo = true;
        }

        // instala bateria se tiver e ainda não instalou
        if (!batteryInstalled && temBateria)
        {
            batteryInstalled = true;
            InventorySystem.Instance.RemoveItem(batteryItem);
            UIMessage.Instance.Show("Bateria instalada!", 2f);
            PlaySound(installClip);
            instalouAlgo = true;
        }

        // verifica se ambos foram instalados
        if (keyInstalled && batteryInstalled)
        {
            state = GeneratorState.Ready;
            UIMessage.Instance.Show("Gerador pronto! Pressione F para ligar.", 3f);
            return;
        }

        // mostra o que ainda falta
        if (!instalouAlgo)
        {
            if (!keyInstalled && !batteryInstalled)
                UIMessage.Instance.Show("Você precisa da chave e da bateria.", 2f);
            else if (!keyInstalled)
                UIMessage.Instance.Show("Ainda falta a chave do gerador.", 2f);
            else
                UIMessage.Instance.Show("Ainda falta a bateria.", 2f);
        }
    }

    // ================================
    // FASE 2 — ligar
    // ================================

    void Ligar()
    {
        state = GeneratorState.On;

        if (animator != null)
            animator.SetTrigger("TurnOn");

        if (sparks != null)
            sparks.Play();

        PlaySound(startupClip);

        UIMessage.Instance.Hide();

        StartCoroutine(RotinaDeLigar());
    }

    IEnumerator RotinaDeLigar()
    {
        // pequeno delay para o som e animação começarem
        yield return new WaitForSeconds(0.5f);

        // troca visual apagado → ligado
        if (lightOff != null) lightOff.SetActive(false);
        if (lightOn  != null) lightOn.SetActive(true);

        UIMessage.Instance.Show("⚡ Energia restaurada! Corra para a saída!", 3f);

        yield return new WaitForSeconds(delayVitoria);

        // TODO: acionar condição de vitória
        // VictoryManager.Instance.Win();
        Debug.Log("GERADOR LIGADO — acionar vitória aqui");
    }

    // ================================
    // TRIGGER PLAYER
    // ================================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || state == GeneratorState.On) return;

        playerNear = true;
        AtualizarMensagemProximidade();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = false;
        UIMessage.Instance.Hide();
    }

    void AtualizarMensagemProximidade()
    {
        switch (state)
        {
            case GeneratorState.Broken:
                if (!keyInstalled && !batteryInstalled)
                    UIMessage.Instance.Show("Gerador quebrado. Precisa de itens.", 999f);
                else if (!keyInstalled)
                    UIMessage.Instance.Show("Gerador incompleto. Falta a chave.", 999f);
                else
                    UIMessage.Instance.Show("Gerador incompleto. Falta a bateria.", 999f);
                break;

            case GeneratorState.Ready:
                UIMessage.Instance.Show("Pressione F para ligar o gerador!", 999f);
                break;
        }
    }

    // ================================
    // UTILITÁRIOS
    // ================================

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    public bool IsOn => state == GeneratorState.On;
}
