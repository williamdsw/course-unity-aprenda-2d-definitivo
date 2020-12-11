using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour 
{
	// Variaveis de controle
	public bool isOpen;
	public int quantidadeMinimaItens;
	public int quantidadeMaximaItens;

	// Components
	private SpriteRenderer spriteRenderer;
	private Collider2D myCollider2D;
	public Sprite[] imagensObjeto;

	// Outros objetos
	public GameObject[] loots;
	private AudioController audioController;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		myCollider2D = this.GetComponent<Collider2D> ();
		audioController = FindObjectOfType<AudioController>();
	}

	// ------------------- FUNCOES ------------------- //

	// Iteracao da caixa com o Personagem
	public void Interact () 
	{
		// Se abrir
		if (!isOpen) 
		{
			// Muda o sprite e chama corrotina
			isOpen = true;
			spriteRenderer.sprite = imagensObjeto[1];
			audioController.TocarEfeito (audioController.efeitoBau, audioController.volumeMaximoEfeitos);
			StartCoroutine ("GerarLoot");
			myCollider2D.enabled = false;
		} 
	}

	// ------------------- CORROTINAS ------------------- //

	// Gera 'n' loots quando abre o bau
	private IEnumerator GerarLoot ()
	{
		int quantidadeMoedas = Random.Range (quantidadeMinimaItens, quantidadeMaximaItens);

		for (int i = 0; i < quantidadeMoedas; i++)
		{
			// Calcula chance de vir uma moeda ou outra
			int randomChance = Random.Range (0, 100);
			int lootIndex = 0;

			if (loots.Length > 1)
			{
				lootIndex = (randomChance >= 75 ? 1 : 0);
			}

			GameObject lootTemp = Instantiate (loots[lootIndex], this.transform.position, this.transform.localRotation);
			float randomX = Random.Range (-25, 25);
			float randomY = Random.Range (50, 100);
			lootTemp.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (randomX, randomY));
			yield return new WaitForSeconds (0.1f);
		}
	}
}