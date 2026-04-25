using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Transform flashlight;

    private Camera cam;
    private bool ligada;

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

    void QuandoTrocarCena(Scene scene, LoadSceneMode mode)
    {
        AtualizarCamera();
    }

    void AtualizarCamera()
    {
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogWarning("Camera principal não encontrada ainda...");
        }
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ligada = !ligada;
            flashlight.gameObject.SetActive(ligada);
        }
    }

    void LateUpdate()
    {
        if (!ligada)
            return;

        if (cam == null)
        {
            AtualizarCamera();
            return;
        }

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPos.x,
                mouseScreenPos.y,
                Mathf.Abs(cam.transform.position.z)
            )
        );

        Vector2 direction = mouseWorldPos - transform.position;

        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        flashlight.rotation = Quaternion.Euler(0, 0, angle);
    }
}