using UnityEngine;

/// <summary>
/// Gerencia o puzzle dos três armários.
/// Coloque num GameObject vazio chamado "CabinetPuzzle".
///
/// ORDEM CORRETA: índice 0 = relâmpago, 1 = meio, 2 = escondido
/// </summary>
public class CabinetPuzzleManager : MonoBehaviour
{
    public static CabinetPuzzleManager Instance;

    [Header("Armários na ordem correta")]
    [SerializeField] private PuzzleCabinet[] correctOrder; // arraste: [0]=relâmpago [1]=meio [2]=escondido

    [Header("Porta que destrava")]
    [SerializeField] private DoorLocked targetDoor;

    [Header("Sons")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   correctSequenceClip; // clique metálico final
    [SerializeField] private AudioClip   wrongOrderClip;      // som de erro / reset

    private int currentStep = 0;
    private bool solved = false;

    void Awake()
    {
        Instance = this;
    }

    // =========================
    // CHAMADO PELO PuzzleCabinet
    // =========================

    public void OnCabinetOpened(PuzzleCabinet cabinet)
    {
        if (solved) return;

        if (cabinet == correctOrder[currentStep])
        {
            // passo certo
            currentStep++;

            if (currentStep >= correctOrder.Length)
                PuzzleSolved();
        }
        else
        {
            // erro — reseta tudo
            StartCoroutine(ResetPuzzle());
        }
    }

    // =========================
    // PUZZLE RESOLVIDO
    // =========================

    void PuzzleSolved()
    {
        solved = true;

        if (audioSource != null && correctSequenceClip != null)
            audioSource.PlayOneShot(correctSequenceClip);

        UIMessage.Instance.Show("*clique metálico* A porta foi destrancada.", 3f);

        // destranca a porta diretamente sem precisar de itens
        if (targetDoor != null)
            targetDoor.UnlockByPuzzle();

        Debug.Log("Puzzle resolvido!");
    }

    // =========================
    // RESET
    // =========================

    System.Collections.IEnumerator ResetPuzzle()
    {
        if (audioSource != null && wrongOrderClip != null)
            audioSource.PlayOneShot(wrongOrderClip);

        UIMessage.Instance.Show("Os armários se fecham novamente...", 2f);

        yield return new WaitForSeconds(0.5f);

        foreach (var cabinet in correctOrder)
            cabinet.ForceClose();

        currentStep = 0;
    }

    // =========================
    // UTILITÁRIO
    // =========================

    public bool IsSolved() => solved;
}
