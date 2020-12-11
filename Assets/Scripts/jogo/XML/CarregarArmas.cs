using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class CarregarArmas : MonoBehaviour 
{
	// Variaveis de controle
	public string nomeArquivoXML;

	// Objetos
	private GameController gameController;

	[Header ("Informações sobre as armas")]
	public List<string> nome;					// Nome da arma
	public List<string> nomeIcones;				// Nome dos icones no spritesheet
	public List<Sprite> iconesArmas;			// Icone exibido no inventario e loja
	public List<string> categorias;				// Espada, machado, martelo, arco, cajado, maca
	public List<int> idClasseArma;
	public List<int> danosMinimos;				// Dano minimo causado pela arma
	public List<int> danosMaximos;				// Dano maximo causado pela arma
	public List<int> tipoDanos;					// Tipo de dano causado
	public List<Sprite> spriteArmas1;
	public List<Sprite> spriteArmas2;
	public List<Sprite> spriteArmas3;
	public List<Sprite> spriteArmas4;

	// Variaveis temporarias
	public List<Sprite> todosSprites;		// Armazena todos sprites
	public Sprite[] spritesIcones;
	public Sprite[] spritesEspadas;
	public Sprite[] spritesMachados;
	public Sprite[] spritesArcos;
	public Sprite[] spritesMacas;
	public Sprite[] spritesMartelos;
	public Sprite[] spritesCajados;
	private Dictionary<string, Sprite> spriteSheetArmas;

	[Header ("SpriteSheets")]
	public Texture spriteSheetIcones;
	public Texture spriteSheetEspadas;
	public Texture spriteSheetMachados;
	public Texture spriteSheetArcos;
	public Texture spriteSheetMacas;
	public Texture spriteSheetMartelos;
	public Texture spriteSheetCajados;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		gameController = FindObjectOfType<GameController>();	
		CarregarSprites ();
		CarregarXML ();
		CarregarSpritesArmas ();
		AtualizarGameController ();
	}

	// ------------------- FUNCOES ------------------- //

	// Responsavel por carregar Sprites
	private void CarregarSprites ()
	{
		// Carrega os sprites
		string caminho = "Sprites/Armas/";
		spritesIcones = Resources.LoadAll<Sprite> (string.Concat (caminho, spriteSheetIcones.name));
		spritesEspadas = Resources.LoadAll<Sprite> (string.Concat (caminho, spriteSheetEspadas.name));
		spritesMachados = Resources.LoadAll<Sprite> (string.Concat (caminho, spriteSheetMachados.name));
		spritesArcos = Resources.LoadAll<Sprite> (string.Concat (caminho, spriteSheetArcos.name));
		spritesMacas = Resources.LoadAll<Sprite> (string.Concat (caminho, spriteSheetMacas.name));
		spritesMartelos = Resources.LoadAll<Sprite> (string.Concat (caminho, spriteSheetMartelos.name));
		spritesCajados = Resources.LoadAll<Sprite> (string.Concat (caminho, spriteSheetCajados.name));

		// Preenche lista com todos sprites
		
		foreach (Sprite sprite in spritesEspadas)
		{
			todosSprites.Add (sprite);
		}

		foreach (Sprite sprite in spritesMachados)
		{
			todosSprites.Add (sprite);
		}

		foreach (Sprite sprite in spritesArcos)
		{
			todosSprites.Add (sprite);
		}

		foreach (Sprite sprite in spritesMacas)
		{
			todosSprites.Add (sprite);
		}

		foreach (Sprite sprite in spritesMartelos)
		{
			todosSprites.Add (sprite);
		}

		foreach (Sprite sprite in spritesCajados)
		{
			todosSprites.Add (sprite);
		}

		// Adiciona sprites para Dictionary
		spriteSheetArmas = todosSprites.ToDictionary (sprite => sprite.name, sprite => sprite);
	}

	// Responsavel pela leitura do XML
	private void CarregarXML ()
	{
		string caminho = string.Concat ("XML/", gameController.idiomaFolders[gameController.idIdioma], "/");
		caminho = string.Concat (caminho, nomeArquivoXML);

		// Carrega o arquivo XML e carrega-o como documento XML
		TextAsset dadosXML = (TextAsset) Resources.Load (caminho);
		XmlDocument documento = new XmlDocument ();
		documento.LoadXml (dadosXML.text);

		// Itera tag root
		foreach (XmlNode armas in documento["Armas"].ChildNodes)
		{
			// Recupera atributo
			string atributo = armas.Attributes["atributo"].Value;

			// Itera tags
			foreach (XmlNode arma in armas["armas"].ChildNodes)
			{
				switch (atributo)
				{
					case "nome":
					{
						nome.Add (arma.InnerText);
						break;
					}
					
					case "icone":
					{
						nomeIcones.Add (arma.InnerText);

						// Itera sprites e coloca icone na lista caso encontre pelo nome
						for (int i = 0; i < spritesIcones.Length; i++)
						{
							if (spritesIcones[i].name.Equals (arma.InnerText))
							{
								iconesArmas.Add (spritesIcones[i]);
								break;								// para o comando para otimizar
							}
						}

						break;
					}

					case "categoria":
					{
						categorias.Add (arma.InnerText);

						// Define tipo de ID da classe
						if (arma.InnerText.Equals ("Staff"))
						{
							idClasseArma.Add (2);
						}
						else if (arma.InnerText.Equals ("Arco"))
						{
							idClasseArma.Add (1);
						}
						else
						{
							idClasseArma.Add (0);
						}

						break;
					}

					case "dano-minimo":
					{
						danosMinimos.Add (int.Parse (arma.InnerText));
						break;
					}
					case "dano-maximo":
					{
						danosMaximos.Add (int.Parse (arma.InnerText));
						break;
					}

					case "tipo-dano":
					{
						tipoDanos.Add (int.Parse (arma.InnerText));
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

	// Carrega sprites das armas dinamicamente
	private void CarregarSpritesArmas ()
	{
		for (int i = 0; i < iconesArmas.Count; i++)
		{
			spriteArmas1.Add (spriteSheetArmas[string.Concat (nomeIcones[i], "0")]);
			spriteArmas2.Add (spriteSheetArmas[string.Concat (nomeIcones[i], "1")]);
			spriteArmas3.Add (spriteSheetArmas[string.Concat (nomeIcones[i], "2")]);
			Sprite cajado4 = (categorias[i].Equals ("Staff") ? spriteSheetArmas[string.Concat (nomeIcones[i], "3")] : null);
			spriteArmas4.Add (cajado4);
		}
	}

	// Atualiza informacoes do GameController
	public void AtualizarGameController ()
	{
		gameController.nomeArmas = this.nome;
		gameController.spriteArmas1 = this.spriteArmas1;
		gameController.spriteArmas2 = this.spriteArmas2;
		gameController.spriteArmas3 = this.spriteArmas3;
		gameController.spriteArmas4 = this.spriteArmas4;
		gameController.danosMinimo = this.danosMinimos;
		gameController.danosMaximo = this.danosMaximos;
		gameController.tipoDanosArma = this.tipoDanos;
		gameController.idClasseArma = this.idClasseArma;
		gameController.imagensInventario = this.iconesArmas;
	}
}