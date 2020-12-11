using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PainelItemInfo : MonoBehaviour 
{
	// Components / Objetos
	private GameController gameController;
	public GameObject objetoSlot;
	public GameObject[] aprimoramentos;

	// Variaveis de controle
	public int idSlot;
	private int aprimoramento;
	private int idArma;

	[Header ("Configuração HUD")]
	public Image imagemItem;
	public TMP_Text nomeItemText;
	public TMP_Text danoArmaText;

	[Header ("Botões")]
	public Button btnAprimorar;
	public Button btnEquipar;
	public Button btnExcluir;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		gameController = FindObjectOfType (typeof (GameController)) as GameController;
		DefinirEventos ();
	}

	// ------------------- FUNCOES ------------------- //

	// Define eventos dos botoes
	private void DefinirEventos ()
	{
		if (btnAprimorar != null)
		{
			btnAprimorar.onClick.AddListener (AprimorarItem);
		}

		if (btnEquipar != null)
		{
			btnEquipar.onClick.AddListener (EquiparItem);
		}

		if (btnExcluir != null)
		{
			btnExcluir.onClick.AddListener (DescartarItem);
		}
	}

	public void CarregarInformacoesItem ()
	{
		// Recupera informacoes
		Item item = objetoSlot.GetComponent<Item>();
		idArma = item.itemID;
		string tipoDano = gameController.tiposDano[gameController.tipoDanosArma[idArma]];
		int danoMinimo = gameController.danosMinimo[idArma];
		int danoMaximo = gameController.danosMaximo[idArma];
		int aprimoramento = gameController.aprimoramentoArmas[idArma];

		// Passa informacoes para UI
		nomeItemText.text = gameController.nomeArmas[idArma];
		imagemItem.sprite = gameController.imagensInventario[idArma];
		danoArmaText.text = string.Concat ("Dano: ", danoMinimo, " - ", danoMaximo, " / ", tipoDano);

		CarregarAprimoramento ();

		// Define controle do primeiro slot
		if (idSlot == 0)
		{
			btnEquipar.interactable = false;
			btnExcluir.interactable = false;
		}
		else 
		{
			// Define se o usuario pode equipar um item pela classe
			int idClasseArma = gameController.idClasseArma[idArma];
			int idClassePersonagem = gameController.idClasse[gameController.playerID];

			if (idClasseArma == idClassePersonagem)
			{
				btnEquipar.interactable = true;
				btnExcluir.interactable = true;
			}
			else 
			{
				btnEquipar.interactable = false;
				btnExcluir.interactable = false;
			}
		}
	}

	// Aprimora valor da arma
	public void AprimorarItem ()
	{
		gameController.TocarClick ();
		gameController.AprimorarArma (idArma);
		CarregarAprimoramento ();
	}

	// Equipa item e volta a gameplay
	public void EquiparItem ()
	{
		gameController.TocarClick ();
		objetoSlot.SendMessage ("UsarItem", SendMessageOptions.DontRequireReceiver);
		gameController.TrocarItens (idSlot);
	}

	// Chama descarte do item
	public void DescartarItem ()
	{
		gameController.TocarClick ();
		gameController.ExcluirItem (idSlot);
	}

	// Habilita botao de aprimoramento e reseta as barras de aprimoramentos
	public void CarregarAprimoramento ()
	{
		aprimoramento = gameController.aprimoramentoArmas[idArma];

		if (aprimoramento >= 10)
		{
			btnAprimorar.interactable = false;
		}
		else 
		{
			btnAprimorar.interactable = true;
		}

		// Desativa objetos por padrao
		foreach (GameObject objeto in aprimoramentos)
		{
			objeto.SetActive (false);
		}

		// Ativando objetos pelo aprimoramento
		for (int i = 0; i < aprimoramento; i++)
		{
			aprimoramentos[i].SetActive (true);
		}
	}
}