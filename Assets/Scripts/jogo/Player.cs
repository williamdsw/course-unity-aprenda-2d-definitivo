using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Variaveis de controle
    public float speed;                  // Velocidade de movimento
    public float jumpForce;              // Forca aplicada sobre o pulo
    public bool isGrounded;              // Indica se esta no chao ou nao
    public bool isLookingLeft;           // Indica se esta olhando pra esquerda 
    public bool isAttacking;             // Indica se esta executando um ataque 
    public int idAnimation;              // Identificacao da animacao
    private float horizontal;            // Indica a velocidade Horizontal
    private float vertical;              // Indica a velocidade Vertical
    public int vidaMaxima = 10;
    public int vidaAtual;
    private bool naoPodeAtacar;          // Indica se podemos executar um ataque

    // Objetos de components
    private Animator animator;            // Objeto responsavel pelo Animator
    private Rigidbody2D rigidBody2D;      // Objeto responsavel pelo Rigidbody2D
    private SpriteRenderer spriteRenderer;

    // ITERACAO COM ITENS E OBJETOS
    public Transform groundCheck;                       // Objeto responsavel pela deteccao da colisao com o chao
    public LayerMask whatIsGround;                      // Indica o que e superficie para o reste do grounded
    public Transform handTransform;                     // Indica o transform do child do Player para Raycast2D
    private Vector3 playerDirection = Vector3.right;    // Indica a direcao do personagem em Vector3
    public LayerMask interacao;                         // Indica os objetos a serem interagidos por essa LayerMask
    public GameObject objetoInteracao;
    private GameController gameController;
    public GameObject balaoAlerta;
    private AudioController audioController;

    [Header ("Sistema de Armas")]
    public int idArma;
    public int idArmaAtual;
    public GameObject[] weapons;
    public GameObject[] arcos;
    public GameObject[] flechas;
    public GameObject[] staffs;
    public GameObject magiaPrefab;
    public Transform spawnFlecha;
    public Transform spawnMagia;

    // ------------------- FUNCOES UNITY ------------------- //

    private void Start ()
    {
        // Pegando components
        animator = this.GetComponent<Animator> ();
        rigidBody2D = this.GetComponent<Rigidbody2D> ();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        gameController = FindObjectOfType<GameController>();
        audioController = FindObjectOfType<AudioController>();

        // Inicializa parametros
        vidaMaxima = gameController.vidaMaxima;
        idArma = gameController.idArma;
        vidaAtual = vidaMaxima;
        gameController.manaAtual = gameController.manaMaxima;

        // Desabilita armas, arcos, e staffs

        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive (false);
        }

        foreach (GameObject arco in arcos)
        {
            arco.SetActive (false);
        }

        foreach (GameObject staff in staffs)
        {
            staff.SetActive (false);
        }

        TrocarArma (idArma);
    }

    private void Update ()
    {
        // Interacao depois do dialogo ja inicializado
        if (gameController.estadoAtual == GameState.DIALOGO)
        {
            // Para o player
            rigidBody2D.velocity = new Vector2 (0, rigidBody2D.velocity.y);
            animator.SetInteger ("id_animation", 0);
            balaoAlerta.SetActive (false);

            // Interage com NPC
            if (Input.GetButtonDown ("Fire1"))
            {
                objetoInteracao.SendMessage ("Falar", SendMessageOptions.DontRequireReceiver);
            }
        }

        // Cancela comandos se nao estiver no gameplay
        if (gameController.estadoAtual != GameState.GAMEPLAY)
        {
            return;
        }

        Movement ();
    }

    private void FixedUpdate ()
    {
        // Cancela comandos se nao estiver no gameplay
        if (gameController.estadoAtual != GameState.GAMEPLAY)
        {
            return;
        }

        // Physics2D.OverlapCircle (Vector2, float, int) = Verifica a colisao da posicao de um objeto
        isGrounded = Physics2D.OverlapCircle (groundCheck.position, 0.02f, whatIsGround);

        // Atualiza a velocidade do corpo rigido com a velocidade horizontal e mantendo a velocidade em y
        rigidBody2D.velocity = new Vector2 (horizontal * speed, rigidBody2D.velocity.y);

        Interact ();
    }

    private void LateUpdate () 
    {
        if (gameController.idArma != gameController.idArmaAtual)    
        {
            TrocarArma (gameController.idArma);
        }
    }

    // FUNCOES PARA TRATAR COLISOES
    // FUNCOES PARA TRATAR TRIGGERS

    // OnTriggerEnter2D: Quando o objeto colidir com o trigger
    // Collider2D: objeto que foi "gatilhado"
    private void OnTriggerEnter2D (Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Lampada":
            {
                print ("Colidiu com a lampada trigger");
                break;
            }

            case "Coletavel":
            {
                other.gameObject.SendMessage ("Coletar", SendMessageOptions.DontRequireReceiver);
                break;
            }

            case "Inimigo":
            {
                gameController.vidaAtual -= 1;
                break;
            }

            default:
            {
                break;
            }
        }
    }

    // ------------------- FUNCOES ------------------- //

    // Controla movimentacao e pulo do personagem
    private void Movement ()
    {
        // Input.GetAxisRaw () = Pega valores float dos eixos "Horizontal" e "Vertical" de acordo com as setas do teclado
        horizontal = Input.GetAxisRaw ("Horizontal");
        vertical = Input.GetAxisRaw ("Vertical");

        // Se estiver andando pra frente , olhando pra frente e nao estiver atacando
        if (horizontal > 0 && isLookingLeft && !isAttacking)
        {
            Flip ();
        }
        else if (horizontal < 0 && !isLookingLeft && !isAttacking)
        {
            Flip ();
        }

        /* Se agachar */
        if (vertical < 0)
        {
            idAnimation = 2;
            horizontal = (isGrounded ? 0 : horizontal);
        }
        else if (horizontal != 0)
        {
            idAnimation = 1;
        }
        else
        {
            idAnimation = 0;
        }

        // Se apertar o botao de ataque e estiver em pe
        if (Input.GetButtonDown ("Fire1") && vertical >= 0 && !isAttacking && objetoInteracao == null && !naoPodeAtacar)
        {
            naoPodeAtacar = true;
            animator.SetTrigger ("attack");
        }

        // Se apertar o botao de ataque e estiver em pe
        if (Input.GetButtonDown ("Fire1") && vertical >= 0 && !isAttacking && objetoInteracao != null)
        {
            /* object.SendMessage(String method, SendMessageOptions options): 
             * Envia uma "mensagem" para o objeto chamando uma funcao do script dele pelo nome da mesma.
             * -- "Iteracao" = Nesse caso e o nome da funcao que existe no script do "objetoInteracao"
             * -- "SendMessageOptions.DontRequireReceiver" = Nao precisa de um receptor para nao retornar erro */
            objetoInteracao.SendMessage ("Interact", SendMessageOptions.DontRequireReceiver);
        }

        // Se apertar o botao  e estiver no chao
        if (Input.GetButtonDown ("Jump") && isGrounded && !isAttacking)
        {
            // Adiciona forca no corpo rigido em Y
            rigidBody2D.AddForce (new Vector2 (0, jumpForce));
        }

        // Se estiver no chao e atacando, nao pode andar
        horizontal = (isAttacking && isGrounded ? 0 : horizontal);

        // Faz controle das flechas
        foreach (GameObject flecha in flechas)
        {
            flecha.SetActive ((gameController.quantidadeFlechas[gameController.idFlechaEquipada] > 0));
        }

        /* Define parametros */
        animator.SetBool ("grounded", isGrounded);
        animator.SetInteger ("id_animation", idAnimation);
        animator.SetFloat ("speed_y", rigidBody2D.velocity.y);
        animator.SetFloat ("id_classe_arma", gameController.idClasseArma[gameController.idArmaAtual]);
    }

    // Muda a direcao do sprite
    private void Flip ()
    {
        // Atualizando posicao, escala em X, e eixo X da direcao
        isLookingLeft = !isLookingLeft;
        float localScaleX = this.transform.localScale.x;
        localScaleX *= -1;
        this.transform.localScale = new Vector3 (localScaleX, transform.localScale.y, transform.localScale.z);
        playerDirection.x = localScaleX;
    }

    // Controla se esta atacando
    private void Attack (int atk)
    {
        switch (atk)
        {
            case 0:
            {
                isAttacking = false;
                weapons[2].SetActive (false);
                StartCoroutine ("EsperarNovoAtaque");
                break;
            }

            case 1:
            {
                audioController.TocarEfeito (audioController.efeitoEspada, 1f);
                isAttacking = true;
                break;
            }
            
            default:
            {
                break;
            }
        }
    }

    // Controla se esta atacando
    private void AttackFlecha (int atk)
    {
        switch (atk)
        {
            case 0:
            {
                isAttacking = false;
                arcos[2].SetActive (false);
                StartCoroutine ("EsperarNovoAtaque");
                break;
            }

            case 1:
            {
                isAttacking = true;
                break;
            }

            case 2:
            {
                // Instancia flecha
                if (gameController.quantidadeFlechas[gameController.idFlechaEquipada] > 0)
                {
                    audioController.TocarEfeito (audioController.efeitoArco, 1f);
                    gameController.quantidadeFlechas[gameController.idFlechaEquipada]--;
                    GameObject flechaTemp = Instantiate (gameController.flechaPrefabs[gameController.idFlechaEquipada], spawnFlecha.position, spawnFlecha.localRotation);
                    flechaTemp.transform.localScale = new Vector3 (flechaTemp.transform.localScale.x * playerDirection.x, flechaTemp.transform.localScale.y, flechaTemp.transform.localScale.z);
                    flechaTemp.GetComponent<Rigidbody2D>().velocity = new Vector2 (gameController.velocidadesFlecha[gameController.idFlechaEquipada] * playerDirection.x, 0);
                    Destroy (flechaTemp, 2f);
                }

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
                isAttacking = false;
                staffs[3].SetActive (false);
                StartCoroutine ("EsperarNovoAtaque");
                break;
            }

            case 1:
            {
                isAttacking = true;
                break;
            }

            case 2:
            {
                // Instancia magia
                if (gameController.manaAtual >= 1)
                {
                    audioController.TocarEfeito (audioController.efeitoCajado, 1f);
                    gameController.manaAtual -= 1;
                    GameObject magiaTemp = Instantiate (magiaPrefab, spawnMagia.position, spawnMagia.localRotation);
                    magiaTemp.GetComponent<Rigidbody2D>().velocity = new Vector2 (3 * playerDirection.x, 0);
                    Destroy (magiaTemp, 1f);    
                }

                break;
            }
            
            default:
            {
                break;
            }
        }
    }

    // Interage com objetos
    private void Interact ()
    {
        /* Physics2D.Raycast (Vector2 origin, Vector2 direction, Float distance): 
         * Cria um Raycast (linha) em determinada posicao, apontando para tal direcao com determinada distancia. 
         * -- Vector2 origin = Nesse caso a "position" do personagem pelo "transform" (pivot)
         * -- Vector2 direction = Nesse caso "Vector3.right" que e uma abreviacao para Vector(1, 0 ,0)
         * -- Float distance = Nesse caso e a distancia da linha (tamanho?)
         * -- Int layerMask = Nesse caso sera a layerMask "Teste" para iteracoes */
        RaycastHit2D hit = Physics2D.Raycast (handTransform.position, playerDirection, 0.1f, interacao);

        // Mesmo efeito do Raycast, porem desenha no debug
        Debug.DrawRay (handTransform.position, playerDirection * 0.1f, Color.red);

        // Passa objeto local para objeto glocal caso haja colisao
        if (hit)
        {
            objetoInteracao = hit.collider.gameObject;
            balaoAlerta.SetActive (true);
        }
        else 
        {
            objetoInteracao = null;
            balaoAlerta.SetActive (false);
        }
    }

    // Controle do objetos (sprites) da arma
    private void WeaponControl (int id)
    {
        foreach (var weapon in weapons)
        {
            weapon.SetActive (false);
        }

        weapons[id].SetActive (true);
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

    // Muda o tipo de material do player e armas
    public void MudarMaterial (Material novoMaterial)
    {
        spriteRenderer.material = novoMaterial;

        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<SpriteRenderer>().material = novoMaterial;
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

    // Muda sprite e valores das armas de acordo do ID informado
    public void TrocarArma (int id)
    {
        gameController.idArma = id;

        switch (gameController.idClasseArma[id])
        {
            // Espadas, Machados, Martelos
            case 0:
            {
                // Atualiza sprites
                weapons[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[id];
                weapons[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[id];
                weapons[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[id];

                // Atualiza dados
                for (int i = 0; i < weapons.Length; i++)
                {
                    ArmaInfo armaInfo = weapons[i].GetComponent<ArmaInfo>();
                    armaInfo.danoMinimo = gameController.danosMinimo[id];
                    armaInfo.danoMaximo = gameController.danosMaximo[id];
                    armaInfo.tipoDano = gameController.tipoDanosArma[id];
                }

                break;
            }

            // Arcos
            case 1:
            {
                // Atualiza sprites
                arcos[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[id];
                arcos[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[id];
                arcos[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[id];

                break;
            }

            // Staff
            case 2:
            {
                // Atualiza sprites
                staffs[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[id];
                staffs[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[id];
                staffs[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[id];
                staffs[3].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas4[id];

                break;
            }

            default:
            {
                break;
            }
        }
       
        gameController.idArmaAtual = gameController.idArma;
    }

    // ------------------- CORROTINAS ------------------- //

    // Espera 'n' segundos para liberar o ataque
    private IEnumerator EsperarNovoAtaque ()
    {
        yield return new WaitForSeconds (0.2f);
        naoPodeAtacar = false;
    }
}