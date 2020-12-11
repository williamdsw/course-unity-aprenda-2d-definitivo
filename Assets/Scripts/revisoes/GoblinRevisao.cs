using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simula os estados de maquina do Inimigo
public enum EstadoInimigoRevisao
{
	ALERT,
	ATTACK,
	BACKOFF,
	STOP,
	PATROL,
}

public class GoblinRevisao : MonoBehaviour 
{
	// Variaveis de controle
	public float velocity;
	public float baseVelocity;
	public float timeToIdle;
	public float timeToBackoff;
	public int weaponID;
	public int classID;
	private Vector3 direction = Vector3.right;

	[Header ("Booleanos")]
	public bool isLookingLeft;
	private bool isAttacking;
	private bool isOnAlert;
	private bool isOnDark;
	
	[Header ("Valores de Distância")]
	public float distanceToChangeRoute;
	public float distanceToSeePlayer;
	public float distanceToAttack;
	public float distanceToAlertOff;

	// Components
	private Rigidbody2D rigidBody2D;
	private Animator animator;
	private SpriteRenderer spriteRenderer;

	// Objetos
	private PlayerRevisao playerRevisao;
	private GameControllerRevisao gameControllerRevisao;
	public EstadoInimigoRevisao actualState;
	public EstadoInimigoRevisao initialState;
	public LayerMask obstacleLayers;
	public LayerMask playerLayers;
	public GameObject ballonAlert;
	public GameObject[] weapons;
	public GameObject[] bows;
	public GameObject[] arrows;
	public GameObject[] staffs;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		rigidBody2D = this.GetComponent<Rigidbody2D>();	
		animator = this.GetComponent<Animator>();	
		spriteRenderer = this.GetComponent<SpriteRenderer>();	
		playerRevisao = FindObjectOfType (typeof (PlayerRevisao)) as PlayerRevisao;
		gameControllerRevisao = FindObjectOfType (typeof (GameControllerRevisao)) as GameControllerRevisao;

		// Desabilita objetos
		ballonAlert.SetActive (false);

		// Muda posicao
		if (isLookingLeft)
		{
			Flip ();
		}

