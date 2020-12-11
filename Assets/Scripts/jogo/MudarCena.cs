using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarCena : MonoBehaviour 
{
	// Objetos
	private EfeitoFade efeitoFade;
	private GameController gameController;

	// Variaveis de controle
	public string cenaDestino;

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa 
	private void Start () 
	{
		efeitoFade = FindObjectOfType<EfeitoFade>();
		gameController = FindObjectOfType<GameController>();
	}

	// ------------------- FUNCOES ------------------- //

	public void Interact ()
	{
		StartCoroutine ("MudancaCena");
	}

	// ------------------- CORROTINAS ------------------- //
	
	// Aplica efeito fade-in e muda cena
	private IEnumerator MudancaCena ()
	{
		// Parametros e efeitos
		efeitoFade.FadeIn ();
		yield return new WaitWhile (() => efeitoFade.fumeImage.color.a < 0.9f);

		if (cenaDestino.Equals ("Titulo"))
		{
			Destroy (gameController.gameObject);
		}

		SceneManager.LoadScene (cenaDestino);
	}
}