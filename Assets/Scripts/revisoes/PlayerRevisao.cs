using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRevisao : MonoBehaviour
{
    // Variaveis de controle
    public float movementSpeed;
    public float jumpForce;
    public bool isGrounded;
    public bool isLookingLeft;
    public bool isAttacking;
    public int idAnimation;
    private float horizontal;
    private float vertical;
    private int maxLife = 10;
    private int actualLife;
    private bool cantAttack;

    // Components
    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer spriteRenderer;

    // Outros objetos
    private GameControllerRevisao gameControllerRevisao;
    private AudioControllerRevisao audioControllerRevisao;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public Transform handTransform;
    private Vector3 playerDirection = Vector3.right;
    public LayerMask layerIteration;
    public GameObject objectInteration;
    public GameObject alertBalloon;

    [Header ("Sistema de Armas")]
    public int weaponID;
    public int actualWeaponID;
    public GameObject[] weapons;
    public GameObject[] bows;
    public GameObject[] arrows;
    public GameObject[] staffs;
    public GameObject arrowPrefab;
    public GameObject magicPrefab;
    public Transform spawnArrowTransform;
    public Transform spawnMagicTransform;

    // Properties

    public int MaxLife 
    {
        get { return this.maxLife; }
        set { this.maxLife = value; }
    }

    public int ActualLife 
    {
        get { return this.actualLife; }
        set { this.actualLife = value; }
    }

    // ------------------- FUNCOES UNITY ------------------- //

    private void Start ()
    {
        // Pegando components
        animator = this.GetComponent<Animator> ();
        rigidBody2D = this.GetComponent<Rigidbody2D> ();
        spriteRenderer = this.GetComponent<SpriteRenderer> ();
        gameControllerRevisao = FindObjectOfType<GameControllerRevisao>();
        audioControllerRevisao = FindObjectOfType<AudioControllerRevisao>();

        // Inicializa parametros
        maxLife = gameControllerRevisao.playerMaxLife;
        weaponID = gameControllerRevisao.weaponID;
        actualLife = maxLife;
        gameControllerRevisao.playerActualMana = gameControllerRevisao.playerMaxMana;

        // Desabilita armas, arcos, e staffs
        
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive (false);
        }

        foreach (GameObject bow in bows)
        {
            bow.SetActive (false);
        }

        foreach (GameObject staff in staffs)
        {
            staff.SetActive (false);
        }

        ChangeWeapon (weaponID);
    }

    private void Update ()
    {
        // Interacao depois do dialogo ja inicializado
        if (gameControllerRevisao.actualGameState == GameStateRevisao.BEGIN_DIALOGUE)
        {
            // Para o player
            rigidBody2D.velocity = new Vector2 (0, rigidBody2D.velocity.y);
            animator.SetInteger ("id_animation", 0);
            alertBalloon.SetActive (false);

            // Interage com NPC
            if (Input.GetButton ("Fire1"))
            {
                objectInteration.SendMessage ("Interact", SendMessageOptions.DontRequireReceiver);
            }
        }

        // Cancela comandos se nao estiver no gameplay
        if (gameControllerRevisao.actualGameState != GameStateRevisao.GAMEPLAY)
        {
            return;
        }

        Movement ();
    }

    private void FixedUpdate ()
    {
        // Cancela comandos se nao estiver no gameplay
        if (gameControllerRevisao.actualGameState != GameStateRevisao.GAMEPLAY)
        {
            return;
        }

        // Physics2D.OverlapCircle (Vector2, float) =  Verifica a colisao da posicao de um objeto
        isGrounded = Physics2D.OverlapCircle (groundCheck.position, 0.02f, whatIsGround);

        // Atualiza a velocidade do corpo rigido com a velocidade horizontal e mantendo a velocidade em y
        rigidBody2D.velocity = new Vector2 (horizontal * movementSpeed, rigidBody2D.velocity.y);

        Interact ();
    }

    private void LateUpdate () 
    {
        if (gameControllerRevisao.weaponID != gameControllerRevisao.actualWeaponID)
        {
            ChangeWeapon (gameControllerRevisao.weaponID);
        }
    }

    // FUNCOES DE COLISOES
    // FUNCOES DE TRIGGERS

    // Quando colide com algum objeto com trigger
    private void OnTriggerEnter2D (Collider2D other) 
    {
        switch (other.gameObject.tag)
        {
            case "Lampada":
            {
                print ("Colidiu com trigger da Lampada");
                break;
            }

            case "Coletavel":
            {
                other.gameObject.SendMessage ("Coletar", SendMessageOptions.DontRequireReceiver);
                break;
            }

            case "Inimigo":
            {
                gameControllerRevisao.playerActualLife -= 1;
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
        // Input.GetAxisRaw () = Pega valores float dos eixos "Horizontal" e "Vertical" de acordo com as setas do teclado */
        horizontal = Input.GetAxisRaw ("Horizontal");
        vertical = Input.GetAxisRaw ("Vertical");

        // Se estiver andando pra frente , olhando pra frente e nao estiver atacando
        if (horizontal > 0 && isLookingLeft == true && isAttacking == false)
        {
            Flip ();
        }
        else if (horizontal < 0 && isLookingLeft == false && isAttacking == false)
        {
            Flip ();
        }

        // Se agachar
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

        // Se apertar o botao de ataque e estiver em pe e nao for objeto de interacao
        if (Input.GetButtonDown ("Fire1") && vertical >= 0 && isAttacking == false && objectInteration == null && !cantAttack)
        {
            cantAttack = true;
            animator.SetTrigger ("attack");
        }

        // Se apertar o botao de ataque e estiver em pe e for objeto de interacao
        if (Input.GetButtonDown ("Fire1") && vertical >= 0 && isAttacking == false && objectInteration != null)
        {
            objectInteration.SendMessage ("Interact", SendMessageOptions.DontRequireReceiver);
        }

        // Se apertar o botao  e estiver no chao
        if (Input.GetButtonDown ("Jump") && isGrounded == true && isAttacking == false)
        {
            // Adiciona forca no corpo rigido em Y
            rigidBody2D.AddForce (new Vector2 (0, jumpForce));
        }

        // Se estiver no chao e atacando, nao pode andar
        horizontal = (isAttacking == true && isGrounded == true ? 0 : horizontal);

        // Faz controle das flechas
        foreach (GameObject arrow in arrows)
        {
            arrow.SetActive (gameControllerRevisao.arrowsQuantity[gameControllerRevisao.equippedArrowID] > 0);
        }

        // Define parametros
        animator.SetBool ("grounded", isGrounded);
        animator.SetInteger ("id_animation", idAnimation);
        animator.SetFloat ("speed_y", rigidBody2D.velocity.y);
        animator.SetFloat ("id_weapon_class", gameControllerRevisao.weaponClassID[gameControllerRevisao.actualWeaponID]);
    }

    // Controla escala local do sprite
    private void Flip ()
    {
        // Inverte posicao e calcula escala para alterar
        isLookingLeft = !isLookingLeft;
        float localScaleX = this.transform.localScale.x;
        localScaleX *= -1;
        this.transform.localScale = new Vector3 (localScaleX, this.transform.localScale.y, this.transform.localScale.z);
        playerDirection.x = localScaleX;
    }

    // Controla ataque
    private void Attack (int value)
    {
        if (value == 0)
        {
            isAttacking = false;
            weapons[2].SetActive (false);
            StartCoroutine ("WaitForNewAttack");
        }
        else if (value == 1)
        {
            audioControllerRevisao.PlayFX (audioControllerRevisao.fxSword, 1f);
            isAttacking = true;
        }
    }

    // Controla ataque do arco - flecha
    private void BowAttack (int value)
    {
        if (value == 0)
        {
            isAttacking = false;
            bows[2].SetActive (false);
            StartCoroutine ("WaitForNewAttack");
        }
        else if (value == 1)
        {
            isAttacking = true;
        }
        else if (value == 2)
        {
            // Instancia flecha
            if (gameControllerRevisao.arrowsQuantity[gameControllerRevisao.equippedArrowID] > 0)
            {
                audioControllerRevisao.PlayFX (audioControllerRevisao.fxBow, 1f);
                gameControllerRevisao.arrowsQuantity[gameControllerRevisao.equippedArrowID]--;
                GameObject arrowTemp = Instantiate (gameControllerRevisao.arrowPrefabs[gameControllerRevisao.equippedArrowID], spawnArrowTransform.position, spawnArrowTransform.localRotation);
                arrowTemp.transform.localScale = new Vector3 (arrowTemp.transform.localScale.x * playerDirection.x, arrowTemp.transform.localScale.y, arrowTemp.transform.localScale.z);
                arrowTemp.GetComponent<Rigidbody2D>().velocity = new Vector2 (gameControllerRevisao.arrowVelocities[gameControllerRevisao.equippedArrowID] * playerDirection.x, 0);
                Destroy (arrowTemp, 2f);
            }
        }
    }

    // Controla ataque do staff
    private void StaffAttack (int value)
    {
        if (value == 0)
        {
            isAttacking = false;
            staffs[3].SetActive (false);
            StartCoroutine ("WaitForNewAttack");
        }
        else if (value == 1)
        {
            isAttacking = true;
        }
        else if (value == 2)
        {
            // Instancia magia
            if (gameControllerRevisao.playerActualMana >= 1)
            {
                audioControllerRevisao.PlayFX (audioControllerRevisao.fxStaff, 1f);
                gameControllerRevisao.playerActualMana--;
                GameObject magicTemp = Instantiate (magicPrefab, spawnMagicTransform.transform.position, spawnMagicTransform.transform.localRotation);
                magicTemp.GetComponent<Rigidbody2D>().velocity = new Vector2 (3 * playerDirection.x, 0);
                Destroy (magicTemp, 1f);
            }
        }
    }

    // Iteracao com outros objetos
    private void Interact ()
    {
        RaycastHit2D hit = Physics2D.Raycast (handTransform.position, playerDirection, 0.1f, layerIteration);
        //Debug.DrawRay (handTransform.position, playerDirection * 0.1f, Color.red);
        objectInteration = (hit ? hit.collider.gameObject : null);
        alertBalloon.SetActive (hit);
    }

    // Controle do objetos (sprites) da arma
    private void WeaponController (int id)
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive (false);
        }

        weapons[id].SetActive (true);
    }

    // Controle do objetos (sprites) do arco-flecha
    private void BowController (int id)
    {
        foreach (GameObject bow in bows)
        {
            bow.SetActive (false);
        }

        bows[id].SetActive (true);
    }

    // Controle do objetos (sprites) do staff
    private void StaffController (int id)
    {
        foreach (GameObject staff in staffs)
        {
            staff.SetActive (false);
        }

        staffs[id].SetActive (true);
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

        foreach (GameObject arrows in arrows)
        {
            arrows.GetComponent<SpriteRenderer>().material = newMaterial;
        }

        foreach (GameObject staff in staffs)
        {
            staff.GetComponent<SpriteRenderer>().material = newMaterial;
        }
    }

    // Muda sprite e valores das armas de acordo do ID informado
    public void ChangeWeapon (int id)
    {
        gameControllerRevisao.weaponID = id;

        switch (gameControllerRevisao.weaponClassID[id])
        {
            // Espadas, Machados, Martelos
            case 0:
            {
                // Atualiza sprites
                weapons[0].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite1[id];
                weapons[1].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite2[id];
                weapons[2].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite3[id];

                // Atualiza dados
                for (int i = 0; i < weapons.Length; i++)
                {
                    ArmaInfoRevisao armaInfoRevisao = weapons[i].GetComponent<ArmaInfoRevisao>();
                    armaInfoRevisao.MinDamage = gameControllerRevisao.minDamages[id];
                    armaInfoRevisao.MaxDamage = gameControllerRevisao.maxDamages[id];
                    armaInfoRevisao.DamageType = gameControllerRevisao.damageTypesInt[id];
                }

                break;
            }

            // Arco
            case 1:
            {
                // Atualiza sprites
                bows[0].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite1[id];
                bows[1].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite2[id];
                bows[2].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite3[id];
                break;
            }

            // Staff
            case 2:
            {
                // Atualiza sprites
                staffs[0].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite1[id];
                staffs[1].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite2[id];
                staffs[2].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite3[id];
                staffs[3].GetComponent<SpriteRenderer>().sprite = gameControllerRevisao.weaponsSprite4[id];
                break;
            }
        }

        gameControllerRevisao.actualWeaponID = weaponID;
    }

    // ------------------- CORROTINAS ------------------- //

    // Espera 'n' segundos para liberar o ataque
    private IEnumerator WaitForNewAttack ()
    {
        yield return new WaitForSeconds (0.2f);
        cantAttack = false;
    }
}