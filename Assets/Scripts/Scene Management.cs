using UnityEngine;

public class SceneManagement : Singleton<SceneManagement>
{
    public string SceneTransitionName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void SetTransitionName(string sceneTransitionName)
    {
        SceneTransitionName = sceneTransitionName;
    }
}