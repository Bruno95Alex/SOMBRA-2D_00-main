using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    private const string SAVE_KEY  = "SOMBRA_Save_";
    private const int    MAX_SLOTS = 3;

    private SaveData dadosAtuais = new SaveData();

    public int SlotAtivo { get; private set; } = 0;

    // permite o SlotSelectUI definir o slot ativo para novo jogo
    public int SlotAtivoPublico { set { SlotAtivo = value; } }

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

        ColetarDados();

        string json = JsonUtility.ToJson(dadosAtuais, true);
        PlayerPrefs.SetString(SAVE_KEY + SlotAtivo, json);
        PlayerPrefs.SetInt("SaveExists_"  + SlotAtivo, 1);
        PlayerPrefs.SetString("SaveDate_" + SlotAtivo,
            System.DateTime.Now.ToString("dd/MM/yy HH:mm"));
        PlayerPrefs.Save();

        Debug.Log($"[SaveSystem] Salvo no slot {SlotAtivo}: {json}");
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
        Debug.Log($"[SaveSystem] Carregando slot {slot}: {json}");

        dadosAtuais = JsonUtility.FromJson<SaveData>(json);
        SlotAtivo   = slot;

        SceneManager.LoadScene(dadosAtuais.cenaAtual);
        SceneManager.sceneLoaded += AplicarAoCarregar;

        return true;
    }

    void AplicarAoCarregar(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= AplicarAoCarregar;
        // pequeno delay para a cena inicializar completamente
        StartCoroutine(AplicarComDelay());
    }

    System.Collections.IEnumerator AplicarComDelay()
    {
        yield return null;
        yield return null;
        AplicarDados();
    }

    // ================================
    // VERIFICAR
    // ================================

    public bool   TemSave(int slot)       => PlayerPrefs.HasKey(SAVE_KEY + slot);
    public string DataDoSave(int slot)    => PlayerPrefs.GetString("SaveDate_" + slot, "");
    public bool   TemQualquerSave()
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

    public void DeletarSave(int slot)
    {
        PlayerPrefs.DeleteKey(SAVE_KEY + slot);
        PlayerPrefs.DeleteKey("SaveExists_" + slot);
        PlayerPrefs.DeleteKey("SaveDate_"   + slot);
        PlayerPrefs.Save();
    }

    // ================================
    // COLETAR DADOS
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

        // inventário — salva o nome do ScriptableObject (não do sprite)
        dadosAtuais.itensSalvos.Clear();

        if (InventorySystem.Instance != null)
        {
            var itens = InventorySystem.Instance.GetAllItems();
            foreach (var item in itens)
            {
                if (item != null)
                {
                    dadosAtuais.itensSalvos.Add(item.name);
                    Debug.Log($"[SaveSystem] Salvando item: {item.name}");
                }
            }
        }

        // gerador
        var gen = FindFirstObjectByType<Generator>();
        if (gen != null)
        {
            dadosAtuais.geradorLigado      = gen.IsOn;
            dadosAtuais.chaveInstalada     = gen.ChaveInstalada;
            dadosAtuais.bateriaInstalada   = gen.BateriaInstalada;
        }

        // puzzle armários
        if (CabinetPuzzleManager.Instance != null)
            dadosAtuais.puzzleArmarioResolvido = CabinetPuzzleManager.Instance.IsSolved();
    }

    // ================================
    // APLICAR DADOS
    // ================================

    void AplicarDados()
    {
        Debug.Log($"[SaveSystem] Aplicando dados — itens: {dadosAtuais.itensSalvos.Count}");

        // posição do player
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.transform.position =
                new Vector3(dadosAtuais.playerX, dadosAtuais.playerY, 0f);
        }

        // inventário — limpa primeiro para não duplicar
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.ClearInventory();

            foreach (string nomeItem in dadosAtuais.itensSalvos)
            {
                // tenta carregar direto
                ItemData item = Resources.Load<ItemData>("Items/" + nomeItem);

                // tenta na subpasta Diario
                if (item == null)
                    item = Resources.Load<ItemData>("Items/Diario/" + nomeItem);

                if (item != null)
                {
                    InventorySystem.Instance.AddItem(item);
                    Debug.Log($"[SaveSystem] Item restaurado: {nomeItem}");
                }
                else
                {
                    Debug.LogError($"[SaveSystem] Item NÃO encontrado em Resources: {nomeItem}");
                }
            }
        }

        OnSaveLoaded?.Invoke();
        Debug.Log("[SaveSystem] Dados aplicados com sucesso");
    }

    public SaveData GetDados() => dadosAtuais;
}
