using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraController : Singleton<CameraController>
{
    private CinemachineCamera[] cams;

    protected override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetPlayerCameraFollow());
    }

    private IEnumerator SetPlayerCameraFollow()
    {
        // espera a cena estabilizar
        yield return null;
        yield return null;

        // 🚫 evita conflito durante teleporte
        if (GameState.IsTeleporting)
            yield break;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("❌ Player não encontrado para câmera");
            yield break;
        }

        cams = FindObjectsByType<CinemachineCamera>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        if (cams == null || cams.Length == 0)
        {
            Debug.LogError("❌ Nenhuma CinemachineCamera encontrada");
            yield break;
        }

        foreach (var cam in cams)
        {
            if (cam == null) continue;

            // 🔥 PARA FOLLOW TEMPORARIAMENTE
            cam.Follow = null;

            // 🔥 REMOVE SUAVIZAÇÃO (compatível com qualquer versão)
            var behaviours = cam.GetComponentsInChildren<MonoBehaviour>();

            foreach (var b in behaviours)
            {
                if (b == null) continue;

                var type = b.GetType().Name;

                if (type.Contains("Transposer") || type.Contains("Composer"))
                {
                    var dampingX = b.GetType().GetField("m_XDamping");
                    var dampingY = b.GetType().GetField("m_YDamping");

                    if (dampingX != null) dampingX.SetValue(b, 0f);
                    if (dampingY != null) dampingY.SetValue(b, 0f);
                }
            }

            // 🔥 TELEPORTA CÂMERA INSTANTANEAMENTE
            cam.ForceCameraPosition(player.transform.position, Quaternion.identity);

            // 🔥 VOLTA A SEGUIR O PLAYER
            cam.Follow = player.transform;
        }
    }
}