using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Generator : MonoBehaviour
{
    [Header("Itens necessários")]
    [SerializeField] private ItemData keyItem;
    [SerializeField] private ItemData batteryItem;

    [Header("Animação")]
    [SerializeField] private Animator animator;

    [Header("Luzes")]
    [SerializeField] private GameObject lightOff;
    [SerializeField] private GameObject lightOn;

    [Header("Iluminação Global")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float lightIntensidadeNoite  = 0.04f; // valor atual da noite
    [SerializeField] private float lightIntensidadeDia    = 1.0f;  // valor com luzes acesas
    [SerializeField] private float lightTransicaoSpeed    = 0.8f;  // velocidade da transição

    [Header("Sons")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   installClip;
    [SerializeField] private AudioClip   startupClip;

    [Header("Partículas (opcional)")]
    [SerializeField] private ParticleSystem sparks;

    [Header("Vitória")]
    [SerializeField] private float delayVitoria = 2f;

    // ================================
    // ESTADO
    // ================================

    private enum GeneratorState { Broken, Ready, On }
    private GeneratorState state = GeneratorState.Broken;

    private bool keyInstalled     = false;
    private bool batteryInstalled = false;
    private bool playerNear       = false;

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

        if (!keyInstalled && temChave)
        {
            keyInstalled = true;
            InventorySystem.Instance.RemoveItem(keyItem);
            PlaySound(installClip);
            instalouAlgo = true;

            if (!batteryInstalled)
                UIMessage.Instance.Show("✓ Chave instalada! Agora coloque a bateria.", 3f);
        }

        if (!batteryInstalled && temBateria)
        {
            batteryInstalled = true;
            InventorySystem.Instance.RemoveItem(batteryItem);
            PlaySound(installClip);
            instalouAlgo = true;

            if (!keyInstalled)
                UIMessage.Instance.Show("✓ Bateria instalada! Agora encontre a chave.", 3f);
        }

        if (keyInstalled && batteryInstalled)
        {
            state = GeneratorState.Ready;
            StartCoroutine(MensagemPronto());
            return;
        }

        if (!instalouAlgo)
        {
            if (!keyInstalled && !batteryInstalled)
                UIMessage.Instance.Show("O gerador precisa de 2 peças:\n[ ] Chave do gerador\n[ ] Bateria", 3f);
            else if (!keyInstalled)
                UIMessage.Instance.Show("Falta ainda:\n[ ] Chave do gerador\n✓  Bateria instalada", 3f);
            else
                UIMessage.Instance.Show("Falta ainda:\n✓  Chave instalada\n[ ] Bateria", 3f);
        }
    }

    IEnumerator MensagemPronto()
    {
        UIMessage.Instance.Show("✓ Chave instalada!\n✓ Bateria instalada!", 2f);
        yield return new WaitForSeconds(2f);
        UIMessage.Instance.Show("Gerador pronto! Pressione F para ligar.", 999f);
    }

    // ================================
    // FASE 2 — ligar
    // ================================

    void Ligar()
    {
        state = GeneratorState.On;

        if (animator != null) animator.SetTrigger("TurnOn");
        if (sparks   != null) sparks.Play();

        PlaySound(startupClip);
        UIMessage.Instance.Hide();

        StartCoroutine(RotinaDeLigar());
    }

    IEnumerator RotinaDeLigar()
    {
        UIMessage.Instance.Show("Ligando o gerador...", 1.5f);

        yield return new WaitForSeconds(0.8f);

        if (lightOff != null) lightOff.SetActive(false);
        if (lightOn  != null) lightOn.SetActive(true);

        // iluminação acende gradualmente — simula luzes ligando
        yield return StartCoroutine(AcenderLuzes());

        UIMessage.Instance.Show("⚡ Energia restaurada! Corra para a saída!", 4f);

        yield return new WaitForSeconds(delayVitoria);

        // TODO: VictoryManager.Instance.Win();
        Debug.Log("GERADOR LIGADO — acionar vitória aqui");
    }

    IEnumerator AcenderLuzes()
    {
        if (globalLight == null) yield break;

        float intensidadeAtual = globalLight.intensity;
        float t = 0f;

        // pisca 2x antes de acender completamente — simula luzes ligando
        globalLight.intensity = lightIntensidadeDia * 0.3f;
        yield return new WaitForSeconds(0.1f);
        globalLight.intensity = intensidadeAtual;
        yield return new WaitForSeconds(0.1f);
        globalLight.intensity = lightIntensidadeDia * 0.5f;
        yield return new WaitForSeconds(0.15f);
        globalLight.intensity = intensidadeAtual;
        yield return new WaitForSeconds(0.15f);

        // acende suavemente até o valor final
        while (t < 1f)
        {
            t += Time.deltaTime * lightTransicaoSpeed;
            globalLight.intensity = Mathf.Lerp(intensidadeAtual, lightIntensidadeDia, t);
            yield return null;
        }

        globalLight.intensity = lightIntensidadeDia;
    }

    // ================================
    // TRIGGER PLAYER
    // ================================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || state == GeneratorState.On) return;
        playerNear = true;
        MostrarMensagemProximidade();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = false;
        UIMessage.Instance.Hide();
    }

    void MostrarMensagemProximidade()
    {
        switch (state)
        {
            case GeneratorState.Broken:
                if (!keyInstalled && !batteryInstalled)
                    UIMessage.Instance.Show("Gerador sem energia.\nPressione F para inspecionar.", 999f);
                else if (!keyInstalled)
                    UIMessage.Instance.Show("✓ Bateria instalada.\n[ ] Falta a chave — Pressione F.", 999f);
                else
                    UIMessage.Instance.Show("✓ Chave instalada.\n[ ] Falta a bateria — Pressione F.", 999f);
                break;

            case GeneratorState.Ready:
                UIMessage.Instance.Show("✓ Tudo pronto!\nPressione F para ligar o gerador.", 999f);
                break;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    public bool  IsOn                => state == GeneratorState.On;
    public float LightIntensidadeDia => lightIntensidadeDia;
    public bool  ChaveInstalada      => keyInstalled;
    public bool  BateriaInstalada    => batteryInstalled;
}
