using System;
using System.Collections.Generic;

/// <summary>
/// Estrutura de dados do save — tudo que precisa ser salvo.
/// Serializada em JSON e salva com PlayerPrefs.
/// </summary>
[Serializable]
public class SaveData
{
    // ================================
    // LOCALIZAÇÃO
    // ================================
    public string cenaAtual      = "";
    public float  playerX        = 0f;
    public float  playerY        = 0f;

    // ================================
    // INVENTÁRIO
    // lista de nomes dos itens coletados
    // ================================
    public List<string> itensSalvos = new List<string>();

    // ================================
    // PUZZLES
    // ================================
    public bool   geradorLigado        = false;
    public bool   puzzleArmarioResolvido = false;
    public bool   chaveInstalada       = false;
    public bool   bateriaInstalada     = false;

    // ================================
    // ITENS COLETÁVEIS JÁ PEGOS
    // guarda o nome do GameObject para não reaparecer
    // ================================
    public List<string> coletaveisColetados = new List<string>();

    // ================================
    // PÁGINAS DO DIÁRIO JÁ LIDAS
    // ================================
    public List<string> paginasLidas = new List<string>();

    // ================================
    // PORTAS ABERTAS
    // ================================
    public List<string> portasAbertas = new List<string>();

    // ================================
    // META
    // ================================
    public string dataHora = "";
    public int    slot     = 0;
}
