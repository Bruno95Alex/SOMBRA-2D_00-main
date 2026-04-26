using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class TeleportFade : MonoBehaviour
{
    [SerializeField] private Transform destino;

    

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(Teleport(other.gameObject));
        }
    }

    IEnumerator Teleport(GameObject player)
    {
        isTeleporting = true;
        GameState.IsTeleporting = true;

        var cam = FindFirstObjectByType<CinemachineCamera>();
        var brain = Camera.main.GetComponent<CinemachineBrain>();

        // 🔥 DESLIGA BLEND DA CAMERA
        if (brain != null)
        {
            brain.DefaultBlend.Time = 0f;
        }

        // 🔥 FADE OUT
        yield return StartCoroutine(UIFade.Instance.FadeOut());

        // 🔥 PARA FOLLOW
        if (cam != null)
            cam.Follow = null;

        // 🔥 TELEPORTA PLAYER
        player.transform.position = destino.position;

        // 🔥 TELEPORTA CAMERA JUNTO
        if (cam != null)
            cam.ForceCameraPosition(destino.position, Quaternion.identity);

        yield return null;

        // 🔥 VOLTA FOLLOW
        if (cam != null)
            cam.Follow = player.transform;

        // 🔥 FADE IN
        yield return StartCoroutine(UIFade.Instance.FadeIn());

        GameState.IsTeleporting = false;
        isTeleporting = false;
    }

}