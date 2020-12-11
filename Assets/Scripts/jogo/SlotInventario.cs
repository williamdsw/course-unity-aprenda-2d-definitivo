using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotInventario : MonoBehaviour 
{
	// Variaveis de controle
	public int idSlot;

	// Variaveis de controle
	private GameController gameController;
	private AudioController audioController;
	private PainelItemInfo painelItemInfo;
	public GameObject objetoSlot;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		gameController = FindObjectOfType<GameController>();
		audioController = FindObjectOfType<AudioController>();
		painelItemInfo = FindObjectOfType<PainelItemInfo>();

		// Botao do proprio slot 
		Button btn = this.GetComponent<Button>();
		btn.onClick.AddListener (delegate 
		{
			UsarItem ();
		});
	}

	// ------------------- FUNCOES ------------------- //

	// Passa informacoes e chama funcoes o item
	public void UsarItem ()
	{
		if (objetoSlot != null)
		{
			audioController.TocarEfeito (audioController.efeitoClick, 1f);
			painelItemInfo.objetoSlot = this.objetoSlot;
			painelItemInfo.idSlot = this.idSlot;
			painelItemInfo.CarregarInformacoesItem ();
			gameController.AbrirItemInfo ();
		}
	}
}