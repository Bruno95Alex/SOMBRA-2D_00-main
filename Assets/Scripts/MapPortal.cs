using UnityEngine;
using System.Collections;

public class MapPortal : MonoBehaviour
{
    [Header("Mapa Destino")]
    [SerializeField] private int nextMapIndex;

    [Header("Posição de Spawn")]
    [SerializeField] private Transform spawnPoint;

    [Header("Transição")]
    [SerializeField] private Sprite mapTransitionImage;

    [TextArea(3, 6)]
    [SerializeField] private string transitionText;

    [SerializeField] private float transitionDuration = 4f;

    private bool teleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (teleporting)
            return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(Teleport(other.transform));
        }
    }

    private IEnumerator Teleport(Transform player)
    {
        teleporting = true;

        // Fade para preto
        if (UIFade.Instance != null)
            yield return StartCoroutine(UIFade.Instance.FadeOut());

        // Mostra imagem da transição
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(
                TransitionManager.Instance.ShowTransition(
                    mapTransitionImage,
                    transitionText,
                    transitionDuration
                )
            );
        }

        // Troca o mapa
        if (MapController.Instance != null)
        {
            MapController.Instance.ChangeMap(nextMapIndex);
        }

        yield return null;

        // Move jogador
        if (spawnPoint != null)
        {
            player.position = spawnPoint.position;
        }

        yield return new WaitForSeconds(0.2f);

        // Volta do preto
        if (UIFade.Instance != null)
            yield return StartCoroutine(UIFade.Instance.FadeIn());

        teleporting = false;
    }
}