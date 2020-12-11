using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour 
{
	public Transform backgroundTransform;
	public float escalaParallax;
	public float velocidade;

	private Transform cameraTransform;
	private Vector3 posicaoAnteriorCamera;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		cameraTransform = Camera.main.transform;

		// Armazena posicao da camera
		posicaoAnteriorCamera = cameraTransform.position;
	}

	private void LateUpdate () 
	{
		// Calcula efeito parallax em X
		float efeitoParallaxX = (posicaoAnteriorCamera.x - cameraTransform.position.x) * escalaParallax;
		float backgroundDestinoX = (backgroundTransform.position.x + efeitoParallaxX);

		// Define nova posicao
		Vector3 posicaoBackground = new Vector3 (backgroundDestinoX, backgroundTransform.position.y, backgroundTransform.position.z);

		// Faz a animacao de uma posicao ate outra
		// "Time.deltaTime" = Quanto tempo se passou de um frame para o outro
		backgroundTransform.position = Vector3.Lerp (backgroundTransform.position, posicaoBackground, velocidade * Time.deltaTime);

		// Armazena posicao da camera
		posicaoAnteriorCamera = cameraTransform.position;
	}
}