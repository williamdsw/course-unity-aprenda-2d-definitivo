using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControleDanoInimigo : MonoBehaviour 
{
	[Header ("Variaveis de Controle")]
	public bool estaOlhandoEsquerda;
	public bool playerEstaOlhandoEsquerda;
	private bool foiAcertado;
	private bool foiMorto;

	[Header ("Variaveis de Vida")]
	public int vidaInimigo;
	public int vidaAtual;
	private float percentualVida;							// Controla o percentual de vida
	public GameObject barrasDeVida;							// Objeto que contem as barras
	public Transform barraDeHP;								// Objeto indicador da barra de vida
	public Color[] characterColors;
	public GameObject danoTextPrefab;						// Indica o numero de dano

	[Header ("Variaveis de Knockback")]
	public GameObject knockForcePrefab;						// Forca de repulsao
	public Transform knockPosition;							// Ponto de origem da forca
	public float knockX = 0.3f;								// Valor padrao
	private float kx;

	[Header ("Configuração de Chão")]
	public Transform groundCheckTransform;
	public LayerMask whatIsGround;

	[Header ("Configuração de Loot")]
	public GameObject[] loots;

	// Components
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	// Outros objetos
	private GameController gameController;
	private Player player;
	private AudioController audioController;

	[Header ("Configuração de Resistência / Fraqueza")]
	public float[] ajusteDano;								// Resistencia / Fraqueza contra tipo de dano

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		animator = this.GetComponent<Animator>();
		gameController = GameObject.FindObjectOfType (typeof (GameController)) as GameController;
		player = GameObject.FindObjectOfType<Player>();
		audioController = GameObject.FindObjectOfType<AudioController>();

		// Configuracoes
		spriteRenderer.color = characterColors[0];
		barrasDeVida.SetActive (false);
		vidaAtual = vidaInimigo;
		barrasDeVida.transform.localScale = new Vector3 (1, 1, 1);

		// Flip
		if (estaOlhandoEsquerda == true)
		{
			float localScaleX = transform.localScale.x;
			localScaleX *= -1;
			transform.localScale = new Vector3 (localScaleX, transform.localScale.y, transform.localScale.z);
			barrasDeVida.transform.localScale = new Vector3 (localScaleX, barrasDeVida.transform.localScale.y, barrasDeVida.transform.localScale.z);
		}
	}
	
	private void Update () 
	{
		if (!foiMorto)
		{
			// Verifica se o player esta a esquerda / direita
			float xPlayer = player.transform.position.x;

			if (xPlayer < transform.position.x)
			{
				playerEstaOlhandoEsquerda = true;
			}
			else if (xPlayer > transform.position.x)
			{
				playerEstaOlhandoEsquerda = false;
			}

			if (playerEstaOlhandoEsquerda && estaOlhandoEsquerda)
			{
				kx = knockX;
			}
			else if (playerEstaOlhandoEsquerda && !estaOlhandoEsquerda)
			{
				kx = (knockX * -1);
			}
			else if (!playerEstaOlhandoEsquerda && estaOlhandoEsquerda)
			{
				kx = (knockX * -1);
			}
			else if (!playerEstaOlhandoEsquerda && !estaOlhandoEsquerda)
			{
				kx = knockX;
			}

			// Atualiza posicao
			knockPosition.localPosition = new Vector3 (kx, knockPosition.localPosition.y, 0);
			animator.SetBool ("grounded", true);
		}
	}

	// Quando colide com algo
	private void OnTriggerEnter2D (Collider2D other)
	{
		// Para o bloco
		if (foiMorto) { return; }

		switch (other.gameObject.tag)
		{
			case "Arma":
			{
				if (!foiAcertado)
				{
					foiAcertado = true;
					barrasDeVida.SetActive (true);
					animator.SetTrigger ("hit");
					audioController.TocarEfeito (audioController.efeitoHit, 1);

					// Informacoes do dano
					ArmaInfo info = other.gameObject.GetComponent<ArmaInfo>();
					float danoArma = Random.Range (info.danoMinimo, info.danoMaximo);
					int tipoDano = info.tipoDano;
					float danoTomado = danoArma + (danoArma * (ajusteDano[tipoDano] / 100));

					// Reduz da vida a quantidade de vida e define percentual
					vidaAtual -= Mathf.RoundToInt (danoTomado);
					percentualVida = (float) vidaAtual / (float) vidaInimigo;
					percentualVida = (percentualVida < 0 ? 0 : percentualVida);
					barraDeHP.localScale = new Vector3 (percentualVida, 1, 1);

					if (vidaAtual <= 0)
					{
						foiMorto = true;

						if (this.GetComponent<Goblin>() != null)
						{
							this.GetComponent<Goblin>().alertaBalao.SetActive (false);
							this.GetComponent<Goblin>().enabled = false;
						}

						animator.SetInteger ("id_animation", 3);
						StartCoroutine ("Loot");
					}
					else 
					{
						// Efeito de hit
						GameObject fxTemp = Instantiate (gameController.fxDanos[tipoDano], this.transform.position, this.transform.rotation);
						Destroy (fxTemp, 1f);

						// Texto de hit
						GameObject danoTemp = Instantiate (danoTextPrefab, transform.position, transform.rotation);
						danoTemp.GetComponentInChildren<MeshRenderer>().sortingLayerName = "HUD";
						danoTemp.GetComponentInChildren<TextMeshPro>().text = Mathf.RoundToInt (danoTomado).ToString ();
						float forcaX = 50;
						
						if (!playerEstaOlhandoEsquerda)
						{
							forcaX *= -1;
						}

						danoTemp.GetComponentInChildren<Rigidbody2D>().AddForce (new Vector2 (forcaX, 200));
						Destroy (danoTemp, 1f);

						// Instancia knockback
						GameObject knockTemp = Instantiate (knockForcePrefab, knockPosition.position, knockPosition.localRotation);
						Destroy (knockTemp, 0.02f);
						StartCoroutine ("Invuneravel");

						// Chama o metodo do outro script
						this.gameObject.SendMessage ("TomeiHit", SendMessageOptions.DontRequireReceiver);
					}
				}

				break;
			}

			default:
			{
				break;
			}
		}
	}

	// ------------------- FUNCOES ------------------- //

	// Muda a direcao do sprite
    private void Flip ()
    {
        // Atualizando posicao, escala em X, e eixo X da direcao
        estaOlhandoEsquerda = !estaOlhandoEsquerda;
        float localScaleX = transform.localScale.x;
        localScaleX *= -1;
        transform.localScale = new Vector3 (localScaleX, transform.localScale.y, transform.localScale.z);
		barrasDeVida.transform.localScale = new Vector3 (localScaleX, barrasDeVida.transform.localScale.y, barrasDeVida.transform.localScale.z);
    }

	// ------------------- CORROTINAS ------------------- //

	// "Pisca" o sprite
	private IEnumerator Invuneravel ()
	{
		spriteRenderer.color = characterColors[1];
		yield return new WaitForSeconds (0.2f);
		spriteRenderer.color = characterColors[0];
		yield return new WaitForSeconds (0.2f);
		spriteRenderer.color = characterColors[1];
		yield return new WaitForSeconds (0.2f);
		spriteRenderer.color = characterColors[0];
		yield return new WaitForSeconds (0.2f);
		spriteRenderer.color = characterColors[1];
		yield return new WaitForSeconds (0.2f);
		spriteRenderer.color = characterColors[0];
		foiAcertado = false;
		barrasDeVida.SetActive (false);
	}

	// Gera loots
	private IEnumerator Loot ()
	{
		this.gameObject.layer = 12;
		yield return new WaitForSeconds (1f);
		GameObject fxMorte = Instantiate (gameController.fxMorte, groundCheckTransform.position, this.transform.localRotation);
		yield return new WaitForSeconds (0.5f);
		spriteRenderer.enabled = false;
		barrasDeVida.SetActive (false);

		int quantidadeMoedas = Random.Range (1, 5);

		// Gera moedas em anuglos diferentes
		for (int i = 0; i < quantidadeMoedas; i++)
		{
			int randomIndex = Random.Range (0, loots.Length - 1);
			GameObject lootTemp = Instantiate (loots[randomIndex], this.transform.position, this.transform.localRotation);
			audioController.TocarEfeito (audioController.efeitoSpawnMoedas, 1f);
			lootTemp.GetComponent<Rigidbody2D>().AddForce (new Vector2 (Random.Range (-25, 25), 75));
			yield return new WaitForSeconds (0.1f);
		}

		yield return new WaitForSeconds (0.7f);
		Destroy (fxMorte);
		Destroy (this.gameObject);
	}
}