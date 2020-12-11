using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour 
{
	// Variaveis de Controle
	public int itemID;

	// Variaveis de Objetos / Components
	private GameController gameController;

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa
	private void Start () 
	{
		gameController = FindObjectOfType<GameController>();	
	}

	// ------------------- FUNCOES ------------------- //

	// Usa item enviando o ID do proprio
	public void UsarItem ()
	{
		gameController.UsarItemArma (itemID);
	}
}
