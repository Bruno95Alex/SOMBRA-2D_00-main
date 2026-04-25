// using UnityEngine;

// public class FlashlightTrigger : MonoBehaviour
// {
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         var effect = other.GetComponent<LightInteractEffect>();

//         if (effect != null)
//         {
//             effect.OnLightEnter();
//         }
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         var effect = other.GetComponent<LightInteractEffect>();

//         if (effect != null)
//         {
//             effect.OnLightExit();
//         }
//     }
// }


// using UnityEngine;

// public class FlashlightTrigger : MonoBehaviour
// {
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         // 🔥 só reage a itens
//         if (!other.CompareTag("Item")) return;

//         var effect = other.GetComponent<LightInteractEffect>();

//         if (effect != null)
//         {
//             effect.OnLightEnter();
//         }
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         // 🔥 só reage a itens
//         if (!other.CompareTag("Item")) return;

//         var effect = other.GetComponent<LightInteractEffect>();

//         if (effect != null)
//         {
//             effect.OnLightExit();
//         }
//     }
// }



using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    private int itemLayer;

    void Start()
    {
        itemLayer = LayerMask.NameToLayer("Item");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != itemLayer) return;

        var effect = other.GetComponent<LightInteractEffect>();

        if (effect != null)
        {
            effect.OnLightEnter();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != itemLayer) return;

        var effect = other.GetComponent<LightInteractEffect>();

        if (effect != null)
        {
            effect.OnLightExit();
        }
    }
}