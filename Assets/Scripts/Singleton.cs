using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = (T)this;

        // Sobe até a raiz para garantir DontDestroyOnLoad mesmo em objetos filhos
        Transform root = transform.root;
        DontDestroyOnLoad(root.gameObject);
    }
}
