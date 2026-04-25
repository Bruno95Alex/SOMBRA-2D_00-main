// // using UnityEngine;

// // public class Teleport : MonoBehaviour
// // {
// //     [SerializeField] private Transform destino;

// //     private void OnTriggerEnter2D(Collider2D other)
// //     {
// //         if (other.CompareTag("Player"))
// //         {
// //             if (destino == null)
// //             {
// //                 Debug.LogError("Destino não definido!");
// //                 return;
// //             }

// //             other.transform.position = destino.position;
// //         }
// //     }
// // }


// using UnityEngine;
// using System.Collections;

// public class Telepor : MonoBehaviour
// {
//     [SerializeField] private Transform destino;

//     private bool teleportando = false;

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player") && !teleportando)
//         {
//             StartCoroutine(Teleport(other.gameObject));
//         }
//     }

//     // IEnumerator Teleport(GameObject player)
//     // {
//     //     teleportando = true;

//     //     // 🔥 segurança
//     //     if (UIFade.Instance == null)
//     //     {
//     //         Debug.LogError("UIFade não encontrado!");
//     //         yield break;
//     //     }

//     //     // fade para preto
//     //     yield return StartCoroutine(UIFade.Instance.FadeOut());

//     //     // teleporta
//     //     if (destino != null)
//     //     {
//     //         player.transform.position = destino.position;
//     //     }
//     //     else
//     //     {
//     //         Debug.LogError("Destino não definido!");
//     //     }

//     //     yield return new WaitForSeconds(0.2f);

//     //     // fade volta
//     //     yield return StartCoroutine(UIFade.Instance.FadeIn());

//     //     teleportando = false;
//     // }
// //     IEnumerator Teleport(GameObject player)
// // {
// //     teleportando = true;

// //     if (UIFade.Instance == null)
// //     {
// //         Debug.LogError("❌ UIFade NÃO existe na cena!");
// //         yield break;
// //     }

// //     yield return StartCoroutine(UIFade.Instance.FadeOut());

// //     if (destino != null)
// //         player.transform.position = destino.position;

// //     yield return new WaitForSeconds(0.2f);

// //     yield return StartCoroutine(UIFade.Instance.FadeIn());

// //     teleportando = false;
// // }

// IEnumerator Teleport(GameObject player)
// {
//     teleportando = true;

//     if (UIFade.Instance == null)
//     {
//         Debug.LogError("UIFade não encontrado!");
//         yield break;
//     }

//     // 🔥 1. Fade até preto COMPLETO
//     yield return StartCoroutine(UIFade.Instance.FadeOut());

//     // 🔥 2. ESPERA um frame (garante tela preta)
//     yield return null;

//     // 🔥 3. Teleporta (agora invisível)
//     if (destino != null)
//     {
//         player.transform.position = destino.position;
//     }

//     // 🔥 4. Espera um pouco (camera estabilizar)
//     yield return new WaitForSeconds(0.1f);

//     // 🔥 5. Volta imagem
//     yield return StartCoroutine(UIFade.Instance.FadeIn());

//     teleportando = false;
// }


// }



using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class TeleportFade : MonoBehaviour
{
    [SerializeField] private Transform destino;

    private bool teleportando = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !teleportando)
        {
            StartCoroutine(Teleport(other.gameObject));
        }
    }

    IEnumerator Teleport(GameObject player)
    {
        teleportando = true;

        yield return StartCoroutine(UIFade.Instance.FadeOut());

        yield return null; // garante tela preta

        // 🔥 TELEPORTA PLAYER
        player.transform.position = destino.position;

        // 🔥 FORÇA CÂMERA (ESSA LINHA É A CHAVE)
        ForceCameraInstant(player.transform);

        yield return new WaitForSeconds(0.05f);

        yield return StartCoroutine(UIFade.Instance.FadeIn());

        teleportando = false;
    }

    void ForceCameraInstant(Transform target)
{
    var cam = FindFirstObjectByType<CinemachineCamera>();

    if (cam != null)
    {
        cam.Follow = null;

        Camera.main.transform.position = new Vector3(
            target.position.x,
            target.position.y,
            Camera.main.transform.position.z
        );

        cam.Follow = target;
    }
}
}