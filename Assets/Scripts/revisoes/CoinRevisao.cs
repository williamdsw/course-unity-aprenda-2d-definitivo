using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRevisao : MonoBehaviour 
{
	private int coinValue;
	private GameControllerRevisao gameControllerRevisao;

	// GETTER / SETTER
	public int CoinValue
	{
		get { return coinValue; }
		set { coinValue = value; }
	}

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa instancias
	private void Start () 
	{
		gameControllerRevisao = FindObjectOfType<GameControllerRevisao> ();
	}

	// ------------------- FUNCOES ------------------- //

	// Incrementa e destroi objeto
	public void Coletar ()
	{
		gameControllerRevisao.NumberGold += coinValue;
		Destroy (this.gameObject);
	}
}