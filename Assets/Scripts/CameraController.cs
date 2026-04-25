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

        DontDestroyOnLoad(gameObject);

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
        // espera a cena terminar de carregar
        yield return null;
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player não encontrado para câmera");
            yield break;
        }

        // encontra todas as CinemachineCamera da cena (mesmo dentro de StateDrivenCamera)
        cams = FindObjectsByType<CinemachineCamera>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        if (cams.Length == 0)
        {
            Debug.LogError("Nenhuma CinemachineCamera encontrada na cena");
            yield break;
        }

        foreach (var cam in cams)
        {
            cam.Follow = player.transform;
        }
    }
}