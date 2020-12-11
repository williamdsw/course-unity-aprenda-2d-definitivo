using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porta : MonoBehaviour 
{
	// Variaveis de controle
	public bool estaNoEscuro;

	// Variaveis de objetos / components
	public Transform destino;
	private Player player;
	private EfeitoFade efeitoFade;
	public Material luz2D;
	public Material padrao2D;
	private AudioController audioController;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		efeitoFade = FindObjectOfType (typeof (EfeitoFade))	as EfeitoFade;
		player = FindObjectOfType (typeof (Player)) as Player;
		audioController = FindObjectOfType (typeof (AudioController)) as AudioController;
	}

	// ------------------- FUNCOES ------------------- //

	public void Interact ()
	{
		StartCoroutine ("AcionarPorta");
	}

	// ------------------- CORROTINAS ------------------- //

	private IEnumerator AcionarPorta ()
	{
		// Efeito fade in
		efeitoFade.FadeIn ();

		audioController.TocarEfeito (audioController.efeitoPorta, 1f);

		// Configuracoes
		// "WaitWhile (delegate)" = Vai esperar ate a condicao informada seja verdadeira
		yield return new WaitWhile (() => efeitoFade.fumeImage.color.a < 0.9f);
		player.gameObject.SetActive (false);
		Material material = (estaNoEscuro ? luz2D : padrao2D);
		player.MudarMaterial (material);
		player.transform.position = destino.position;
		player.gameObject.SetActive (true);

		// Efeito fade out
		efeitoFade.FadeOut ();
	}
}
