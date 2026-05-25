using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance;

    public Vector2 MoveInput        { get; private set; }
    public Vector2 AimInput         { get; private set; }
    public bool    JumpPressed       { get; private set; }
    public bool    InteractPressed   { get; private set; }
    public bool    FlashlightPressed { get; private set; }
    public bool    InventoryPressed  { get; private set; }
    public bool    MenuPressed       { get; private set; }
    public bool    IsUsingController { get; private set; }

    private const float DEAD_ZONE = 0.2f;

    private bool triggerLastFrame = false;
    private bool bolaLastFrame    = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        var kb = Keyboard.current;
        var js = Joystick.current;
        var gp = Gamepad.current;

        // =========================
        // MOVIMENTO
        // =========================
        Vector2 move = Vector2.zero;

        if (js != null) move = js.stick.ReadValue();
        if (move.magnitude < DEAD_ZONE && gp != null) move = gp.leftStick.ReadValue();
        if (move.magnitude < DEAD_ZONE && gp != null) move = gp.dpad.ReadValue();

        if (move.magnitude < DEAD_ZONE && kb != null)
        {
            float x = 0f, y = 0f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  x -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  y -= 1f;
            move = new Vector2(x, y);
            if (move.magnitude > 1f) move.Normalize();
        }

        MoveInput = move;

        // =========================
        // ANALÓGICO DIREITO
        // Twin USB confirmado:
        //   rz = horizontal (esquerda/direita) — negado para corrigir direção
        //   z  = vertical   (cima/baixo)       — negado pois Twin USB inverte Y
        // =========================
        float aimX = 0f, aimY = 0f;

        if (js != null)
        {
            foreach (var ctrl in js.allControls)
            {
                if (ctrl is AxisControl axis)
                {
                    if (ctrl.name == "rz") aimX = axis.ReadValue(); // horizontal invertido
                    if (ctrl.name == "z")  aimY = -axis.ReadValue(); // vertical invertido
                }
            }
        }

        AimInput = new Vector2(aimX, aimY);

        // =========================
        // TRIÂNGULO — eixo "trigger"
        // =========================
        bool triggerNow = false;
        if (js != null)
            foreach (var ctrl in js.allControls)
                if (ctrl.name == "trigger" && ctrl is AxisControl ax)
                    triggerNow = ax.ReadValue() > 0.5f;

        bool triggerPressed = triggerNow && !triggerLastFrame;
        triggerLastFrame    = triggerNow;

        // =========================
        // BOLA — eixo "button2"
        // =========================
        bool bolaNow = false;
        if (js != null)
            foreach (var ctrl in js.allControls)
                if (ctrl.name == "button2" && ctrl is AxisControl ax)
                    bolaNow = ax.ReadValue() > 0.5f;

        bool bolaPressed = bolaNow && !bolaLastFrame;
        bolaLastFrame    = bolaNow;

        // =========================
        // AÇÕES
        // =========================
        JumpPressed = (kb != null && kb.spaceKey.wasPressedThisFrame)
                   || BotaoJS(js, "button3")
                   || (gp != null && gp.buttonSouth.wasPressedThisFrame);

        InteractPressed = (kb != null && (kb.eKey.wasPressedThisFrame || kb.fKey.wasPressedThisFrame))
                       || triggerPressed
                       || (gp != null && gp.buttonNorth.wasPressedThisFrame);

        FlashlightPressed = (kb != null && kb.eKey.wasPressedThisFrame)
                         || BotaoJS(js, "button4")
                         || (gp != null && gp.buttonWest.wasPressedThisFrame);

        InventoryPressed = (kb != null && kb.iKey.wasPressedThisFrame)
                        || bolaPressed
                        || (gp != null && gp.buttonEast.wasPressedThisFrame);

        MenuPressed = (kb != null && kb.escapeKey.wasPressedThisFrame)
                   || BotaoJS(js, "button10")
                   || (gp != null && gp.startButton.wasPressedThisFrame);

        // =========================
        // DISPOSITIVO ATIVO
        // =========================
        bool controleAtivo = (js != null && js.stick.ReadValue().magnitude > DEAD_ZONE)
                          || (js != null && AnyButtonJS(js))
                          || triggerNow || bolaNow
                          || AimInput.magnitude > DEAD_ZONE;

        if (controleAtivo)                     IsUsingController = true;
        if (kb != null && kb.anyKey.isPressed) IsUsingController = false;
    }

    bool BotaoJS(Joystick js, string nome)
    {
        if (js == null) return false;
        foreach (var ctrl in js.allControls)
            if (ctrl.name == nome && ctrl is ButtonControl btn)
                return btn.wasPressedThisFrame;
        return false;
    }

    bool AnyButtonJS(Joystick js)
    {
        foreach (var ctrl in js.allControls)
            if (ctrl is ButtonControl btn && btn.wasPressedThisFrame)
                return true;
        return false;
    }
}
