using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Transform flashlight;
    [SerializeField] private float rotationSpeed = 12f; // suavidade da rotação

    private Camera cam;
    private bool ligada;
    private float currentAngle = 0f;
    private float targetAngle  = 0f;

    // guarda última direção do controle para não cair no mouse ao soltar o stick
    private bool usandoControle = false;

    void Awake()
    {
        AtualizarCamera();
        flashlight.gameObject.SetActive(false);
        SceneManager.sceneLoaded += QuandoTrocarCena;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= QuandoTrocarCena;
    }

    void QuandoTrocarCena(Scene scene, LoadSceneMode mode) => AtualizarCamera();

    void AtualizarCamera()
    {
        cam = Camera.main;
        if (cam == null)
            Debug.LogWarning("FlashlightController: camera não encontrada.");
    }

    void Update()
    {
        bool toggle = InputReader.Instance != null
            ? InputReader.Instance.FlashlightPressed
            : Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

        if (toggle)
        {
            ligada = !ligada;
            flashlight.gameObject.SetActive(ligada);
        }
    }

    void LateUpdate()
    {
        if (!ligada || cam == null) return;

        Vector2 aim = InputReader.Instance != null
            ? InputReader.Instance.AimInput
            : Vector2.zero;

        if (aim.magnitude > 0.2f)
        {
            // stick sendo movido — marca que está no controle e calcula ângulo alvo
            usandoControle = true;
            targetAngle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg - 90f;
        }
        else if (!usandoControle)
        {
            // só usa mouse se NÃO estiver no modo controle
            if (Mouse.current == null) return;

            Vector3 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 mouseWorld  = cam.ScreenToWorldPoint(
                new Vector3(mouseScreen.x, mouseScreen.y,
                            Mathf.Abs(cam.transform.position.z)));

            Vector2 dir = (Vector2)(mouseWorld - transform.position);
            if (dir.magnitude < 0.01f) return;

            targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        }
        // se usandoControle=true e stick zerado: mantém targetAngle da última direção

        // detecta se voltou para teclado/mouse — reseta modo controle
        if (InputReader.Instance != null && !InputReader.Instance.IsUsingController)
            usandoControle = false;

        // suaviza a rotação — LerpAngle lida com a virada de 360→0 corretamente
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        flashlight.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}
