// using UnityEngine;
// using System.Collections;

// public class MapPortal : MonoBehaviour
// {
//     [Header("Mapa Destino")]
//     [SerializeField] private int nextMapIndex;

//     [Header("Posição Spawn")]
//     [SerializeField] private Transform spawnPoint;

//     private bool teleporting;

//     // =========================

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (teleporting)
//             return;

//         if (other.CompareTag("Player"))
//         {
//             StartCoroutine(Teleport(other.transform));
//         }
//     }

//     // =========================

//     IEnumerator Teleport(Transform player)
//     {
//         teleporting = true;

//         // fade
//         if (UIFade.Instance != null)
//             yield return StartCoroutine(UIFade.Instance.FadeOut());

//         // troca mapa
//         MapController.Instance.ChangeMap(nextMapIndex);

//         yield return null;

//         // move player
//         player.position = spawnPoint.position;

//         yield return new WaitForSeconds(0.1f);

//         // fade volta
//         if (UIFade.Instance != null)
//             yield return StartCoroutine(UIFade.Instance.FadeIn());

//         teleporting = false;
//     }
// }


using UnityEngine;
using System.Collections;

public class MapPortal : MonoBehaviour
{
    [SerializeField] private int nextMapIndex;
    [SerializeField] private Transform spawnPoint;

    private bool teleporting;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (teleporting)
            return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(Teleport(other.transform));
        }
    }

    IEnumerator Teleport(Transform player)
    {
        teleporting = true;

        // fade preto
        if (UIFade.Instance != null)
            yield return StartCoroutine(UIFade.Instance.FadeOut());

        yield return new WaitForSeconds(0.2f);

        // troca mapa
        MapController.Instance.ChangeMap(nextMapIndex);

        yield return null;

        // move player
        player.position = spawnPoint.position;

        yield return new WaitForSeconds(0.2f);

        // volta imagem
        if (UIFade.Instance != null)
            yield return StartCoroutine(UIFade.Instance.FadeIn());

        teleporting = false;
    }
}