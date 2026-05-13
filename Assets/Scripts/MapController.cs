using UnityEngine;

public class MapController : MonoBehaviour
{
    public static MapController Instance;

    [Header("Mapas")]
    [SerializeField] private GameObject[] maps;

    private int currentMap;

    // =========================

    private void Awake()
    {
        Instance = this;
    }

    // =========================
    // TROCAR MAPA
    // =========================

    public void ChangeMap(int nextMap)
    {
        if (nextMap < 0 || nextMap >= maps.Length)
        {
            Debug.LogError("Mapa inválido");
            return;
        }

        // desliga mapa atual
        maps[currentMap].SetActive(false);

        // liga próximo
        maps[nextMap].SetActive(true);

        currentMap = nextMap;

        Debug.Log("Mapa atual: " + currentMap);
    }
}