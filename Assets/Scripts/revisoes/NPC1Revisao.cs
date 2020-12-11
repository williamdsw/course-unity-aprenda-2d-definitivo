using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC1Revisao : MonoBehaviour 
{
	// Variaveis de Controle
	public int speechID;
	public int dialogID;
	private bool isDialoguing;
	private bool isResponding;
	public string xmlFileName;

	// Objetos
	public GameObject npcCanvas;
	public TMP_Text dialogBox;
	public GameObject answerPanel;
	private GameControllerRevisao gameControllerRevisao;

	// Botoes
	private Button buttonYes;
	private Button buttonNo;
	private TMP_Text buttonYesText;
	private TMP_Text buttonNoText;

	// Falas e linhas de dialogo
	public List<string> speech0;
	public List<string> speech1;
	public List<string> speech2;
	public List<string> speech3;
	public List<string> speech4;
	public List<string> speech5;
	public List<string> answer0;
	public List<string> dialogLines;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Awake () 
	{
		// Inicializa botoes e eventos
		if (GameObject.Find ("Button-Yes") != null)
		{
			buttonYes = GameObject.Find ("Button-Yes").GetComponent<Button>();
			buttonYesText = GameObject.Find ("Button-Yes-Text").GetComponent<TMP_Text>();
			buttonYes.onClick.AddListener (YesAnswer);
		}	

		if (GameObject.Find ("Button-No") != null)
		{
			buttonNo = GameObject.Find ("Button-No").GetComponent<Button>();
			buttonNoText = GameObject.Find ("Button-No-Text").GetComponent<TMP_Text>();
			buttonNo.onClick.AddListener (NoAnswer);
		}	
	}

	private void Start () 
	{
		// Inicializa
		gameControllerRevisao = FindObjectOfType (typeof (GameControllerRevisao)) as GameControllerRevisao;

		// Desabilita paineis
		npcCanvas.SetActive (false);
		answerPanel.SetActive (false);

		LoadXML ();
	}

	// ------------------- FUNCOES ------------------- //

	public void Interact ()
	{
		if (gameControllerRevisao.actualGameState == GameStateRevisao.GAMEPLAY)
		{
			// Define parametros
			speechID = 0;

			// Verifica se a missao for concluida
			if (speechID == 3 && gameControllerRevisao.mission1Finished)
			{
				speechID = 4;
			}

			// Chama funcoes
			PrepareDialogue ();
			Dialogue ();

			// Outros parametros
			npcCanvas.SetActive (true);
			isDialoguing = true;
			gameControllerRevisao.ChangeGameState (GameStateRevisao.BEGIN_DIALOGUE);

		}
	}

	// Incrementa fala e chama dialogo
	public void Speak ()
	{
		if (!isDialoguing && isResponding)
		{
			speechID++;
			Dialogue ();
		}
	}

	// Escreve os dialogos na tela
	public void Dialogue ()
	{
		if (speechID < dialogLines.Count)
		{
			// Para corrotina e chama novamente
			dialogBox.text = string.Empty;
			StopCoroutine ("WriteSpeech");
			StartCoroutine ("WriteSpeech");

			if (dialogID == 0 && speechID == 2)
			{
				// Atualiza texto dos botoes
				buttonYesText.text = answer0[0];
				buttonNoText.text = answer0[1];

				// Elementos do painel
				answerPanel.SetActive (true);
				buttonYes.Select ();
				isResponding = true;
			}
		}
		else
		{
			// Encerra a conversa
			if (dialogID == 1)
			{
				dialogID = 3;
			}
			else if (dialogID == 2)
			{
				dialogID = 0;
			}
			else if (dialogID == 4)
			{
				dialogID = 5;
			}

			// Propriedades
			npcCanvas.SetActive (false);
			isDialoguing = false;
			gameControllerRevisao.ChangeGameState (GameStateRevisao.END_DIALOGUE);
		}
	}

	// Limpa o lista e preenche novamente
	public void PrepareDialogue ()
	{
		dialogLines.Clear ();

		switch (dialogID)
		{
			case 0:
			{
				FillDialogueList (speech0);
				break;
			}
			
			case 1:
			{
				FillDialogueList (speech1);
				break;
			}

			case 2:
			{
				FillDialogueList (speech2);
				break;
			}

			case 3:
			{
				FillDialogueList (speech3);
				break;
			}

			case 4:
			{
				FillDialogueList (speech4);
				break;
			}

			case 5:
			{
				FillDialogueList (speech5);
				break;
			}

			default:
			{
				break;
			}
		}
	}

	// Preenche a lista de dialogos com falas
	private void FillDialogueList (List<string> speechs)
	{
		foreach (string speech in speechs)
		{
			dialogLines.Add (speech);
		}
	}

	// Comandos do botao A "Sim" da resposta
	public void YesAnswer ()
	{
		dialogID = 1;
		speechID = 0;
		PrepareDialogue ();
		Dialogue ();
		isResponding = true;
		answerPanel.SetActive (false);
	}

	// Comandos do botao B "Nao" da resposta
	public void NoAnswer ()
	{
		dialogID = 2;
		speechID = 0;
		PrepareDialogue ();
		Dialogue ();
		isResponding = true;
		answerPanel.SetActive (false);
	}

	// Esta funcao ira ler o arquivo XML de dialogo do NPC
	public void LoadXML ()
	{
		// Caminho do arquivo
		string languageFolder = gameControllerRevisao.languageFolders[gameControllerRevisao.languageID];
		xmlFileName = string.Concat ("XML/", languageFolder, "/", xmlFileName);

		// Carrega o arquivo convertendo para "TextAsset"
		TextAsset xmlData = (TextAsset) Resources.Load (xmlFileName);

		// Cria documento XML e carrega 
		XmlDocument document = new XmlDocument ();
		document.LoadXml (xmlData.text);

		// Itera "dialogos"
		foreach (XmlNode dialogue in document["dialogue"].ChildNodes)
		{
			string name = dialogue.Attributes["name"].Value;

			// Itera "falas"
			foreach (XmlNode speech in dialogue["speech"].ChildNodes)
			{
				switch (name)
				{
					case "speech0":
					{
						speech0.Add (gameControllerRevisao.FormatText (speech.InnerText));
						break;
					}
					
					case "speech1":
					{
						speech1.Add (gameControllerRevisao.FormatText (speech.InnerText));
						break;
					}
					
					case "speech2":
					{
						speech2.Add (gameControllerRevisao.FormatText (speech.InnerText));
						break;
					}
					
					case "speech3":
					{
						speech3.Add (gameControllerRevisao.FormatText (speech.InnerText));
						break;
					}
					
					case "speech4":
					{
						speech4.Add (gameControllerRevisao.FormatText (speech.InnerText));
						break;
					}
					
					case "speech5":
					{
						speech5.Add (gameControllerRevisao.FormatText (speech.InnerText));
						break;
					}
					
					case "answer1":
					{
						answer0.Add (gameControllerRevisao.FormatText (speech.InnerText));
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
	private IEnumerator WriteSpeech ()
	{
		foreach (char letter in dialogLines[dialogID])
		{
			dialogBox.text += letter;
			yield return new WaitForSeconds (0.05f);
		}
	}
}