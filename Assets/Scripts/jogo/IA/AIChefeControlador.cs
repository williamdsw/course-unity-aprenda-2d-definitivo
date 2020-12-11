using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChefeControlador : MonoBehaviour 
{
	public enum Rotinas
	{
		RotinaA, 
		RotinaB
	}

	// Components
	private Rigidbody2D rigidBody2D;
	//private Animator animator;

	// Variaveis de controle
	public float velocidade;
	private int horizontal;
	public Rotinas rotinaAtual;
	private int idEtapa;
	private float tempoAcumulado;
	private float tempoDeEspera;
	public bool estaOlhandoAEsquerda;
	public bool podeAndar;
	public bool estaNoChao;

	// Posicoes para ir
	public Transform[] posicoes;
	private Transform destino;

	// Outros objetos
	public Transform groundCheck;
	public LayerMask layerChao;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{	
		// Recupera components	
		this.rigidBody2D = this.GetComponent<Rigidbody2D>();
		//this.animator = this.GetComponent<Animator>();

		ResetarValores (Rotinas.RotinaA, 0, 0, 3f);
	}

	private void Update () 
	{
		DefineRotinas ();

		// Implementa Flip
		if (horizontal > 0 && estaOlhandoAEsquerda)
		{
			Flip ();
		}
		else if (horizontal < 0 && !estaOlhandoAEsquerda)
		{
			Flip ();
		}
	}

	private void FixedUpdate () 
	{
		// Verifica colisao com chao
		estaNoChao = Physics2D.OverlapCircle (groundCheck.position, 0.02f, layerChao);

		// Movimenta
		if (podeAndar)
		{
			this.rigidBody2D.velocity = new Vector2 (horizontal * velocidade, this.rigidBody2D.velocity.y);	
		}

		// Atualiza animator
		//animator.SetInteger ("horizontal", horizontal);
	}

	// ------------------- FUNCOES ------------------- //

	// Define rotinas do chefe
	private void DefineRotinas ()
	{
		switch (rotinaAtual)
		{
			case Rotinas.RotinaA:
			{
				switch (idEtapa)
				{
					// Espera 3 segundos, determina o destino com a posicao B
					case 0:
					{
						tempoAcumulado += Time.deltaTime;

						if (tempoAcumulado >= tempoDeEspera)
						{
							idEtapa++;
							destino = posicoes[1];
							horizontal = -1;
							podeAndar = true;
						}

						break;
					}
					
					// Move ate o destino
					case 1:
					{
						if (this.transform.position.x <= destino.transform.position.x)
						{
							idEtapa++;
							horizontal = 0;
							tempoAcumulado = 0;
							tempoDeEspera = 3f;
						}

						break;
					}

					// Espera 3 segundos e define outro destino
					case 2:
					{
						tempoAcumulado += Time.deltaTime;

						if (tempoAcumulado >= tempoDeEspera)
						{
							idEtapa++;
							destino = posicoes[0];
							horizontal = 1;
						}

						break;
					}

					// Move ate o destino (FIM DA ROTINA)
					case 3:
					{
						if (this.transform.position.x >= destino.transform.position.x)
						{
							horizontal = 0;
							ResetarValores (Rotinas.RotinaB, 0, 0, 3f);
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
			
			case Rotinas.RotinaB:
			{

				switch (idEtapa)
				{
					// Espera 3 segundos, determina o destino com a posicao B
					case 0:
					{
						tempoAcumulado += Time.deltaTime;

						if (tempoAcumulado >= tempoDeEspera)
						{
							idEtapa++;
							destino = posicoes[1];
							horizontal = -1;
							podeAndar = true;
						}

						break;
					}
					
					// Move ate o destino
					case 1:
					{
						if (this.transform.position.x <= destino.transform.position.x)
						{
							idEtapa++;
							horizontal = 0;
							tempoAcumulado = 0;
							tempoDeEspera = 3f;
						}

						break;
					}

					// Espera 3 segundos e define outro destino
					case 2:
					{
						tempoAcumulado += Time.deltaTime;

						if (tempoAcumulado >= tempoDeEspera)
						{
							idEtapa++;
							destino = posicoes[2];
							horizontal = 1;
						}

						break;
					}

					// Move ate o destino e pula
					case 3:
					{
						if (this.transform.position.x >= destino.transform.position.x)
						{
							horizontal = 0;
							idEtapa++;
							rigidBody2D.AddForce (new Vector2 (0, 300));
							tempoAcumulado = 0;
							tempoDeEspera = 1f;
						}

						break;
					}

					// Espera 1 segundo
					case 4:
					{
						tempoAcumulado += Time.deltaTime;

						if (tempoAcumulado >= tempoDeEspera)
						{
							idEtapa += 1;
						}

						break;
					}

					// Spawna goblins
					case 5:
					{
						print ("Invocar goblins");
						tempoAcumulado = 0;
						tempoDeEspera = 5f;
						idEtapa++;
						break;
					}

					// Fica parado e pula
					case 6:
					{
						tempoAcumulado += Time.deltaTime;

						if (tempoAcumulado >= tempoDeEspera)
						{
							idEtapa++;
							podeAndar = false;
							rigidBody2D.AddForce (new Vector2 (100, 250));
							tempoAcumulado = 0;
							tempoDeEspera = 1f;
						}

						break;
					}

					// Define nova posicao e anda
					case 7:
					{
						tempoAcumulado += Time.deltaTime;

						if (tempoAcumulado >= tempoDeEspera)
						{
							if (estaNoChao)
							{
								idEtapa++;
								podeAndar = true;
								destino = posicoes[2];
								horizontal = -1;
							}
						}

						break;
					}

					// Escolhe uma acao aleatoria
					case 8:
					{
						if (this.transform.position.x <= destino.transform.position.x)
						{
							int random = Random.Range (0, 100);

							if (random <= 50)
							{
								destino = posicoes[0];
								horizontal = 1;
								idEtapa = 9;
							}
							else 
							{
								destino = posicoes[1];
								horizontal = -1;
								idEtapa = 10;
							}
						}

						break;
					}

					// Se for para o ponto A
					case 9:
					{
						if (this.transform.position.x >= destino.transform.position.x)
						{
							horizontal = 0;
							ResetarValores (Rotinas.RotinaA, 0, 0, 3f);
						}

						break;
					}

					// Se for para o ponto B
					case 10:
					{
						if (this.transform.position.x <= destino.transform.position.x)
						{
							horizontal = 0;
							ResetarValores (Rotinas.RotinaA, 0, 0, 3f);
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

			default:
			{
				break;
			}
		}
	}

	// Inverte posicao
	private void Flip ()
	{
		estaOlhandoAEsquerda = !estaOlhandoAEsquerda;
		float localScaleX = (this.transform.localScale.x * -1);
		this.transform.localScale = new Vector3 (localScaleX, this.transform.localScale.y, this.transform.localScale.z);
	}

	// Reseta valores padraos
	private void ResetarValores (Rotinas pRotinaAtual, int pIdEtapa, float pTempoArmazenado, float pTempoDeEspera)
	{
		// Valores padrao
		this.rotinaAtual = pRotinaAtual;
		this.idEtapa = pIdEtapa;
		this.tempoAcumulado = pTempoArmazenado;
		this.tempoDeEspera = pTempoDeEspera;
	}
}