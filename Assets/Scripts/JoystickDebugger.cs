using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Rode o jogo, pressione cada botão e mova os analógicos.
/// O console vai mostrar TODOS os controles ativos do joystick.
/// </summary>
public class JoystickDebugger : MonoBehaviour
{
    void Update()
    {
        var js = Joystick.current;
        if (js == null) { Debug.Log("Nenhum joystick conectado"); return; }

        // botões
        foreach (var ctrl in js.allControls)
        {
            if (ctrl is ButtonControl btn && btn.wasPressedThisFrame)
                Debug.Log($"BOTAO: name='{ctrl.name}'  path='{ctrl.path}'");
        }

        // todos os eixos com valor diferente de zero
        foreach (var ctrl in js.allControls)
        {
            if (ctrl is AxisControl axis)
            {
                float val = axis.ReadValue();
                if (Mathf.Abs(val) > 0.2f)
                    Debug.Log($"EIXO: name='{ctrl.name}'  path='{ctrl.path}'  valor={val:F2}");
            }
        }
    }
}
