// using UnityEngine;
// using TMPro;
// using UnityEngine.InputSystem;

// public class DialogueUI : MonoBehaviour
// {
//     public static DialogueUI Instance;

//     [Header("UI")]
//     [SerializeField] private GameObject panel;
//     [SerializeField] private TextMeshProUGUI nameText;
//     [SerializeField] private TextMeshProUGUI dialogueText;

//     private string[] lines;
//     private int index;

//     private bool dialogueActive;

//     void Awake()
//     {
//         Instance = this;

//         panel.SetActive(false);
//     }

//     void Update()
//     {
//         if (!dialogueActive)
//             return;

//         if (Keyboard.current.fKey.wasPressedThisFrame)
//         {
//             NextLine();
//         }
//     }

//     // =========================
//     // INICIAR
//     // =========================

//     public void StartDialogue(string speaker, string[] dialogueLines)
//     {
//         panel.SetActive(true);

//         dialogueActive = true;

//         lines = dialogueLines;

//         index = 0;

//         nameText.text = speaker;
//         dialogueText.text = lines[index];

//         // trava player
//         Time.timeScale = 0f;
//     }

//     // =========================
//     // PRÓXIMA LINHA
//     // =========================

//     void NextLine()
//     {
//         index++;

//         if (index >= lines.Length)
//         {
//             EndDialogue();
//             return;
//         }

//         dialogueText.text = lines[index];
//     }

//     // =========================
//     // FECHAR
//     // =========================

//     void EndDialogue()
//     {
//         panel.SetActive(false);

//         dialogueActive = false;

//         Time.timeScale = 1f;
//     }

//     public bool IsDialogueActive()
//     {
//         return dialogueActive;
//     }
// }


// using UnityEngine;
// using TMPro;
// using UnityEngine.InputSystem;

// public class DialogueUI : MonoBehaviour
// {
//     public static DialogueUI Instance;

//     [Header("UI")]
//     [SerializeField] private GameObject panel;
//     [SerializeField] private TextMeshProUGUI nameText;
//     [SerializeField] private TextMeshProUGUI dialogueText;

//     private string[] lines;
//     private int index;

//     private bool dialogueActive;

//     private void Awake()
//     {
//         Instance = this;
//     }

//     private void Update()
//     {
//         if (!dialogueActive)
//             return;

//         if (Keyboard.current.fKey.wasPressedThisFrame)
//         {
//             NextLine();
//         }
//     }

//     // =========================
//     // INICIAR DIÁLOGO
//     // =========================

//     public void StartDialogue(string speaker, string[] dialogueLines)
//     {
//         if (panel == null)
//         {
//             Debug.LogError("Panel não atribuído!");
//             return;
//         }

//         panel.SetActive(true);

//         dialogueActive = true;

//         lines = dialogueLines;
//         index = 0;

//         nameText.text = speaker;
//         dialogueText.text = lines[index];

//         Time.timeScale = 0f;
//     }

//     // =========================
//     // PRÓXIMA LINHA
//     // =========================

//     private void NextLine()
//     {
//         index++;

//         if (index >= lines.Length)
//         {
//             EndDialogue();
//             return;
//         }

//         dialogueText.text = lines[index];
//     }

//     // =========================
//     // FINALIZAR
//     // =========================

//     private void EndDialogue()
//     {
//         panel.SetActive(false);

//         dialogueActive = false;

//         Time.timeScale = 1f;
//     }

//     public bool IsDialogueActive()
//     {
//         return dialogueActive;
//     }
// }

// using UnityEngine;
// using TMPro;
// using UnityEngine.InputSystem;

// public class DialogueUI : MonoBehaviour
// {
//     public static DialogueUI Instance;

//     [Header("UI")]
//     [SerializeField] private GameObject panel;

//     [SerializeField] private TextMeshProUGUI nameText;
//     [SerializeField] private TextMeshProUGUI dialogueText;

//     [SerializeField] private GameObject continueText;

//     private string[] lines;
//     private int index;

//     private bool dialogueActive;

//     // =========================

//     private void Awake()
//     {
//         Instance = this;

//         if (panel != null)
//             panel.SetActive(false);
//     }

//     // =========================

//     private void Update()
//     {
//         if (!dialogueActive)
//             return;

//         if (Keyboard.current.fKey.wasPressedThisFrame)
//         {
//             NextLine();
//         }
//     }

//     // =========================
//     // INICIAR DIÁLOGO
//     // =========================

//     public void StartDialogue(string speaker, string[] dialogueLines)
//     {
//         if (panel == null)
//         {
//             Debug.LogError("Panel não atribuído!");
//             return;
//         }

//         panel.SetActive(true);

//         dialogueActive = true;

//         lines = dialogueLines;

//         index = 0;

//         nameText.text = speaker;

//         dialogueText.text = lines[index];

//         // mostra continuar
//         if (continueText != null)
//             continueText.SetActive(true);

//         // trava player
//         Time.timeScale = 0f;
//     }

//     // =========================
//     // PRÓXIMA LINHA
//     // =========================

//     private void NextLine()
//     {
//         // esconde continuar após apertar
//         if (continueText != null)
//             continueText.SetActive(false);

//         index++;

//         // terminou diálogo
//         if (index >= lines.Length)
//         {
//             EndDialogue();
//             return;
//         }

//         dialogueText.text = lines[index];

//         // mostra novamente
//         if (continueText != null)
//             continueText.SetActive(true);
//     }

//     // =========================
//     // FINALIZAR
//     // =========================

//     private void EndDialogue()
//     {
//         dialogueActive = false;

//         if (panel != null)
//             panel.SetActive(false);

//         Time.timeScale = 1f;
//     }

//     // =========================

//     public bool IsDialogueActive()
//     {
//         return dialogueActive;
//     }
// }

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

    private bool justClosedDialogue;

    private string[] lines;
    private int index;

    private bool dialogueActive;

    // 🔥 trava primeiro frame
    private bool canAdvance;

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
        if (!dialogueActive)
            return;

        if (!canAdvance)
            return;

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

        // 🔥 impede avanço instantâneo
        StartCoroutine(EnableAdvance());
    }

    // =========================

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

        // terminou diálogo
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

    StartCoroutine(DialogueCooldown());
}

    public bool JustClosedDialogue()
{
    return justClosedDialogue;
}

IEnumerator DialogueCooldown()
{
    justClosedDialogue = true;

    yield return new WaitForSeconds(0.2f);

    justClosedDialogue = false;
}

    // =========================

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}