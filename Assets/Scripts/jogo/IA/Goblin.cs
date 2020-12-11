using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simula os estados de maquina do Inimigo
public enum EstadoInimigo
{
	PARADO, 
	ALERTA,
	PATRULHA,
	ATAQUE,
	RECUAR
}

public class Goblin : MonoBehaviour 
{
	// Variaveis de Controle
	public float velocidadeBase;
	public float velocidade;
	public float distanciaMudarRota;
	public float distanciaVerPersonagem;
	public float distanciaParaAtacar;
	public float distanciaSairAlerta;
	public bool estaOlhandoEsquerda;
	public float tempoParaFicarParado;
	public float tempoParaRecuar;
	private bool estaAtacando;
	private bool estaEmAlerta;
	public int idArma;
	public int idClasse;
	public bool estaNoEscuro;
	private Vector3 direcao = Vector3.right;

	// Components
	private Rigidbody2D rigidBody2D;
	private Animator animator;
	private SpriteRenderer spriteRenderer;

	// Objetos
	public EstadoInimigo estadoAtual;
	public EstadoInimigo estadoInicial;
	public GameObject alertaBalao;
	public LayerMask layerObstaculos;
	public LayerMask layerPersonagem;
	private Player player;
	public GameObject[] armas;
	public GameObject[] arcos;
	public GameObject[] flechas;
	public GameObject[] staffs;
	private GameController gameController;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		rigidBody2D = this.GetComponent<Rigidbody2D>();
		animator = this.GetComponent<Animator>();
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		player = FindObjectOfType (typeof (Player)) as Player;
		gameController = FindObjectOfType (typeof (GameController)) as GameController;

		// Desabilita objetos
		alertaBalao.SetActive (false);

		// Muda posicao 
		if (estaOlhandoEsquerda)
		{
			Flip ();
		}

