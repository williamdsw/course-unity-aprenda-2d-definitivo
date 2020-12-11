using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EfeitoFadeRevisao : MonoBehaviour 
{
	// Variaveis de controle
	public float step;
	public bool isFading;

	// Objetos 
	public GameObject blackoutPanel;
	public Image blackoutImage;
	public Color[] transitionColors;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		StartCoroutine ("FadeOutCoroutine");
	}

	// ------------------- FUNCOES ------------------- //

	// Chama corrotina de fade in
	public void FadeIn ()
	{
		if (!isFading)
		{
			StartCoroutine ("FadeInCoroutine");	
		}
	}

	// Chama corrotina de fade out
	public void FadeOut ()
	{
		StartCoroutine ("FadeOutCoroutine");
	}

	// ------------------- CORROTINAS ------------------- //

	// Muda o alpha da imagem de 0.1 para 1.0
	private IEnumerator FadeInCoroutine ()
	{
		isFading = true;
		blackoutPanel.SetActive (true);

		for (float i = 0; i <= 1; i++)
		{
			blackoutImage.color = Color.Lerp (transitionColors[0], transitionColors[1], i);
			yield return new WaitForEndOfFrame ();
		}
	}

	// Muda o alpha da imagem de 1 para 0.1
	private IEnumerator FadeOutCoroutine ()
	{
		yield return new WaitForSeconds (0.5f);

		for (float i = 0; i <= 1; i++)
		{
			blackoutImage.color = Color.Lerp (transitionColors[1], transitionColors[0], i);
			yield return new WaitForEndOfFrame ();
		}

		blackoutPanel.SetActive (false);
		isFading = false;
	}
}