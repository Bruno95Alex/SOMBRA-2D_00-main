using UnityEngine;

/// <summary>
/// Script temporário para apagar todos os saves durante o desenvolvimento.
/// REMOVA antes de publicar o jogo.
/// Coloque em qualquer GameObject, rode o jogo e pressione Delete.
/// </summary>
public class ClearSaves : MonoBehaviour
{
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.deleteKey.wasPressedThisFrame)
        {
            for (int i = 0; i < 3; i++)
            {
                if (SaveSystem.Instance != null)
                    SaveSystem.Instance.DeletarSave(i);
            }

            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            Debug.Log("Todos os saves apagados!");
        }
    }
}
