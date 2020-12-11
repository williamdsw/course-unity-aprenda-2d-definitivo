using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour 
{
	// Objetos
	private GameController gameController;
	private Player player;

	[Header ("Imagens da barra de vida")]
	public Image[] barrasDeVidaImage;
	public Sprite vidaMetadeSprite;
	public Sprite vidaMaximaSprite;

	[Header ("Imagens da barra de mana")]
	public Image[] barrasDeManaImage;
	public Sprite manaMetadeSprite;
	public Sprite manaMaximaSprite;

	[Header ("Paineis, Imagem e Texto da Flecha")]
	public GameObject painelMana;
	public GameObject painelFlecha;
	public Image flechaIcone;
	public TMP_Text quantidadeFlechasText;

	[Header ("Caixas e Textos das poções de HP e MP")]
	public GameObject boxHP;
	public GameObject boxMP;
	public TMP_Text quantidadeBoxHPtext;
	public TMP_Text quantidadeBoxMPtext;
	public RectTransform boxARectTransform;
	public RectTransform boxBRectTransform;
	public Vector2 boxAPosition;
	public Vector2 boxBPosition;

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa
	private void Start () 
	{
		gameController = FindObjectOfType (typeof (GameController)) as GameController;
		player = FindObjectOfType (typeof (Player)) as Player;

		// Desativa paineis
		painelMana.SetActive (false);
		painelFlecha.SetActive (false);
		boxHP.SetActive (false);
		boxMP.SetActive (false);

		VerificarHUDPersonagem ();

		// Posiciona caixas
		boxAPosition = boxARectTransform.anchoredPosition;
		boxBPosition = boxBRectTransform.anchoredPosition;
	}
	
	private void Update () 
	{
		ControleBarraVida ();
		PosicaoCaixaPocoes ();

		// Controla barra de mana ou quantidade de flechas
		if (painelMana.activeSelf)
		{
			ControleBarraMana ();	
		}
		else if (painelFlecha.activeSelf)
		{
			// Faz troca dos tipos de flechas
			if (Input.GetButtonDown ("ButtonL"))
			{
				if (gameController.idFlechaEquipada == 0)
				{
					gameController.idFlechaEquipada = (gameController.iconesFlecha.Length - 1);
				}
				else 
				{
					gameController.idFlechaEquipada--;
				}
			}
			else if (Input.GetButtonDown ("ButtonR"))
			{
				if (gameController.idFlechaEquipada == (gameController.iconesFlecha.Length - 1))
				{
					gameController.idFlechaEquipada = 0;
				}
				else 
				{
					gameController.idFlechaEquipada++;
				}
			}

			flechaIcone.sprite = gameController.iconesFlecha[gameController.idFlechaEquipada];
			quantidadeFlechasText.text = string.Concat ("x", gameController.quantidadeFlechas[gameController.idFlechaEquipada]);
		}

		// Define quantidades na UI
		quantidadeBoxHPtext.text = gameController.quantidadePocoes[0].ToString ();
		quantidadeBoxMPtext.text = gameController.quantidadePocoes[1].ToString ();
	}

	// ------------------- FUNCOES ------------------- //

	// Faz o controle dos sprites de vida
	private void ControleBarraVida ()
	{
		// Calcula o percentual da vida 0 - 1
		float percentualVida = ((float) gameController.vidaAtual / (float) gameController.vidaMaxima);

		// Ativa pocao de cura
		if (Input.GetButtonDown ("ItemA") && percentualVida < 1)
		{
			gameController.UsarPocao (0);
		}

		// Representa 100% de vida
		foreach (Image imagem in barrasDeVidaImage)
		{
			imagem.enabled = true;
			imagem.sprite = vidaMaximaSprite;
		}

		// Controla sprites
		if (percentualVida == 1)
		{}
		else if (percentualVida >= 0.9f)
		{
			barrasDeVidaImage[4].sprite = vidaMetadeSprite;
		}
		else if (percentualVida >= 0.8f)
		{
			barrasDeVidaImage[4].enabled = false;
		}
		else if (percentualVida >= 0.7f)
		{
			barrasDeVidaImage[4].enabled = false;
			barrasDeVidaImage[3].sprite = vidaMetadeSprite;
		}
		else if (percentualVida >= 0.6f)
		{
			barrasDeVidaImage[4].enabled = false;
			barrasDeVidaImage[3].enabled = false;
		}
		else if (percentualVida >= 0.5f)
		{
			barrasDeVidaImage[4].enabled = false;
			barrasDeVidaImage[3].enabled = false;
			barrasDeVidaImage[2].sprite = vidaMetadeSprite;
		}
		else if (percentualVida >= 0.4f)
		{
			barrasDeVidaImage[4].enabled = false;
			barrasDeVidaImage[3].enabled = false;
			barrasDeVidaImage[2].enabled = false;
		}
		else if (percentualVida >= 0.3f)
		{
			barrasDeVidaImage[4].enabled = false;
			barrasDeVidaImage[3].enabled = false;
			barrasDeVidaImage[2].enabled = false;
			barrasDeVidaImage[1].sprite = vidaMetadeSprite;
		}
		else if (percentualVida >= 0.2f)
		{
			barrasDeVidaImage[4].enabled = false;
			barrasDeVidaImage[3].enabled = false;
			barrasDeVidaImage[2].enabled = false;
			barrasDeVidaImage[1].enabled = false;
		}
		else if (percentualVida >= 0.01f)
		{
			barrasDeVidaImage[4].enabled = false;
			barrasDeVidaImage[3].enabled = false;
			barrasDeVidaImage[2].enabled = false;
			barrasDeVidaImage[1].enabled = false;
			barrasDeVidaImage[0].sprite = vidaMetadeSprite;
		}
		else if (percentualVida <= 0)
		{
			barrasDeVidaImage[4].enabled = false;
			barrasDeVidaImage[3].enabled = false;
			barrasDeVidaImage[2].enabled = false;
			barrasDeVidaImage[1].enabled = false;
			barrasDeVidaImage[0].enabled = false;
		}

		// Mostra barra caso haja pocoes
		boxHP.SetActive (gameController.quantidadePocoes[0] > 0);
	}

	// Faz o controle dos sprites de mana
	private void ControleBarraMana ()
	{
		// Calcula o percentual da mana 0 - 1
		float percentualMana = ((float) gameController.manaAtual / (float) gameController.manaMaxima);

		// Ativa pocao de mana
		if (Input.GetButtonDown ("ItemB") && percentualMana < 1)
		{
			gameController.UsarPocao (1);
		}

		// Representa 100% de mana
		foreach (Image imagem in barrasDeManaImage)
		{
			imagem.enabled = true;
			imagem.sprite = manaMaximaSprite;
		}

		// Controla sprites
		if (percentualMana == 1)
		{}
		else if (percentualMana >= 0.9f)
		{
			barrasDeManaImage[4].sprite = manaMetadeSprite;
		}
		else if (percentualMana >= 0.8f)
		{
			barrasDeManaImage[4].enabled = false;
		}
		else if (percentualMana >= 0.7f)
		{
			barrasDeManaImage[4].enabled = false;
			barrasDeManaImage[3].sprite = manaMetadeSprite;
		}
		else if (percentualMana >= 0.6f)
		{
			barrasDeManaImage[4].enabled = false;
			barrasDeManaImage[3].enabled = false;
		}
		else if (percentualMana >= 0.5f)
		{
			barrasDeManaImage[4].enabled = false;
			barrasDeManaImage[3].enabled = false;
			barrasDeManaImage[2].sprite = manaMetadeSprite;
		}
		else if (percentualMana >= 0.4f)
		{
			barrasDeManaImage[4].enabled = false;
			barrasDeManaImage[3].enabled = false;
			barrasDeManaImage[2].enabled = false;
		}
		else if (percentualMana >= 0.3f)
		{
			barrasDeManaImage[4].enabled = false;
			barrasDeManaImage[3].enabled = false;
			barrasDeManaImage[2].enabled = false;
			barrasDeManaImage[1].sprite = manaMetadeSprite;
		}
		else if (percentualMana >= 0.2f)
		{
			barrasDeManaImage[4].enabled = false;
			barrasDeManaImage[3].enabled = false;
			barrasDeManaImage[2].enabled = false;
			barrasDeManaImage[1].enabled = false;
		}
		else if (percentualMana >= 0.01f)
		{
			barrasDeManaImage[4].enabled = false;
			barrasDeManaImage[3].enabled = false;
			barrasDeManaImage[2].enabled = false;
			barrasDeManaImage[1].enabled = false;
			barrasDeManaImage[0].sprite = manaMetadeSprite;
		}
		else if (percentualMana <= 0)
		{
			barrasDeManaImage[4].enabled = false;
			barrasDeManaImage[3].enabled = false;
			barrasDeManaImage[2].enabled = false;
			barrasDeManaImage[1].enabled = false;
			barrasDeManaImage[0].enabled = false;
		}

		// Mostra barra caso haja pocoes
		boxMP.SetActive (gameController.quantidadePocoes[1] > 0);
	}

	// Posiciona as caixas indicativas de pocoes de acordo com as quantidades das mesmas
	public void PosicaoCaixaPocoes ()
	{
		if (gameController.quantidadePocoes[0] > 0)
		{
			boxHP.GetComponent<RectTransform>().anchoredPosition = boxAPosition;
			boxMP.GetComponent<RectTransform>().anchoredPosition = boxBPosition;
		}
		else 
		{
			boxHP.GetComponent<RectTransform>().anchoredPosition = boxBPosition;
			boxMP.GetComponent<RectTransform>().anchoredPosition = boxAPosition;
		}
	}

	// Verifica e esconde elementos de acordo com a classe do personagem
	public void VerificarHUDPersonagem ()
	{
		// Ativa de acordo com classe (tipo) de personagem
		if (gameController.idClasse[gameController.playerID] == 1)
		{
			flechaIcone.sprite = gameController.iconesFlecha[gameController.idFlechaEquipada];
			painelFlecha.SetActive (true);
		} 
		else if (gameController.idClasse[gameController.playerID] == 2)
		{
			painelMana.SetActive (true);
		}
	}
}
