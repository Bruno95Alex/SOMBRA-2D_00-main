using UnityEngine;
using TMPro;

public class PickupUI : MonoBehaviour
{
    public static PickupUI Instance;

    [SerializeField] private GameObject textObject;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowText()
    {
        textObject.SetActive(true);
    }

    public void HideText()
    {
        textObject.SetActive(false);
    }
}