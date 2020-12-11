using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : MonoBehaviour 
{
	// Variaveis de controle
	private bool foiColetado;

	// Components / Objetos
	private GameController gameController;
	public GameObject[] itensColetar;

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa instancias
	private void Start ()
	{
		gameController = FindObjectOfType<GameController>();
	}

	// ------------------- FUNCOES ------------------- //

	// Coleta item e destroi objeto
	public void Coletar ()
	{
		// Coleta um item aleatorio da lista e faz controle para nao colidir duas vezes
		if (!foiColetado)
		{
			foiColetado = true;
			int index = Random.Range (0, itensColetar.Length - 1);
			gameController.ColetarItem (itensColetar[index]);
			Destroy (this.gameObject);
		}
	}
}