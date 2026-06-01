using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Diálogo de confirmação reutilizável (Sim/Não).
/// Usado para "Iniciar novo jogo?" e "Substituir save?".
///
/// HIERARQUIA:
/// ConfirmDialog (este script)
///   └── DialogPanel
///       ├── Mensagem (TextMeshPro)
///       ├── BtnSim (Button)
///       └── BtnNao (Button)
/// </summary>
public class ConfirmDialog : MonoBehaviour
{
    public static ConfirmDialog Instance;

    [SerializeField] private GameObject      painel;
    [SerializeField] private TextMeshProUGUI mensagem;
    [SerializeField] private Button          btnSim;
    [SerializeField] private Button          btnNao;

    private Action onSim;
    private Action onNao;

    void Awake()
    {
        Instance = this;
        if (painel != null) painel.SetActive(false);
    }

    public void Mostrar(string texto, Action aoConfirmar, Action aoCancelar = null)
    {
        if (painel    != null) painel.SetActive(true);
        if (mensagem  != null) mensagem.text = texto;

        onSim = aoConfirmar;
        onNao = aoCancelar;
    }

    public void EscolheuSim()
    {
        if (painel != null) painel.SetActive(false);
        onSim?.Invoke();
    }

    public void EscolheuNao()
    {
        if (painel != null) painel.SetActive(false);
        onNao?.Invoke();
    }
}