		// Chama funcoes
		Material material = (isOnDark ? gameControllerRevisao.light2D : gameControllerRevisao.default2D);
		ChangeMaterial (material);
		ChangeState (initialState);
		ChangeWeapon (weaponID);
	}

	private void Update () 
	{
		// Verificacao de observacao contra o personagem
		if (actualState != EstadoInimigoRevisao.ATTACK && actualState != EstadoInimigoRevisao.BACKOFF)
		{
			RaycastHit2D playerHit = Physics2D.Raycast (this.transform.position, direction, distanceToSeePlayer, playerLayers);
			if (playerHit)
			{
				ChangeState (EstadoInimigoRevisao.ALERT);
			}
		}

		// Verificacao de observacao contra o obstaculo enquanto patrulha
		if (actualState == EstadoInimigoRevisao.PATROL)
		{
			RaycastHit2D obstacleHit = Physics2D.Raycast (this.transform.position, direction, distanceToChangeRoute, obstacleLayers);
			if (obstacleHit)
			{
				ChangeState (EstadoInimigoRevisao.STOP);
			}
		}

		// Verificacao de colisao contra obstaculo enquanto recuar
		if (actualState == EstadoInimigoRevisao.BACKOFF)
		{
			RaycastHit2D obstacleHit = Physics2D.Raycast (this.transform.position, direction ,distanceToChangeRoute, obstacleLayers);
			if (obstacleHit)
			{
				Flip ();
			}
		}

		if (actualState == EstadoInimigoRevisao.ALERT)
		{
			// Calcula distancia entre inimigo e player
			float distance = Vector3.Distance (this.transform.position, playerRevisao.transform.position);

			// Define estado
			if (distance <= distanceToAttack)
			{
				ChangeState (EstadoInimigoRevisao.ATTACK);
			}
			else if (distance >= distanceToAlertOff && !isOnAlert)
			{
				ChangeState (EstadoInimigoRevisao.STOP);
			}
		}

		// Exibe balao de alerto
		if (actualState == EstadoInimigoRevisao.ALERT)
		{
			ballonAlert.SetActive (true);
		}

		// "Anda"
		rigidBody2D.velocity = new Vector2 (velocity, rigidBody2D.velocity.y);

		// Define variaveis de animator
		animator.SetInteger ("animation_id", (velocity == 0 ? 0 : 1));
		animator.SetFloat ("class_id", classID);
	}

	// ------------------- FUNCOES ------------------- //

	// Muda a direcao do sprite
	public void Flip ()
	{
		// Atualizando posicao, escala em X, e eixo X da direcao
		isLookingLeft = !isLookingLeft;
		float localScaleX = this.transform.localScale.x;
		localScaleX *= -1;
		this.transform.localScale = new Vector3 (localScaleX, this.transform.localScale.y, this.transform.localScale.z);
		direction.x = localScaleX;
		baseVelocity *= -1;
		float actualVelocity = (velocity * -1);
		velocity = actualVelocity;
	}

	// Muda estado atual de maquina passando um novo
	public void ChangeState (EstadoInimigoRevisao newState)
	{
		this.actualState = newState;

		switch (newState)
		{
			// Alerta
			case EstadoInimigoRevisao.ALERT:
			{
				velocity = 0;
				ballonAlert.SetActive (true);
				break;
			}
			
			// Ataque
			case EstadoInimigoRevisao.ATTACK:
			{
				animator.SetTrigger ("Attack");
				break;
			}

			// Recuar
			case EstadoInimigoRevisao.BACKOFF:
			{
				Flip ();
				velocity = (baseVelocity * 2);
				StartCoroutine ("BackOff");
				break;
			}

			// Parar
			case EstadoInimigoRevisao.STOP:
			{
				velocity = 0;
				StartCoroutine ("Idle");
				break;
			}

			// Patrulha
			case EstadoInimigoRevisao.PATROL:
			{
				velocity = baseVelocity;
				break;
			}

			default:
			{
				break;
			}
		}
	}

	// Muda o tipo de material do player e armas
	public void ChangeMaterial (Material newMaterial)
	{
		spriteRenderer.material = newMaterial;

        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<SpriteRenderer>().material = newMaterial;
        }

        foreach (GameObject bow in bows)
        {
            bow.GetComponent<SpriteRenderer>().material = newMaterial;
        }

        foreach (GameObject arrow in arrows)
        {
            arrow.GetComponent<SpriteRenderer>().material = newMaterial;
        }

        foreach (GameObject staff in staffs)
        {
            staff.GetComponent<SpriteRenderer>().material = newMaterial;
        }
	}

	// Controla se esta atacando
    private void Attack (int value)
    {
		if (value == 0)
		{
			isAttacking = false;
			weapons[2].SetActive (false);
			ChangeState (EstadoInimigoRevisao.BACKOFF);
		}
		else if (value == 1)
		{
			isAttacking = true;
		}
    }

	// Controla se esta atacando
    private void BowAttack (int value)
    {
        switch (value)
        {
            case 0:
            {
                isAttacking = false;
                bows[2].SetActive (false);
                break;
            }

            case 1:
            {
                isAttacking = true;
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
    private void StaffAttack (int value)
    {
        switch (value)
        {
            case 0:
            {
                isAttacking = false;
                staffs[3].SetActive (false);
                break;
            }

            case 1:
            {
                isAttacking = true;
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
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive (false);
        }

        weapons[id].SetActive (true);
    }

	// Controle do objetos (sprites) do arco-flecha
    private void BowControl (int id)
    {
        foreach (GameObject bow in bows)
        {
            bow.SetActive (false);
        }

        bows[id].SetActive (true);
    }

    // Controle do objetos (sprites) do staff
    private void StaffControl (int id)
    {
        foreach (GameObject staff in staffs)
        {
            staff.SetActive (false);
        }

        staffs[id].SetActive (true);
    }


	public void ChangeWeapon (int id)
	{
		weaponID = id;

        switch (id)
        {
            // Espadas, Machados, Martelos
            case 0:
            {
                // Atualiza sprites
                weapons[0].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite1[weaponID];
                weapons[1].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite2[weaponID];
                weapons[2].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite3[weaponID];

                // Atualiza dados
                for (int i = 0; i < weapons.Length; i++)
                {
                    ArmaInfo armaInfo = weapons[i].GetComponent<ArmaInfo>();
                    armaInfo.danoMinimo = gameControllerRevisao.minDamages[weaponID];
                    armaInfo.danoMaximo = gameControllerRevisao.maxDamages[weaponID];
                    armaInfo.tipoDano = gameControllerRevisao.damageTypesInt[weaponID];
                }

                break;
            }

            // Arcos
            case 1:
            {
                // Atualiza sprites
                bows[0].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite1[weaponID];
                bows[1].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite2[weaponID];
                bows[2].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite3[weaponID];

                break;
            }

            // Staff
            case 2:
            {
                // Atualiza sprites
                staffs[0].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite1[weaponID];
                staffs[1].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite2[weaponID];
                staffs[2].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite3[weaponID];
                staffs[3].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite4[weaponID];

                break;
            }

            default:
            {
                break;
            }
        }
	}

	// Chama corrotina e muda estado
	public void GetHit ()
	{
		isOnAlert = true;
		StartCoroutine ("TakeHit");
		ChangeState (EstadoInimigoRevisao.ALERT);
	}

	// ------------------- CORROTINAS ------------------- //

	// Espera 'n' segundos, muda direcao e muda estado
	private IEnumerator Idle ()
	{
		yield return new WaitForSeconds (timeToIdle);
		Flip ();
		ChangeState (EstadoInimigoRevisao.PATROL);
	}

	// Espera 'n' segundos, muda direcao e muda estado
	private IEnumerator BackOff ()
	{
		yield return new WaitForSeconds (timeToBackoff);
		Flip ();
		ChangeState (EstadoInimigoRevisao.ALERT);
	}

	private IEnumerator TakeHit ()
	{
		yield return new WaitForSeconds (1f);
		isOnAlert = false;
	}
}