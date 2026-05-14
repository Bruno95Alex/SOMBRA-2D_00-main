using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject continueText;

    private string[] lines;
    private int index;
    private bool dialogueActive;
    private bool canAdvance;
    private bool justClosedDialogue;

    // =========================

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    // =========================

    private void Update()
    {
        if (!dialogueActive || !canAdvance) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            NextLine();
        }
    }

    // =========================
    // INICIAR DIÁLOGO
    // =========================

    public void StartDialogue(string speaker, string[] dialogueLines)
    {
        panel.SetActive(true);
        dialogueActive = true;
        lines = dialogueLines;
        index = 0;

        nameText.text = speaker;
        dialogueText.text = lines[index];

        if (continueText != null)
            continueText.SetActive(true);

        StartCoroutine(EnableAdvance());
    }

    IEnumerator EnableAdvance()
    {
        canAdvance = false;
        yield return null;
        yield return null;
        canAdvance = true;
    }

    // =========================
    // PRÓXIMA LINHA
    // =========================

    private void NextLine()
    {
        index++;

        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = lines[index];
    }

    // =========================
    // FINALIZAR
    // =========================

    private void EndDialogue()
    {
        dialogueActive = false;
        canAdvance = false;
        panel.SetActive(false);

        if (continueText != null)
            continueText.SetActive(false);

        StartCoroutine(DialogueCooldown());
    }

    IEnumerator DialogueCooldown()
    {
        justClosedDialogue = true;
        yield return new WaitForSeconds(0.2f);
        justClosedDialogue = false;
    }

    // =========================

    public bool IsDialogueActive() => dialogueActive;
    public bool JustClosedDialogue() => justClosedDialogue;
}
