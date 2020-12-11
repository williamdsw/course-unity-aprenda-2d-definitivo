using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EfeitoFade : MonoBehaviour 
{
	// Variaveis de controle
	public float step;
	public bool emTransicao;

	// Objetos 
	public GameObject painelFume;
	public Image fumeImage;
	public Color[] corTransicao;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		StartCoroutine ("FadeOutCorroutine");
	}

	// ------------------- FUNCOES ------------------- //

	// Chama corrotina de fade in
	public void FadeIn ()
	{
		if (!emTransicao)
		{
			StartCoroutine ("FadeInCorroutine");
		}
	}

	// Chama corrotina de fade out
	public void FadeOut ()
	{
		StartCoroutine ("FadeOutCorroutine");
	}

	// ------------------- CORROTINAS ------------------- //

	// Muda o alpha da imagem de 0.1 para 1.0
	private IEnumerator FadeInCorroutine ()
	{
		emTransicao = true;
		painelFume.SetActive (true);

		for (float i = 0; i <= 1; i += step)
		{
			// Lerp = Interpola entre uma cor e outra
			fumeImage.color = Color.Lerp (corTransicao[0], corTransicao[1], i);
			yield return new WaitForEndOfFrame ();	
		}
	}

	// Muda o alpha da imagem de 1.0 para 0.1
	private IEnumerator FadeOutCorroutine ()
	{
		yield return new WaitForSeconds (0.5f);
		
		for (float i = 0; i <= 1; i += step)
		{
			// Lerp = Interpola entre uma cor e outra
			fumeImage.color = Color.Lerp (corTransicao[1], corTransicao[0], i);
			yield return new WaitForEndOfFrame ();	
		}

		painelFume.SetActive (false);
		emTransicao = false;
	}
}