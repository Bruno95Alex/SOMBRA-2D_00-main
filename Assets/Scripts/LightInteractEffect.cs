using UnityEngine;

/// <summary>
/// Ponte entre o FlashlightTrigger e o ItemGlow.
/// Coloque este componente no mesmo GameObject que o ItemGlow.
/// O FlashlightTrigger detecta pela Layer "Item" e chama OnLightEnter/Exit.
/// </summary>
public class LightInteractEffect : MonoBehaviour
{
    private ItemGlow itemGlow;

    void Awake()
    {
        itemGlow = GetComponent<ItemGlow>();

        if (itemGlow == null)
            Debug.LogWarning("LightInteractEffect: ItemGlow não encontrado em " + gameObject.name);
    }

    public void OnLightEnter()
    {
        if (itemGlow != null)
            itemGlow.SetLit(true);
    }

    public void OnLightExit()
    {
        if (itemGlow != null)
            itemGlow.SetLit(false);
    }

    public void SetLit(bool value)
    {
        if (value) OnLightEnter();
        else OnLightExit();
    }
}
