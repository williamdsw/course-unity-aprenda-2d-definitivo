using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour 
{
	public int valor;
	private GameController gameController;

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa instancias
	private void Start ()
	{
		gameController = FindObjectOfType<GameController>();
	}

	// ------------------- FUNCOES ------------------- //

	// Incrementa e destroi objeto
	public void Coletar ()
	{
		gameController.gold += valor;
		Destroy (this.gameObject);
	}
}