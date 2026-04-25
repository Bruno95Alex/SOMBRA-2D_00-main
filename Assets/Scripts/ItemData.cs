// using UnityEngine;

// [System.Serializable]
// public class ItemData
// {
//     public Sprite icon;
//     public string description;

//     public bool isDiaryPage;
// }


using UnityEngine;

[CreateAssetMenu(menuName = "Item/Novo Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;

    public bool isDiaryPage;
}