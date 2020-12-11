using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControleDanoInimigoRevisao : MonoBehaviour 
{
    [Header ("Variaveis de Controle")]
    private bool isLookingLeft;
    private bool playerIsLookingLeft;
    private bool wasHit;
    private bool hasDied;

    [Header ("Variaveis de Vida")]
	private int maxLife = 15;
    private int actualLife;
    private float lifePercentual;
    public GameObject damageTextPrefab;
    public GameObject lifebarHolder;
    public Transform lifebarTransform;

    [Header ("Propriedades do Knockback")]
    public GameObject knockforcePrefab;
    public Transform knockforceTransform;
    private float knockforceX = 0.3f;
    private float actualKnockforceX;

    [Header ("Configuração do Chão")]
    public Transform groundCheckTransform;
    public LayerMask whatIsGround;

    [Header ("Configuração de Loot")]
    public List<GameObject> lootPrefabs;

    // Components
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Outros objetos
    private GameControllerRevisao gameControllerRevisao;
    private PlayerRevisao playerRevisao;
    private AudioControllerRevisao audioControllerRevisao;

    [Header ("Configuração de Resistência / Fraqueza")]
    private float[] damageTypeValues;

    // Properties - Getters / Setters

    public float[] DamageTypeValues
    {
        get { return this.damageTypeValues; }
        set { damageTypeValues = value; }
    }

    // ------------------- FUNCOES UNITY ------------------- //

    private void Start ()
    {
        // Recupera instancias - Components
        spriteRenderer = this.GetComponent<SpriteRenderer> ();
        animator = this.GetComponent<Animator>();

        // Recupera instancias - Objetos
        gameControllerRevisao = FindObjectOfType (typeof (GameControllerRevisao)) as GameControllerRevisao;
        playerRevisao = FindObjectOfType<PlayerRevisao>();
        audioControllerRevisao = FindObjectOfType (typeof (AudioControllerRevisao)) as AudioControllerRevisao;

        // Define configuracoes
        actualLife = maxLife;
        lifebarHolder.SetActive (true);
        lifebarHolder.transform.localScale = new Vector3 (1, 1, 1);

        // Passa valores
        DamageTypeValues[0] = 1;
        DamageTypeValues[1] = 5;
        DamageTypeValues[2] = 0;

        // Define escala x do Sprite
        if (isLookingLeft)
        {
            Flip (false);
        }
    }

    private void Update () 
    {
        if (!hasDied)
        {
            // Verifica se o player esta a esquerda ou nao do inimigo
            float xPlayer = playerRevisao.transform.position.x;
            float xSelf = this.transform.position.x;
            playerIsLookingLeft = (xPlayer < xSelf ? true : false);

            // Ajusta posicao do knockback
            if ((playerIsLookingLeft && isLookingLeft) || (!playerIsLookingLeft && !isLookingLeft))
            {
                actualKnockforceX = knockforceX;
            }
            else if ((playerIsLookingLeft && !isLookingLeft) || (!playerIsLookingLeft && isLookingLeft))
            {
                actualKnockforceX = (knockforceX * -1);
            }

            // Atualiza posicao e animator
            knockforceTransform.localPosition = new Vector3 (actualKnockforceX, knockforceTransform.localPosition.y, 0);
            animator.SetBool ("grounded", true);
        }
    }

    // Quando colide com algo
    private void OnTriggerEnter2D (Collider2D other) 
    {
        // Cancela o resto do script
        if (hasDied)
        {
            return;
        }

        switch (other.gameObject.tag)
        {
            case "Arma":
            {
                if (!wasHit)
                {
                    wasHit = true;
                    lifebarHolder.SetActive (true);
                    animator.SetTrigger ("hit");
                    audioControllerRevisao.PlayFX (audioControllerRevisao.fxHit, 1f);

                    // Propriedades da arma e calculo de dano
                    ArmaInfoRevisao armaInfoRevisao = other.gameObject.GetComponent<ArmaInfoRevisao> ();
                    float damage = Random.Range (armaInfoRevisao.MinDamage, armaInfoRevisao.MaxDamage);
                    int damageType = armaInfoRevisao.DamageType;
                    float inflictedDamage = damage + (damage * (damageTypeValues[damageType] / 100));
                    maxLife -= Mathf.RoundToInt (inflictedDamage);

                    // Define percentual e barra
                    lifePercentual = (float) actualLife / (float) maxLife;
                    lifePercentual = (lifePercentual < 0 ? 0 : lifePercentual);
                    lifebarTransform.localScale = new Vector3 (lifePercentual, 1, 1);

                    if (maxLife <= 0)
                    {
                        hasDied = true;
                        animator.SetInteger ("id_animation", 3);
                        StartCoroutine ("SpawnLoot");
                    }
                    else 
                    {
                        // Instancia efeito de dano
                        GameObject effectTemp = Instantiate (gameControllerRevisao.damageEffect[damageType], this.transform.position, this.transform.rotation);
                        Destroy (effectTemp, 1f);

                        // Instancia Text com Dano e define propriedades
                        GameObject damageTemp = Instantiate (damageTextPrefab, this.transform.position, this.transform.rotation) as GameObject;
                        damageTemp.GetComponent<MeshRenderer> ().sortingLayerName = "HUD";
                        damageTemp.GetComponent<TextMeshPro> ().text = Mathf.RoundToInt (inflictedDamage).ToString ();
                        float xForce = 50;
                        xForce *= (!playerIsLookingLeft ? -1 : 1);
                        damageTemp.GetComponent<Rigidbody2D>().AddForce (new Vector2 (xForce, 200));
                        Destroy (damageTemp, 1f);

                        // Inicia animacao e efeito de Knockback
                        GameObject knockbackTemp = Instantiate (knockforcePrefab, knockforceTransform.position, knockforceTransform.rotation) as GameObject;
                        Destroy (knockbackTemp, 0.02f);
                        StartCoroutine ("Invulnerable");
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

    // Inverte sprite e barra
    private void Flip (bool setaBool)
    {
        if (setaBool)
        {
            isLookingLeft = !isLookingLeft;    
        }

        float localScaleX = this.transform.localScale.x;
        localScaleX *= -1;
        this.transform.localScale = new Vector3 (localScaleX, this.transform.localScale.y, this.transform.localScale.z);
        lifebarHolder.transform.localScale = new Vector3 (localScaleX, lifebarHolder.transform.localScale.x, lifebarHolder.transform.localScale.z);
    }

    // ------------------- CORROTINAS ------------------- //

    // "Pisca" o alpha do sprite e seta booleano
    private IEnumerator Invulnerable ()
    {
        Color color = spriteRenderer.color;

        for (int i = 0; i <= 5; i++)
        {
            color.a = (i % 2 == 0 ? 0.05f : 1);
			spriteRenderer.color = color;
            yield return new WaitForSeconds (0.1f);
        }

        wasHit = false;
        lifebarHolder.SetActive (false);
    }

    // Gera 'n' loots quando abre inimigo morre
    private IEnumerator SpawnLoot ()
    {
        this.gameObject.layer = 12;
        yield return new WaitForSeconds (1f);
        GameObject deathEffect = Instantiate (gameControllerRevisao.deathEffect, groundCheckTransform.position, this.transform.localRotation);
        yield return new WaitForSeconds (0.5f);
        spriteRenderer.enabled = false;

        int quantity = Random.Range (1, 5);

        // Escolhe um loot aleatorio com forca aleatorias
        for (int i = 0; i < quantity; i++)
        {
            int chance = Random.Range (0, 100);
            int randomIndex = (chance >= 70 ? 1 : 0);
            GameObject lootTemp = Instantiate (lootPrefabs[randomIndex], this.transform.position, this.transform.localRotation);
            audioControllerRevisao.PlayFX (audioControllerRevisao.fxSpawnLoot, 1f);
            int randomX = Random.Range (-25, 25);
            int randomY = Random.Range (75, 100);
            lootTemp.GetComponent<Rigidbody2D>().AddForce (new Vector2 (randomX, randomY));
            yield return new WaitForSeconds (0.1f);
        }

        yield return new WaitForSeconds (0.7f);
        Destroy (deathEffect);
        Destroy (this.gameObject);
    }
}