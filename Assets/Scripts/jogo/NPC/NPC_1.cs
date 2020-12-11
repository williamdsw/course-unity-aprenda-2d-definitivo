using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC_1 : MonoBehaviour 
{
	// Variaveis de controle
	public int idFala;
	private bool estaDialogando;
	public int idDialogo;
	private bool estaRespondendo;
	public string nomeArquivoXML;

	// Objetos
	public GameObject canvasNPC;
	public TMP_Text caixaTexto;
	public GameObject painelResposta;
	private GameController gameController;

	// Botoes
	private Button btnRespostaA;
	private Button btnRespostaB;
	private TMP_Text respostaAText;
	private TMP_Text respostaBText;

	// Falas e linhas de dialogo
	public List<string> falas0;
	public List<string> falas1;
	public List<string> falas2;
	public List<string> falas3;
	public List<string> falas4;					// Fala ao concluir conversa
	public List<string> falas5;					// Ultima fala
	public List<string> respostaFala0;
	public List<string> linhasDialogo;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Awake () 
	{
		// Inicializa botoes e eventos
		if (GameObject.Find ("Button-Resposta-A") != null)	
		{
			btnRespostaA = GameObject.Find ("Button-Resposta-A").GetComponent<Button>();
			respostaAText = GameObject.Find ("RespostaAText").GetComponent<TMP_Text>();
			btnRespostaA.onClick.AddListener (RespostaA);
		}

		if (GameObject.Find ("Button-Resposta-B") != null)	
		{
			btnRespostaB = GameObject.Find ("Button-Resposta-B").GetComponent<Button>();
			respostaBText = GameObject.Find ("RespostaBText").GetComponent<TMP_Text>();
			btnRespostaB.onClick.AddListener (RespostaB);
		}
	}

	private void Start () 
	{
		gameController = FindObjectOfType<GameController>();

		// Desabilita paineis
		canvasNPC.SetActive (false);
		painelResposta.SetActive (false);

		CarregarXML ();
	}

	// ------------------- FUNCOES ------------------- //

	public void Interact ()
	{
		if (gameController.estadoAtual == GameState.GAMEPLAY)
		{
			// Define parametros
			idFala = 0;

			// Verifica se a missao for concluida
			if (idDialogo == 3 && gameController.missao1Finalizada)
			{
				idDialogo = 4;
			}

			// Chama funcoes
			PrepararDialogo ();
			Dialogar ();

			// Outros parametros
			canvasNPC.SetActive (true);
			estaDialogando = true;
			gameController.MudarEstado (GameState.DIALOGO);
		}
	}

	// Incrementa fala e chama dialogo
	public void Falar ()
	{
		if (estaDialogando && !estaRespondendo)
		{
			idFala++;
			Dialogar ();
		}
	}

	// Escreve os dialogos na tela
	public void Dialogar ()
	{
		if (idFala < linhasDialogo.Count)
		{
			// Para corrotina e chama novamente
			caixaTexto.text = string.Empty;
			StopCoroutine ("ExibeFrase");
			StartCoroutine ("ExibeFrase");

			if (idDialogo == 0 && idFala == 2)
			{
				// Atualiza texto dos botoes
				respostaAText.text = respostaFala0[0];
				respostaBText.text = respostaFala0[1];

				// Elementos do painel
				painelResposta.SetActive (true);
				btnRespostaA.Select ();
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

				case 3:
				{
					break;
				}

				case 4:
				{
					idDialogo = 5;
					break;
				}

				default:
				{
					break;
				}
			}

			// Propriedades
			canvasNPC.SetActive (false);
			estaDialogando = false;
			gameController.MudarEstado (GameState.FIM_DIALOGO);
		}
	}

	// Limpa o lista e preenche novamente
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

			case 4:
			{
				foreach (string fala in falas4)
				{
					linhasDialogo.Add (fala);
				}

				break;
			}

			case 5:
			{
				foreach (string fala in falas5)
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

	// Comandos do botao A "Sim" da resposta
	public void RespostaA ()
	{
		idDialogo = 1;
		idFala = 0;
		PrepararDialogo ();
		Dialogar ();
		estaRespondendo = false;
		painelResposta.SetActive (false);
	}

	// Comandos do botao B "Nao" da resposta
	public void RespostaB ()
	{
		idDialogo = 2;
		idFala = 0;
		PrepararDialogo ();
		Dialogar ();
		estaRespondendo = false;
		painelResposta.SetActive (false);
	}

	// Esta funcao ira ler o arquivo XML de dialogo do NPC
	private void CarregarXML ()
	{
		// Caminho do arquivo
		string idiomaFolder = gameController.idiomaFolders[gameController.idIdioma];
		nomeArquivoXML = string.Concat ("XML/", idiomaFolder, "/", nomeArquivoXML);

		// Carrega o arquivo convertendo para "TextAsset"
		TextAsset xmlData = (TextAsset) Resources.Load (nomeArquivoXML);
		
		// Cria documento XML e carrega 
		XmlDocument document = new XmlDocument ();
		document.LoadXml (xmlData.text);

		// Itera "dialogos"
		foreach (XmlNode dialogo in document["dialogos"].ChildNodes)
		{
			string nome = dialogo.Attributes["name"].Value;

			// Itera "falas"
			foreach (XmlNode fala in dialogo["falas"].ChildNodes)
			{
				switch (nome)
				{
					case "fala0":
					{
						falas0.Add (gameController.TextoFormatado (fala.InnerText));
						break;
					}

					case "fala1":
					{
						falas1.Add (gameController.TextoFormatado (fala.InnerText));
						break;
					}

					case "fala2":
					{
						falas2.Add (gameController.TextoFormatado (fala.InnerText));
						break;
					}

					case "fala3":
					{
						falas3.Add (gameController.TextoFormatado (fala.InnerText));
						break;
					}

					case "fala4":
					{
						falas4.Add (gameController.TextoFormatado (fala.InnerText));
						break;
					}

					case "fala5":
					{
						falas5.Add (gameController.TextoFormatado (fala.InnerText));
						break;
					}

					case "resposta0":
					{
						respostaFala0.Add (gameController.TextoFormatado (fala.InnerText));
						break;
					}

					default:
					{
						break;
					}
				}
			}
		}
	}

	// ------------------- CORROTINAS ------------------- //

	// Itera o dialogo e escreve cada palavra com delay de 'n' segundos
	private IEnumerator ExibeFrase ()
	{
		foreach (char palavra in linhasDialogo[idFala])
		{
			caixaTexto.text += palavra;
			yield return new WaitForSeconds (0.05f);	
		}
	}
} 