using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC1Backup : MonoBehaviour 
{
	// Variaveis de controle
	public int idFala;
	private bool estaDialogando;
	public int idDialogo;
	private bool estaRespondendo;

	// Objetos
	public GameObject canvasNPC;
	public TMP_Text caixaTexto;
	public GameObject painelResposta;

	// Botoes
	private Button btnRespostaA;
	private Button btnRespostaB;

	// Falas e linhas de dialogo
	public string[] falas0;
	public string[] falas1;
	public string[] falas2;
	public string[] falas3;
	public List<string> linhasDialogo;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Awake () 
	{
		// Inicializa botoes e eventos
		if (GameObject.Find ("Button-Resposta-A") != null)	
		{
			btnRespostaA = GameObject.Find ("Button-Resposta-A").GetComponent<Button>();
			btnRespostaA.onClick.AddListener (RespostaA);
		}

		if (GameObject.Find ("Button-Resposta-B") != null)	
		{
			btnRespostaB = GameObject.Find ("Button-Resposta-B").GetComponent<Button>();
			btnRespostaB.onClick.AddListener (RespostaB);
		}
	}

	private void Start () 
	{
		// Desabilita paineis
		canvasNPC.SetActive (false);
		painelResposta.SetActive (false);
	}

	// ------------------- FUNCOES ------------------- //

	public void Interact ()
	{
		if (!estaDialogando)
		{
			// Define parametros
			idFala = 0;
			PrepararDialogo ();
			Dialogar ();
			canvasNPC.SetActive (true);
			estaDialogando = true;
		}
		else if (estaDialogando && !estaRespondendo)
		{
			idFala += 1;
			Dialogar ();
		}
	}

	public void Dialogar ()
	{
		if (idFala < linhasDialogo.Count)
		{
			caixaTexto.text = linhasDialogo[idFala];

			if (idDialogo == 0 && idFala == 2)
			{
				painelResposta.SetActive (true);
				estaRespondendo = true;
			}
		}
		else // Encerra a conversa
		{
			switch (idDialogo)
			{
				case 0:
				{
					//estaRespondendo = true;
					//painelResposta.SetActive (true);
					break;
				}

				case 1:
				{
					idDialogo = 3;
					break;
				}
				
				case 2:
				{
					idDialogo = 0;
					break;
				}

				default:
				{
					break;
				}
			}

			canvasNPC.SetActive (false);
			estaDialogando = false;
		}
	}

	public void PrepararDialogo ()
	{
		linhasDialogo.Clear ();
		switch (idDialogo)
		{
			case 0:
			{
				foreach (string fala in falas0)
				{
					linhasDialogo.Add (fala);
				}

				break;
			}

			case 1:
			{
				foreach (string fala in falas1)
				{
					linhasDialogo.Add (fala);
				}

				break;
			}

			case 2:
			{
				foreach (string fala in falas2)
				{
					linhasDialogo.Add (fala);
				}

				break;
			}

			case 3:
			{
				foreach (string fala in falas3)
				{
					linhasDialogo.Add (fala);
				}

				break;
			}

			default:
			{
				break;
			}
		}
	}

	public void RespostaA ()
	{
		idDialogo = 1;
		idFala = 0;
		PrepararDialogo ();
		Dialogar ();
		estaRespondendo = false;
		painelResposta.SetActive (false);
	}

	public void RespostaB ()
	{
		idDialogo = 2;
		idFala = 0;
		PrepararDialogo ();
		Dialogar ();
		estaRespondendo = false;
		painelResposta.SetActive (false);
	}
}