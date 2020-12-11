using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagemFlecha : MonoBehaviour 
{
	// Components
	private SpriteRenderer spriteRenderer;

	// Objetos
	private GameController gameController;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		gameController = FindObjectOfType (typeof (GameController)) as GameController;
	}

	private void Update () 
	{
		// Atualiza sprite
		spriteRenderer.sprite = gameController.imagensFlecha[gameController.idFlechaEquipada];
	}
}