		// Chama funcoes
		MudarEstado (estadoInicial);
		TrocarArma (idArma);
		Material material = (estaNoEscuro ? gameController.luz2D : gameController.padrao2D);
		MudarMaterial (material);
	}

	private void Update () 
	{
		// Verificacao de observacao contra o personagem
		if (estadoAtual != EstadoInimigo.ATAQUE && estadoAtual != EstadoInimigo.RECUAR)
		{
			RaycastHit2D hitPersonagem = Physics2D.Raycast (this.transform.position, direcao, distanciaVerPersonagem, layerPersonagem);
			if (hitPersonagem)
			{
				MudarEstado (EstadoInimigo.ALERTA);
			}
		}

		// Verificacao de observacao contra o obstaculo enquanto patrulha
		if (estadoAtual == EstadoInimigo.PATRULHA)
		{
			RaycastHit2D hitObstaculo = Physics2D.Raycast  (this.transform.position, direcao, distanciaMudarRota, layerObstaculos);
			if (hitObstaculo)
			{
				MudarEstado (EstadoInimigo.PARADO);
			}
		}

		// Verificacao de colisao contra obstaculo enquanto recuar
		if (estadoAtual == EstadoInimigo.RECUAR)
		{
			RaycastHit2D hitObstaculo = Physics2D.Raycast  (this.transform.position, direcao, distanciaMudarRota, layerObstaculos);
			if (hitObstaculo)
			{
				Flip ();
			}
		}

		if (estadoAtual == EstadoInimigo.ALERTA)
		{
			// Calcula distancia entre inimigo e player
			float distancia = Vector3.Distance (this.transform.position, player.transform.position);

			// Define estado
			if (distancia <= distanciaParaAtacar)
			{
				MudarEstado (EstadoInimigo.ATAQUE);
			}
			else if (distancia >= distanciaSairAlerta && !estaEmAlerta)
			{
				MudarEstado (EstadoInimigo.PARADO);
			}
		}

		// Exibe balao
		if (estadoAtual != EstadoInimigo.ALERTA)
		{
			alertaBalao.SetActive (false);
		}

		// "Anda"
		rigidBody2D.velocity = new Vector2 (velocidade, rigidBody2D.velocity.y);

		// Define variaveis de animator
		animator.SetInteger ("id_animation", (velocidade == 0 ? 0 : 1));
		animator.SetFloat ("id_classe", idClasse);
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
        direcao.x = localScaleX;
		velocidadeBase *= -1;
		float velocidadeAtual = velocidade * -1;
		velocidade = velocidadeAtual;
    }

	// Muda estado atual de maquina passando um novo
	private void MudarEstado (EstadoInimigo proximoEstado)
	{
		this.estadoAtual = proximoEstado;

		switch (proximoEstado)
		{
			case EstadoInimigo.PARADO:
			{
				velocidade = 0;
				StartCoroutine ("FicarParado");
				break;
			}

			case EstadoInimigo.PATRULHA:
			{
				velocidade = velocidadeBase;
				break;
			}

			case EstadoInimigo.ALERTA:
			{
				velocidade = 0;
				alertaBalao.SetActive (true);
				break;
			}

			case EstadoInimigo.ATAQUE:
			{
				animator.SetTrigger ("attack");
				break;
			}

			case EstadoInimigo.RECUAR:
			{
				Flip ();
				velocidade = velocidadeBase * 2;
				StartCoroutine ("Recuar");
				break;
			}

			default:
			{
				break;
			}
		}
	}

	// Muda o tipo de material do player e armas
    public void MudarMaterial (Material novoMaterial)
    {
        spriteRenderer.material = novoMaterial;

        foreach (GameObject arma in armas)
        {
            arma.GetComponent<SpriteRenderer>().material = novoMaterial;
        }

        foreach (GameObject arco in arcos)
        {
            arco.GetComponent<SpriteRenderer>().material = novoMaterial;
        }

        foreach (GameObject flecha in flechas)
        {
            flecha.GetComponent<SpriteRenderer>().material = novoMaterial;
        }

        foreach (GameObject staff in staffs)
        {
            staff.GetComponent<SpriteRenderer>().material = novoMaterial;
        }
    }

	// Controla se esta atacando
    private void Attack (int atk)
    {
		if (atk == 0)
		{
			estaAtacando = false;
			armas[2].SetActive (false);
			MudarEstado (EstadoInimigo.RECUAR);
		}
		else if (atk == 1)
		{
			estaAtacando = true;
		}
    }

	// Controla se esta atacando
    private void AttackFlecha (int atk)
    {
        switch (atk)
        {
            case 0:
            {
                estaAtacando = false;
                arcos[2].SetActive (false);
                break;
            }

            case 1:
            {
                estaAtacando = true;
                break;
            }

            case 2:
            {
                /*if (gameController.quantidadeFlechas[gameController.idFlechaEquipada] > 0)
                {
                    gameController.quantidadeFlechas[gameController.idFlechaEquipada]--;
                    GameObject flechaTemp = Instantiate (gameController.flechaPrefabs[gameController.idFlechaEquipada], spawnFlecha.position, spawnFlecha.localRotation);
                    flechaTemp.transform.localScale = new Vector3 (flechaTemp.transform.localScale.x * playerDirection.x, flechaTemp.transform.localScale.y, flechaTemp.transform.localScale.z);
                    flechaTemp.GetComponent<Rigidbody2D>().velocity = new Vector2 (gameController.velocidadesFlecha[gameController.idFlechaEquipada] * playerDirection.x, 0);
                    Destroy (flechaTemp, 2f);
                }*/

                break;
            }
            
            default:
            {
                break;
            }
        }
    }

    // Controla se esta atacando
    private void AttackStaff (int atk)
    {
        switch (atk)
        {
            case 0:
            {
                estaAtacando = false;
                staffs[3].SetActive (false);
                break;
            }

            case 1:
            {
                estaAtacando = true;
                break;
            }

            case 2:
            {
                /*if (gameController.manaAtual >= 1)
                {
                    gameController.manaAtual -= 1;
                    GameObject magiaTemp = Instantiate (magiaPrefab, spawnMagia.position, spawnMagia.localRotation);
                    magiaTemp.GetComponent<Rigidbody2D>().velocity = new Vector2 (3 * playerDirection.x, 0);
                    Destroy (magiaTemp, 1f);    
                }*/

                break;
            }
            
            default:
            {
                break;
            }
        }
    }

	// Controle do objetos (sprites) da arma
    private void WeaponControl (int id)
    {
        foreach (var arma in armas)
        {
            arma.SetActive (false);
        }

        armas[id].SetActive (true);
    }

	// Controle do objetos (sprites) do arco-flecha
    private void ControleArcos (int id)
    {
        foreach (GameObject arco in arcos)
        {
            arco.SetActive (false);
        }

        arcos[id].SetActive (true);
    }

    // Controle do objetos (sprites) do staff
    private void ControleStaff (int id)
    {
        foreach (GameObject staff in staffs)
        {
            staff.SetActive (false);
        }

        staffs[id].SetActive (true);
    }

	// Muda sprite e valores das armas de acordo do ID informado
    public void TrocarArma (int id)
    {
		idArma = id;

        switch (id)
        {
            // Espadas, Machados, Martelos
            case 0:
            {
                // Atualiza sprites
                armas[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[idArma];
                armas[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[idArma];
                armas[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[idArma];

                // Atualiza dados
                for (int i = 0; i < armas.Length; i++)
                {
                    ArmaInfo armaInfo = armas[i].GetComponent<ArmaInfo>();
                    armaInfo.danoMinimo = gameController.danosMinimo[idArma];
                    armaInfo.danoMaximo = gameController.danosMaximo[idArma];
                    armaInfo.tipoDano = gameController.tipoDanosArma[idArma];
                }

                break;
            }

            // Arcos
            case 1:
            {
                // Atualiza sprites
                arcos[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[idArma];
                arcos[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[idArma];
                arcos[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[idArma];

                break;
            }

            // Staff
            case 2:
            {
                // Atualiza sprites
                staffs[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[idArma];
                staffs[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[idArma];
                staffs[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[idArma];
                staffs[3].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas4[idArma];

                break;
            }

            default:
            {
                break;
            }
        }
    }

	// Chama corrotina e muda estado
	public void TomeiHit ()
	{
		estaEmAlerta = true;
		StartCoroutine ("HitAlerta");
		MudarEstado (EstadoInimigo.ALERTA);
	}

	// ------------------- CORROTINAS ------------------- //

	// Espera 'n' segundos, muda direcao e muda estado
	private IEnumerator FicarParado ()
	{
		yield return new WaitForSeconds (tempoParaFicarParado);
		Flip ();
		MudarEstado (EstadoInimigo.PATRULHA);
	}

	// Espera 'n' segundos, muda direcao e muda estado
	private IEnumerator Recuar ()
	{
		yield return new WaitForSeconds (tempoParaRecuar);
		Flip ();
		MudarEstado (EstadoInimigo.ALERTA);
	}

	private IEnumerator HitAlerta ()
	{
		yield return new WaitForSeconds (1f);
		estaEmAlerta = false;
	}
}