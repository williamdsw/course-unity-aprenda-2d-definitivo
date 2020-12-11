using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadExemploRevisao : MonoBehaviour
{
    // Valores a serem salvos ou exibidos
    private int playerID = 1;
    private string playerName = "My Player";
    private int score = 135;
    private string[] letters = new string[5] {"A", "B", "C", "D", "E"};

    // ------------------- FUNCOES UNITY ------------------- //

    // Update is called once per frame
    private void Update () 
    {
        // Salva
        if (Input.GetKeyDown (KeyCode.S))
        {
            print (string.Concat ("Saved in...", Application.persistentDataPath));
            SaveData ();
        }

        // Carrega
        if (Input.GetKeyDown (KeyCode.L))
        {
            print ("Data loaded...");
            LoadData ();
        }
    }

    // ------------------- FUNCOES ------------------- //

    // Salva o arquivo
    public void SaveData ()
    {
        string path = string.Concat (Application.persistentDataPath, "/savegame.dat");

        // Cria arquivo e dados
        using (FileStream fileStream = File.Create (path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter ();

            // Passa dados para classe
            DadosJogadorExemploRevisao dadosJogadorExemploRevisao = new DadosJogadorExemploRevisao ();
            dadosJogadorExemploRevisao.PlayerID = this.playerID;
            dadosJogadorExemploRevisao.PlayerName = this.playerName;
            dadosJogadorExemploRevisao.Score = this.score;
            dadosJogadorExemploRevisao.Letters = this.letters;

            // Serializa dados e fecha o arquivo
            binaryFormatter.Serialize (fileStream, dadosJogadorExemploRevisao);
        }
    }

    // Carrega o arquivo
    public void LoadData ()
    {
        string path = string.Concat (Application.persistentDataPath, "/savegame.dat");

        // Verifica se arquivo existe
        if (File.Exists (path))
        {
            // Carrega arquivo
            using (FileStream fileStream = File.Open (path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter ();

                // Carrega dados
                DadosJogadorExemploRevisao dadosJogadorExemploRevisao = (DadosJogadorExemploRevisao) binaryFormatter.Deserialize (fileStream);
                this.playerID = dadosJogadorExemploRevisao.PlayerID;
                this.playerName = dadosJogadorExemploRevisao.PlayerName;
                this.score = dadosJogadorExemploRevisao.Score;
                this.letters = dadosJogadorExemploRevisao.Letters;
            }
        }
    }
}

// Necessario ter uma classe
// "Serializable" = Permite que uma classe seja serializada para dados binarios
[Serializable]
class DadosJogadorExemploRevisao
{
    private int playerID;
    private string playerName;
    private int score;
    private string[] letters;

    // PROPERTIES 

    public int PlayerID
    {
        get { return this.playerID; }
        set { this.playerID = value; }
    }

    public string PlayerName
    {
        get { return this.playerName; }
        set { this.playerName = value; }
    }

    public int Score
    {
        get { return this.score; }
        set { this.score = value; }
    }

    public string[] Letters
    {
        get { return this.letters; }
        set { this.letters = value; }
    }
}