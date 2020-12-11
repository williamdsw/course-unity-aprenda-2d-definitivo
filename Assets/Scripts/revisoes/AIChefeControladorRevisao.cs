using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChefeControladorRevisao : MonoBehaviour
{
    // Rotinas do chefe
    public enum Routines
    {
        A, B, C, D
    }

    // Components
    private Rigidbody2D rigidBody2D;
    private Animator animator;

    // Variaveis de controle
    public float speed;
    private int horizontal;
    public Routines actualRoutine;
    private int stepID;
    private float accumulatedTime;
    private float waitingTime;
    private bool isLookingLeft;
    private bool canWalk;
    private bool isOnGround;

    // Posicoes para ir
    public Transform[] nextPositions;
    private Transform destiny;

    // Outros objetos
    public Transform groundCheck;
    public LayerMask layerGround;

    // ------------------- FUNCOES UNITY ------------------- //

    private void Start () 
    {
        // Recupera components	
        rigidBody2D = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        ResetValues (Routines.A, 0, 0, 3f);
    }

    private void Update () 
    {
        DefineRoutines ();

        // Implementa Flip
        if (horizontal > 0 && isLookingLeft)
        {
            Flip ();
        }
        else if (horizontal < 0 && !isLookingLeft)
        {
            Flip ();
        }
    }

    private void FixedUpdate () 
    {
        // Verifica colisao com chao
        isOnGround = Physics2D.OverlapCircle (groundCheck.position, 0.02f, layerGround);

        // Movimenta
        if (canWalk)
        {
            rigidBody2D.velocity = new Vector2 (horizontal * speed, rigidBody2D.velocity.y);
        }

        // Atualiza animator
        this.animator.SetInteger ("horizontal", horizontal);
    }

    // ------------------- FUNCOES ------------------- //

    // Define rotinas do chefe
    private void DefineRoutines ()
    {
        switch (actualRoutine)
        {
            case Routines.A:
            {
                switch (stepID)
                {
                    // Espera 3 segundos, determina o destino com a posicao B
                    case 0:
                    {
                        accumulatedTime += Time.deltaTime;
                        
                        if (accumulatedTime >= waitingTime)
                        {
                            stepID++;
                            destiny = nextPositions[1];
                            horizontal = -1;
                            canWalk = true;
                        }

                        break;
                    }
                    
                    // Move ate o destino
                    case 1:
                    {
                        if (this.transform.position.x <= destiny.transform.position.x)
                        {
                            stepID++;
                            horizontal = 0;
                            accumulatedTime = 0;
                            waitingTime = 3f;
                        }

                        break;
                    }

                    // Espera 3 segundos e define outro destino
                    case 2:
                    {
                        accumulatedTime += Time.deltaTime;

                        if (accumulatedTime >= waitingTime)
                        {
                            stepID++;
                            destiny = nextPositions[0];
                            horizontal = 1;
                        }

                        break;
                    }

                    // Move ate o destino (FIM DA ROTINA)
                    case 3:
                    {
                        if (this.transform.position.x >= destiny.transform.position.x)
                        {
                            horizontal = 0;
                            ResetValues (Routines.B, 0, 0, 3f);
                        }

                        break;
                    }

                    default:
                    {
                        break;
                    }
                }

                break;
            }
            
            case Routines.B:
            {
                switch (stepID)
                {
                    // Espera 3 segundos, determina o destino com a posicao B
                    case 0:
                    {
                        accumulatedTime += Time.deltaTime;
                        
                        if (accumulatedTime >= waitingTime)
                        {
                            stepID++;
                            destiny = nextPositions[1];
                            horizontal = -1;
                            canWalk = true;
                        }

                        break;
                    }
                    
                    // Move ate o destino
                    case 1:
                    {
                        if (this.transform.position.x <= destiny.transform.position.x)
                        {
                            stepID++;
                            horizontal = 0;
                            accumulatedTime = 0;
                            waitingTime = 3f;
                        }

                        break;
                    }

                    // Espera 3 segundos e define outro destino
                    case 2:
                    {
                        accumulatedTime += Time.deltaTime;

                        if (accumulatedTime >= waitingTime)
                        {
                            stepID++;
                            destiny = nextPositions[2];
                            horizontal = 1;
                        }

                        break;
                    }
                    
                    // Move ate o destino e pula
                    case 3:
                    {
                        if (this.transform.position.x >= destiny.transform.position.x)
                        {
                            horizontal = 0;
                            stepID++;
                            rigidBody2D.AddForce (new Vector2 (0, 300));
                            accumulatedTime = 0;
                            waitingTime = 1f;
                        }

                        break;
                    }
                    
                    // Espera 1 segundo
                    case 4:
                    {
                        accumulatedTime += Time.deltaTime;

                        if (accumulatedTime >= waitingTime)
                        {
                            stepID++;
                        }

                        break;
                    }
                    
                    // Spawna goblins
                    case 5:
                    {
                        print ("Criando goblins");
                        stepID++;
                        accumulatedTime = 0;
                        waitingTime = 5f;

                        break;
                    }

                    // Fica parado e pula
                    case 6:
                    {
                        accumulatedTime += Time.deltaTime;

                        if (accumulatedTime >= waitingTime)
                        {
                            stepID++;
                            canWalk = false;
                            rigidBody2D.AddForce (new Vector2 (100, 250));
                            accumulatedTime = 0;
                            waitingTime = 1f;
                        }

                        break;
                    }

                    // Define nova posicao e anda
                    case 7:
                    {
                        accumulatedTime += Time.deltaTime;

                        if (accumulatedTime >= waitingTime)
                        {
                            if (isOnGround)
                            {
                                stepID++;
                                canWalk = true;
                                destiny = nextPositions[1];
                                horizontal = -1;
                            }
                        }

                        break;
                    }

                    // Escolhe uma acao aleatoria
                    case 8:
                    {
                        if (this.transform.position.x <= destiny.transform.position.x)
                        {
                            int random = Random.Range (0, 100);

                            if (random <= 50)
                            {
                                destiny = nextPositions[1];
                                horizontal = 1;
                                stepID = 9;
                            }
                            else
                            {
                                destiny = nextPositions[0];
                                horizontal = -1;
                                stepID = 10;
                            }
                        }

                        break;
                    }
                    
                    // Se for para o ponto A
                    case 9:
                    {
                        if (this.transform.position.x >= destiny.transform.position.x)
                        {
                            horizontal = 0;
                            ResetValues (Routines.A, 0, 0, 3f);
                        }

                        break;
                    }
                    
                    // Se for para o ponto B
                    case 10:
                    {
                        if (this.transform.position.x <= destiny.transform.position.x)
                        {
                            horizontal = 0;
                            ResetValues (Routines.A, 0, 0, 3f);
                        }

                        break;
                    }

                    default:
                    {
                        break;
                    }
                }

                break;
            }

            case Routines.C:
            {
                break;
            }

            case Routines.D:
            {
                break;
            }

            default:
            {
                break;
            }
        }
    }

    // Inverte posicao e sprite
    private void Flip ()
    {
        isLookingLeft = !isLookingLeft;
        float localScaleX = (this.transform.localScale.x * -1);
        this.transform.localScale = new Vector3 (localScaleX, this.transform.localScale.y, this.transform.localScale.z);
    }

    // Reseta valores 
    private void ResetValues (Routines pNewRoutine, int pStepID, float pAccumulatedTime, float pWaitingTime)
    {
        this.actualRoutine = pNewRoutine;
        this.stepID = pStepID;
        this.accumulatedTime = pAccumulatedTime;
        this.waitingTime = pWaitingTime;
    }
}