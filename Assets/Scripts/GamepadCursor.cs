using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Permite que o jogador mire a lanterna com o analógico direito do joystick.
/// No teclado/mouse a lanterna já segue o mouse — este script só atua
/// quando um gamepad está conectado e o stick direito é movido.
///
/// Coloque no mesmo GameObject da lanterna (filho do player).
/// </summary>
public class GamepadCursor : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform flashlight; // objeto da lanterna

    [Header("Sensibilidade do stick direito")]
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float deadzone = 0.2f; // ignora tremido no centro

    private Vector2 stickInput;
    private bool usingGamepad = false;

    void Update()
    {
        DetectarDispositivo();

        if (!usingGamepad) return;
        if (flashlight == null) return;

        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        stickInput = gamepad.rightStick.ReadValue();

        // aplica zona morta
        if (stickInput.magnitude < deadzone)
            return;

        // rotaciona a lanterna na direção do stick
        float angle = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;
        Quaternion target = Quaternion.Euler(0f, 0f, angle);
        flashlight.rotation = Quaternion.Lerp(
            flashlight.rotation,
            target,
            rotationSpeed * Time.deltaTime
        );
    }

    void DetectarDispositivo()
    {
        // troca para gamepad se o stick for movido
        if (Gamepad.current != null &&
            Gamepad.current.leftStick.ReadValue().magnitude > deadzone)
        {
            usingGamepad = true;
        }

        // volta para mouse se o mouse for movido
        if (Mouse.current != null &&
            Mouse.current.delta.ReadValue().magnitude > 0.1f)
        {
            usingGamepad = false;
        }
    }

    public bool IsUsingGamepad() => usingGamepad;
}
