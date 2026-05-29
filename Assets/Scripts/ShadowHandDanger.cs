using UnityEngine;

/// <summary>
/// Coloque no GameObject "DangerZone" filho do ShadowHandTrap.
/// Este collider só fica ativo durante a garra.
/// </summary>
public class ShadowHandDanger : MonoBehaviour
{
    private ShadowHandTrap trap;

    void Awake()
    {
        trap = GetComponentInParent<ShadowHandTrap>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (trap != null)
            trap.OnPlayerCaught();
        else if (PlayerController.Instance != null)
            PlayerController.Instance.Die();
    }
}
