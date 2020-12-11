using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortaRevisao : MonoBehaviour 
{
	// Variaveis de controle
	public bool isOnDark;

	// Variaveis de objetos / components
	private PlayerRevisao playerRevisao;
	private EfeitoFadeRevisao efeitoFadeRevisao;
	public Transform destination;
	public Material lightMaterial;
	public Material defaultMaterial;
	private AudioControllerRevisao audioControllerRevisao;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		playerRevisao = FindObjectOfType<PlayerRevisao> ();
		efeitoFadeRevisao = FindObjectOfType (typeof (EfeitoFadeRevisao)) as EfeitoFadeRevisao;
		audioControllerRevisao = FindObjectOfType (typeof (AudioControllerRevisao)) as AudioControllerRevisao;
	}

	// ------------------- FUNCOES ------------------- //

	public void Interact ()
	{
		StartCoroutine ("ActivateDoor");
	}

	// ------------------- CORROTINAS ------------------- //

	private IEnumerator ActivateDoor ()
	{
		// Efeito fade in
		efeitoFadeRevisao.FadeIn ();

		audioControllerRevisao.PlayFX (audioControllerRevisao.fxDoor, 1f);

		// Configuracoes
		yield return new WaitWhile (() => efeitoFadeRevisao.blackoutImage.color.a < 0.9f);
		playerRevisao.gameObject.SetActive (false);
		Material material = (isOnDark ? lightMaterial : defaultMaterial);
		playerRevisao.ChangeMaterial (material);
		playerRevisao.transform.position = destination.position;
		playerRevisao.gameObject.SetActive (false);

		// Efeito fade out
		efeitoFadeRevisao.FadeOut ();
	}
}