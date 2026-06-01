using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraController : Singleton<CameraController>
{
    private CinemachineCamera[] cams;

    [Header("Cenas sem câmera Cinemachine (ex: menu)")]
    [SerializeField] private string[] cenasIgnoradas = { "Menu" };

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
        // ignora cenas de menu ou sem Cinemachine
        foreach (var cena in cenasIgnoradas)
            if (scene.name == cena) return;

        StartCoroutine(SetPlayerCameraFollow());
    }

    private IEnumerator SetPlayerCameraFollow()
    {
        yield return null;
        yield return null;

        if (GameState.IsTeleporting)
            yield break;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("CameraController: Player não encontrado na cena — ignorando.");
            yield break;
        }

        cams = FindObjectsByType<CinemachineCamera>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        if (cams == null || cams.Length == 0)
        {
            Debug.LogWarning("CameraController: Nenhuma CinemachineCamera na cena — ignorando.");
            yield break;
        }

        foreach (var cam in cams)
        {
            if (cam == null) continue;

            cam.Follow = null;

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

            cam.ForceCameraPosition(player.transform.position, Quaternion.identity);
            cam.Follow = player.transform;
        }
    }
}
