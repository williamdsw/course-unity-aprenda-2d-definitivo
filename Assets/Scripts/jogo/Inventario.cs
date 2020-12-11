using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventario : MonoBehaviour 
{
	[Header ("Propriedades dos Slots")]
	public Button[] slots;
	public Image[] iconItems;

	[Header ("Textos das quantidades")]
	public TextMeshProUGUI quantidadePocaoText;
	public TextMeshProUGUI quantidadeManaText;
	public TextMeshProUGUI quantidadeFlechaNormalText;
	public TextMeshProUGUI quantidadeFlechaAzulText;
	public TextMeshProUGUI quantidadeFlechaAmarelaText;

	[Header ("Quantidades de cada item")]
	public int quantidadePocao;
	public int quantidadeMana;
	public int quantidadeFlechaNormal;
	public int quantidadeFlechaAzul;
	public int quantidadeFlechaAmarela;

	// Components / Objetos
	private GameController gameController;
	public List<GameObject> itensInventario;
	public List<GameObject> itensCarregados;

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa
	private void Start () 
	{
		gameController = FindObjectOfType (typeof (GameController)) as GameController;
	}

	// ------------------- FUNCOES ------------------- //

	public void CarregarInventario ()
	{
		// Desabilita botoes
		foreach (Button slot in slots)
		{
			slot.interactable = false;
		}

		// Reseta sprite e desabilita objeto
		foreach (Image icon in iconItems)
		{
			icon.sprite = null;
			icon.gameObject.SetActive (false);
		}

		// Define texto padrao
		quantidadePocaoText.text = string.Concat ("x", gameController.quantidadePocoes[0]);
		quantidadeManaText.text = string.Concat ("x", gameController.quantidadePocoes[1]);
		quantidadeFlechaNormalText.text = string.Concat ("x", gameController.quantidadeFlechas[0]);
		quantidadeFlechaAzulText.text = string.Concat ("x", gameController.quantidadeFlechas[1]);
		quantidadeFlechaAmarelaText.text = string.Concat ("x", gameController.quantidadeFlechas[2]);

		LimparItensCarregados ();

		// Adiciona objetos e faz o botao ser clicavel
		int index = 0;
		foreach (GameObject item in itensInventario)
		{
			GameObject itemTemp = Instantiate (item);
			Item itemInfo = itemTemp.GetComponent<Item>();
			itensCarregados.Add (itemTemp);

			// Button Slot
			slots[index].GetComponent<SlotInventario>().objetoSlot = itemTemp;
			slots[index].interactable = true;

			// Image Slot
			iconItems[index].sprite = gameController.imagensInventario[itemInfo.itemID];
			iconItems[index].gameObject.SetActive (true);
			index++;
		}
	}

	// Destroi objetos e limpa a lista
	public void LimparItensCarregados ()
	{
		foreach (GameObject item in itensCarregados)
		{
			Destroy (item);
		}

		itensCarregados.Clear ();
	}
}