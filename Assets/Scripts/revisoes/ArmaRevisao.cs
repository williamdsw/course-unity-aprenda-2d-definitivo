using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaRevisao : MonoBehaviour 
{
	// Variaveis de controle
	private bool wasCollect;

	// Components - Objetos
	private GameControllerRevisao gameControllerRevisao;
	public List<GameObject> collectableItems;

	// Inicializa
	private void Start () 
	{
		gameControllerRevisao = FindObjectOfType<GameControllerRevisao>();	
	}

	// Coleta item e destroi objeto
	public void Collect ()
	{
		// Coleta um item aleatorio da lista e faz controle para nao colidir duas vezes
		if (!wasCollect)
		{
			wasCollect = true;
			int index = Random.Range (0, collectableItems.Count - 1);
			gameControllerRevisao.CollectItem (collectableItems[index]);
			Destroy (this.gameObject);
		}
	}
}