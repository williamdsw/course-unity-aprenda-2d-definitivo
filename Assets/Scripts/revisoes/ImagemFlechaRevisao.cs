using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagemFlechaRevisao : MonoBehaviour 
{
	// Components
	private SpriteRenderer spriteRenderer;

	// Objetos
	private GameControllerRevisao gameControllerRevisao;

	// ------------------- FUNCOES UNITY ------------------- //
	
	private void Start () 
	{
		// Inicializa
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		gameControllerRevisao = FindObjectOfType (typeof (GameControllerRevisao)) as GameControllerRevisao;
	}

	private void Update () 
	{
		// Atualiza sprite
		spriteRenderer.sprite = gameControllerRevisao.arrowImages[gameControllerRevisao.equippedArrowID];
	}
}