using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRevisao : MonoBehaviour 
{
	// Variaveis de controle
	private bool isOpen = false;
	private int minItemQuantity;
	private int maxItemQuantity;

	// Components
	private SpriteRenderer spriteRenderer;
	private Collider2D myCollider2D;
	public List<Sprite> sprites = new List<Sprite>(2);

	// Outros objetos
	private List<GameObject> lootsPrefabs;

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa objetos
	private void Start ()
	{
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		lootsPrefabs = new List<GameObject> ();

		// Adiciona prefabs a lista
		lootsPrefabs.Add ((GameObject) Resources.Load ("Prefabs/Coin"));
		lootsPrefabs.Add ((GameObject) Resources.Load ("Prefabs/GreenCoin"));
	}

	// ------------------- FUNCOES ------------------- //

	// Interagem com a caixa
	public void Interact ()
	{
		if (!isOpen)
		{
			isOpen = true;
			spriteRenderer.sprite = sprites[1];
			StartCoroutine ("SpawnLoot");
			myCollider2D.enabled = false;
		}
	}

	// ------------------- CORROTINAS ------------------- //

	// Gera 'n' loots quando abre o bau
	private IEnumerator SpawnLoot ()
	{
		int quantity = Random.Range (minItemQuantity, maxItemQuantity);

		for (int i = 0; i < quantity; i++)
		{
			// Calcula chance de escolher um prefab
			int chance = Random.Range (0, 100);
			int lootIndex = 0;

			if (lootsPrefabs.Count > 1)
			{
				lootIndex = (chance >= 75 ? 1 : 0);
			}

			// Instancia e propriedades
			GameObject lootTemp = Instantiate (lootsPrefabs[lootIndex], this.transform.position, this.transform.localRotation);
			float randomX = Random.Range (-25, 25);
			float randomY = Random.Range (50, 100);
			lootTemp.AddComponent<Rigidbody2D>().AddForce (new Vector2 (randomX, randomY));
			yield return new WaitForSeconds (0.1f);
		}
	}
}