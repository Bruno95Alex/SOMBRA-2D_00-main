using System.Collections.Generic;
using UnityEngine;

public class DiarySystem : MonoBehaviour
{
    public static DiarySystem Instance;

    private List<string> pages = new List<string>();
    private int currentIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddPage(string text)
    {
        pages.Add(text);
        currentIndex = pages.Count - 1;
    }

    public string GetCurrentPage()
    {
        if (pages.Count == 0) return "";
        return pages[currentIndex];
    }

    public void NextPage()
    {
        if (currentIndex < pages.Count - 1)
            currentIndex++;
    }

    public void PreviousPage()
    {
        if (currentIndex > 0)
            currentIndex--;
    }
}