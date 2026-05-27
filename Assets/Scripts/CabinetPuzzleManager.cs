using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CabinetPuzzleManager : MonoBehaviour
{
    public static CabinetPuzzleManager Instance;

    [Header("Armários na ordem correta")]
    [SerializeField] private PuzzleCabinet[] correctOrder;

    [Header("Porta que destrava")]
    [SerializeField] private DoorLocked targetDoor;

    [Header("Sons")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   correctSequenceClip;
    [SerializeField] private AudioClip   wrongOrderClip;

    private int  currentStep = 0;
    private bool solved      = false;
    private bool resetting   = false;

    // todos os armários abertos até agora
    private List<PuzzleCabinet> abertos = new List<PuzzleCabinet>();

    void Awake() { Instance = this; }

    // =========================
    // CHAMADO APÓS Open() no PuzzleCabinet
    // =========================

    public void OnCabinetOpened(PuzzleCabinet cabinet)
    {
        Debug.Log($"[Manager] OnCabinetOpened: {cabinet.gameObject.name} | step={currentStep}/{correctOrder.Length} | solved={solved} | resetting={resetting}");

        if (solved || resetting) return;

        if (!abertos.Contains(cabinet))
            abertos.Add(cabinet);

        bool correto = cabinet == correctOrder[currentStep];
        Debug.Log($"[Manager] Correto={correto} | esperado={correctOrder[currentStep].gameObject.name}");

        if (correto)
        {
            currentStep++;
            if (currentStep >= correctOrder.Length)
                PuzzleSolved();
        }
        else
        {
            StartCoroutine(ResetPuzzle());
        }
    }

    // =========================
    // RESOLVIDO
    // =========================

    void PuzzleSolved()
    {
        solved = true;
        abertos.Clear();

        if (audioSource != null && correctSequenceClip != null)
            audioSource.PlayOneShot(correctSequenceClip);

        UIMessage.Instance.Show("*clique metálico* A porta foi destrancada.", 3f);

        if (targetDoor != null)
            targetDoor.UnlockByPuzzle();
    }

    // =========================
    // RESET
    // =========================

    IEnumerator ResetPuzzle()
    {
        resetting = true;

        if (audioSource != null && wrongOrderClip != null)
            audioSource.PlayOneShot(wrongOrderClip);

        UIMessage.Instance.Show("Os armários se fecham novamente...", 2f);

        yield return new WaitForSeconds(0.3f);

        foreach (var cabinet in abertos)
            if (cabinet != null)
                cabinet.ForceClose();

        abertos.Clear();
        currentStep = 0;

        // aguarda animações de fechar
        yield return new WaitForSeconds(1.0f);

        resetting = false;
    }

    public bool IsSolved()    => solved;
    public bool IsResetting() => resetting;
}
