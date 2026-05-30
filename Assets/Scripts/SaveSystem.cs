using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Gerencia save e load do jogo.
/// Singleton persistente entre cenas.
///
/// SETUP:
/// Adicione num GameObject na primeira cena.
/// Todos os outros sistemas registram-se aqui via interface ISaveable.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    private const string SAVE_KEY    = "SOMBRA_Save_";
    private const int    MAX_SLOTS   = 3;

    // save atual em memória
    private SaveData dadosAtuais = new SaveData();

    // slot ativo
    public int SlotAtivo { get; private set; } = 0;

    // evento disparado após carregar
    public System.Action OnSaveLoaded;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ================================
    // SALVAR
    // ================================

    public void Salvar(int slot = -1)
    {
        if (slot >= 0) SlotAtivo = slot;

        // coleta dados de todos os sistemas
        ColetarDados();

        // serializa e salva
        string json = JsonUtility.ToJson(dadosAtuais, true);
        PlayerPrefs.SetString(SAVE_KEY + SlotAtivo, json);
        PlayerPrefs.SetInt("SaveExists_" + SlotAtivo, 1);
        PlayerPrefs.SetString("SaveDate_"  + SlotAtivo, System.DateTime.Now.ToString("dd/MM/yy HH:mm"));
        PlayerPrefs.Save();

        Debug.Log($"[SaveSystem] Jogo salvo no slot {SlotAtivo}");

        UIMessage.Instance?.Show("Jogo salvo!", 2f);
    }

    // ================================
    // CARREGAR
    // ================================

    public bool Carregar(int slot)
    {
        string key = SAVE_KEY + slot;

        if (!PlayerPrefs.HasKey(key))
        {
            Debug.LogWarning($"[SaveSystem] Nenhum save no slot {slot}");
            return false;
        }

        string json = PlayerPrefs.GetString(key);
        dadosAtuais = JsonUtility.FromJson<SaveData>(json);
        SlotAtivo   = slot;

        Debug.Log($"[SaveSystem] Save carregado do slot {slot}");

        // carrega a cena salva
        SceneManager.LoadScene(dadosAtuais.cenaAtual);
        SceneManager.sceneLoaded += AplicarDadosAoCarregarCena;

        return true;
    }

    void AplicarDadosAoCarregarCena(UnityEngine.SceneManagement.Scene scene,
                                     LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= AplicarDadosAoCarregarCena;
        AplicarDados();
    }

    // ================================
    // VERIFICAR SE TEM SAVE
    // ================================

    public bool TemSave(int slot)
        => PlayerPrefs.HasKey(SAVE_KEY + slot);

    public string DataDoSave(int slot)
        => PlayerPrefs.GetString("SaveDate_" + slot, "");

    public bool TemQualquerSave()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
            if (TemSave(i)) return true;
        return false;
    }

    public int PrimeiroSlotComSave()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
            if (TemSave(i)) return i;
        return -1;
    }

    // ================================
    // DELETAR SAVE
    // ================================

    public void DeletarSave(int slot)
    {
        PlayerPrefs.DeleteKey(SAVE_KEY + slot);
        PlayerPrefs.DeleteKey("SaveExists_" + slot);
        PlayerPrefs.DeleteKey("SaveDate_"   + slot);
        PlayerPrefs.Save();
        Debug.Log($"[SaveSystem] Save do slot {slot} deletado");
    }

    // ================================
    // COLETAR DADOS DOS SISTEMAS
    // ================================

    void ColetarDados()
    {
        dadosAtuais.dataHora  = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        dadosAtuais.slot      = SlotAtivo;
        dadosAtuais.cenaAtual = SceneManager.GetActiveScene().name;

        // posição do player
        if (PlayerController.Instance != null)
        {
            dadosAtuais.playerX = PlayerController.Instance.transform.position.x;
            dadosAtuais.playerY = PlayerController.Instance.transform.position.y;
        }

        // inventário
        dadosAtuais.itensSalvos.Clear();
        if (InventorySystem.Instance != null)
        {
            for (int i = 0; i < InventorySystem.Instance.ItemCount; i++)
            {
                var img = InventorySystem.Instance.GetSlotImage(i);
                // usa o nome do sprite como identificador do item
                if (img != null && img.sprite != null)
                    dadosAtuais.itensSalvos.Add(img.sprite.name);
            }
        }

        // gerador
        var gen = FindFirstObjectByType<Generator>();
        if (gen != null)
        {
            dadosAtuais.geradorLigado    = gen.IsOn;
            dadosAtuais.chaveInstalada   = gen.ChaveInstalada;
            dadosAtuais.bateriaInstalada = gen.BateriaInstalada;
        }

        // puzzle armários
        if (CabinetPuzzleManager.Instance != null)
            dadosAtuais.puzzleArmarioResolvido = CabinetPuzzleManager.Instance.IsSolved();
    }

    // ================================
    // APLICAR DADOS NA CENA
    // ================================

    void AplicarDados()
    {
        // posição do player
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.transform.position =
                new Vector3(dadosAtuais.playerX, dadosAtuais.playerY, 0f);
        }

        // itens do inventário — restaura via nome do ScriptableObject
        if (InventorySystem.Instance != null)
        {
            foreach (string nomeItem in dadosAtuais.itensSalvos)
            {
                ItemData item = Resources.Load<ItemData>("Items/" + nomeItem);
                if (item != null)
                    InventorySystem.Instance.AddItem(item);
                else
                    Debug.LogWarning($"[SaveSystem] Item não encontrado: {nomeItem}");
            }
        }

        // notifica outros sistemas
        OnSaveLoaded?.Invoke();

        Debug.Log("[SaveSystem] Dados aplicados na cena");
    }

    // ================================
    // GETTER DOS DADOS (para outros sistemas lerem)
    // ================================

    public SaveData GetDados() => dadosAtuais;
}
