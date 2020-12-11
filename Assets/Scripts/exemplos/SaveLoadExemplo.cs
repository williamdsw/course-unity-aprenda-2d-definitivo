using System;											//
using System.Collections;
using System.Collections.Generic;
using System.IO;										//
using System.Runtime.Serialization.Formatters.Binary;	//
using UnityEngine;

public class SaveLoadExemplo : MonoBehaviour 
{
	// Valores a serem salvos ou exibidos
	public int idPersonagem;
	public string nomePersonagem;
	public int pontuacao;
	public string[] letras;

	// ------------------- FUNCOES UNITY ------------------- //

	// Update is called once per frame
	private void Update () 
	{
		// Salva
		if (Input.GetKeyDown (KeyCode.S))
		{
			print ("Jogo Salvo");
			print ("Pasta do save: " + Application.persistentDataPath);
			Salvar ();
		}

		// Carrega
		if (Input.GetKeyDown (KeyCode.L))
		{
			print ("Dados carregados");
			Carregar ();
		}
	}

	// ------------------- FUNCOES ------------------- //

	// Salva o arquivo
	public void Salvar ()
	{
		// Cria arquivo e formatador binario
		string url = string.Concat (Application.persistentDataPath, "/savegame.dat");
		BinaryFormatter binaryFormater = new BinaryFormatter ();
		FileStream fileStream = File.Create (url);

		// Passa dados para classe
		DadosJogadorExemplo dados = new DadosJogadorExemplo ();
		dados.idPersonagem = this.idPersonagem;
		dados.nomePersonagem = this.nomePersonagem;
		dados.pontuacao = this.pontuacao;
		dados.letras = this.letras;

		// Serializa dados e fecha o arquivo
		binaryFormater.Serialize (fileStream, dados);
		fileStream.Close ();
	}

	// Carrega o arquivo
	public void Carregar ()
	{
		string url = string.Concat (Application.persistentDataPath, "/savegame.dat");

		// Verifica se arquivo existe
		if (File.Exists (url))
		{
			// Carrega arquivo
			BinaryFormatter binaryFormatter = new BinaryFormatter ();
			FileStream fileStream = File.Open (url, FileMode.Open);

			// Carrega dados
			DadosJogadorExemplo dados = (DadosJogadorExemplo) binaryFormatter.Deserialize (fileStream);
			this.idPersonagem = dados.idPersonagem;
			this.nomePersonagem = dados.nomePersonagem;
			this.pontuacao = dados.pontuacao;
			this.letras = dados.letras;

			fileStream.Close ();
		}
	}
}

// Necessario ter uma classe
// "Serializable" = Permite que uma classe seja serializada para dados binarios
[Serializable]
class DadosJogadorExemplo
{
	public int idPersonagem;
	public string nomePersonagem;
	public int pontuacao;
	public string[] letras;
}
