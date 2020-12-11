using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
	DIALOGO,
	FIM_DIALOGO,
	GAMEPLAY,
	ITENS,
	LOADGAME,
	PAUSE
}

public class GameController : MonoBehaviour
{
	// Variaveis de Objects
	public GameState estadoAtual;
	private Inventario inventario;
	private HUD hud;
	private AudioController audioController;

	[Header ("Informacoes do Dano")]
	public string[] tiposDano;
	public GameObject[] fxDanos;
	public GameObject fxMorte;

	[Header ("Informacoes do Ouro")]
	public int gold;							//armazena a quantidade que coletamos
	public TextMeshProUGUI goldText;

	[Header ("Informações do Player")]
	private Player player;
	public int playerID;
	public int playerAtualID;
	public int vidaMaxima;
	public int vidaAtual;
	public int manaMaxima;
	public int manaAtual;
	public int idArma;
	public int idArmaAtual;
	public int idFlechaEquipada;
	public int[] quantidadeFlechas;			// 0 - Flecha normal, 1 - Flecha de prata, 2 - Flecha de ouro
	public int[] quantidadePocoes;			// 0 - Cura, 1 - Mana

	[Header ("Banco de Personagens")]
	public string[] nomePersonagem;
	public Texture[] nomeSpritesheet;
	public int[] idClasse;
	public int idArmaInicial;
	public GameObject[] armaInicial;
	public ItemModelo[] armaInicialPersonagem;

	[Header ("Banco de Dados das Armas")]
	public List<string> nomeArmas;
	public List<int> custoArmas;
	public List<int> idClasseArma;				//0: Machado, Martelo, Espadas - 1: Espadas - 2: Staffs
	public List<Sprite> imagensInventario;
	public List<Sprite> spriteArmas1;
	public List<Sprite> spriteArmas2;
	public List<Sprite> spriteArmas3;
	public List<Sprite> spriteArmas4;
	public List<int> danosMinimo;
	public List<int> danosMaximo;
	public List<int> tipoDanosArma;
	public List<int> aprimoramentoArmas;

	[Header ("Flechas")]
	public Sprite[] iconesFlecha;
	public Sprite[] imagensFlecha;
	public GameObject[] flechaPrefabs;
	public float[] velocidadesFlecha;

	[Header ("Paineis")]
	public GameObject painelPause;
	public GameObject painelItens;
	public GameObject painelItemInfo;
	private bool estaPausado;

	[Header ("Primeiro Elemento de Cada Painel")]
	public Button primeiroPainelPause;
	public Button primeiroPainelItens;
	public Button primeiroPainelItemInfo;

	[Header ("Materiais para iluminação")]
	public Material luz2D;
	public Material padrao2D;

	[Header ("Configuração de Idiomas")]
	public string[] idiomaFolders;
	public int idIdioma;

	// Controle de missao
	public bool missao1Aceita;					// Indica que aceitei a missao
	public bool missao1Finalizada;				// Indica que a missao foi concluida

	public List<string> itensInventario;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Awake ()
	{
		// Resolve valor do ID errado na HUD
		playerID = PlayerPrefs.GetInt ("id_personagem");
	}

	// Inicializa
	private void Start () 
	{
		// Mantem objeto na proxima cena
		DontDestroyOnLoad (this.gameObject);

		// Inicializa valores e objetos
		player = FindObjectOfType<Player>();
		inventario = FindObjectOfType (typeof (Inventario)) as Inventario;
		hud = FindObjectOfType (typeof (HUD)) as HUD;
		audioController = FindObjectOfType<AudioController>();

		// Encontra elementos
		painelPause = GameObject.Find ("Pause");
		painelItens = GameObject.Find ("Itens");
		painelItemInfo = GameObject.Find ("ItemInfo");
		primeiroPainelPause = GameObject.Find ("Button-Fechar-Pause").GetComponent<Button>();
		primeiroPainelItens = GameObject.Find ("Button-Fechar-Itens").GetComponent<Button>();
		primeiroPainelItemInfo = GameObject.Find ("Button-Fechar-ItemInfo").GetComponent<Button>();

		DefinirEventos ();

		// Desativa paineis
		if (painelPause != null)
		{
			painelPause.SetActive (false);
		}

		if (painelItens != null)
		{
			painelItens.SetActive (false);
		}

		if (painelItemInfo != null)
		{
			painelItemInfo.SetActive (false);
		}

		Carregar (PlayerPrefs.GetString ("slot"));
	}

	private void Update () 
	{
		if (estadoAtual == GameState.GAMEPLAY)
		{
			if (player == null)
			{
				player = FindObjectOfType<Player>();
			}

			// Atualiza texto do score
			string goldString = gold.ToString ("N0");
			goldString = goldString.Replace (",", ".");
			goldText.text = goldString;

			//ValidarArma ();

			// Interacao dos menus 
			if (Input.GetButtonDown ("Cancel"))
			{
				// Pausa
				if (estadoAtual != GameState.ITENS)
				{
					Pausar ();	
				}

				// Desativa painel de itens
				if (estadoAtual == GameState.ITENS)
				{
					if (painelItemInfo.activeSelf)
					{
						painelItemInfo.SetActive (false);
						primeiroPainelItens.Select ();
					}
					else if (painelItens.activeSelf)
					{
						FecharPainel (painelItens, GameState.PAUSE);
						primeiroPainelPause.Select ();
						inventario.LimparItensCarregados ();
					}
				}
			}
		}
	}

	// ------------------- FUNCOES ------------------- //

	// Valida uma a arma do personagem caso ele trocar
	public void ValidarArma ()
	{
		if (idClasseArma[idArma] != idClasse[playerID])
		{
			idArma = idArmaInicial;
			player.TrocarArma (idArma);
		}
	}

	// Pausa ou despausa o jogo
	public void Pausar ()
	{
		bool pauseState = painelPause.activeSelf;
		pauseState = !pauseState;
		painelPause.SetActive (pauseState);

		if (pauseState)
		{
			// Pausa
			Time.timeScale = 0;
			MudarEstado (GameState.PAUSE);

			// Controla volume
			float volumeTemporario = audioController.volumeMaximoMusica;
			volumeTemporario = (volumeTemporario / 20);
			audioController.sourceMusica.volume = volumeTemporario;
			
			primeiroPainelPause.Select ();
		}
		else 
		{
			// Despausa
			Time.timeScale = 1;
			MudarEstado (GameState.GAMEPLAY);
			audioController.sourceMusica.volume = audioController.volumeMaximoMusica;
		}
	}

	// Define novo estado do jogo
	public void MudarEstado (GameState novoEstado)
	{
		this.estadoAtual = novoEstado;

		switch (novoEstado)
		{
			case GameState.GAMEPLAY:
			{
				Time.timeScale = 1;
				break;
			}

			case GameState.PAUSE: case GameState.ITENS:
			{
				Time.timeScale = 0;
				break;
			}

			case GameState.FIM_DIALOGO:
			{
				StartCoroutine ("FimDaConversa");
				break;
			}

			default:
			{
				break;
			}
		}
	}

	// Define eventos dos botoes
	public void DefinirEventos ()
	{
		// Encontra objetos
		Button btnFecharPause = GameObject.Find ("Button-Fechar-Pause").GetComponent<Button>();
		Button btnItens = GameObject.Find ("Button-Itens").GetComponent<Button>();
		Button btnFecharItens = GameObject.Find ("Button-Fechar-Itens").GetComponent<Button>();
		Button btnFecharItemInfo = GameObject.Find ("Button-Fechar-ItemInfo").GetComponent<Button>();
		Button btnSair = GameObject.Find ("Button-Quit").GetComponent<Button>();

		// Despausa
		if (btnFecharPause != null)
		{
			btnFecharPause.onClick.AddListener (delegate
			{
				TocarClick ();
				Pausar ();
			});
		}

		// Abre menu de itens
		if (btnItens != null)
		{
			btnItens.onClick.AddListener (delegate 
			{
				TocarClick ();
				AbrirPainel (painelItens, GameState.ITENS);
				primeiroPainelItens.Select ();
				inventario.CarregarInventario ();
			});
		}

		// Fecha menu de itens
		if (btnFecharItens != null)
		{
			btnFecharItens.onClick.AddListener (delegate
			{
				TocarClick ();
				FecharPainel (painelItens, GameState.PAUSE);
				primeiroPainelPause.Select ();
				inventario.LimparItensCarregados ();
			});
		}

		// Fecha menu de informacao do item
		if (btnFecharItemInfo != null)
		{
			btnFecharItemInfo.onClick.AddListener (delegate
			{
				TocarClick ();
				painelItemInfo.SetActive (false);
			});
		}

		// Volta para menu principal
		if (btnSair != null)
		{
			btnSair.onClick.AddListener (delegate
			{
				Destroy (this.gameObject);
				Destroy (audioController.gameObject);
				SceneManager.LoadScene ("PreTitulo");
			});
		}
	}

	// Fecha painel atual e abre o proximo
	public void AbrirPainel (GameObject proximoPainel, GameState proximoEstado)
	{
		painelPause.SetActive (false);
		proximoPainel.SetActive (true);
		MudarEstado (proximoEstado);
	}

	// Fecha painel informado e aparece painel de Pause
	public void FecharPainel (GameObject painel, GameState proximoEstado)
	{
		painel.SetActive (false);
		painelPause.SetActive (true);	
		MudarEstado (proximoEstado);
	}

	// Abre painel de informacao do item
	public void AbrirItemInfo ()
	{
		painelItemInfo.SetActive (true);
		primeiroPainelItemInfo.Select ();
	}

	// Troca arma do usuario
	public void UsarItemArma (int id)
	{
		player.TrocarArma (id);
	}

	// Fecha paineis e volta gameplay
	public void VoltarGameplay ()
	{
		painelItemInfo.SetActive (false);
		painelItens.SetActive (false);
		painelPause.SetActive (false);
		MudarEstado (GameState.GAMEPLAY);
	}

	// Exclui item do inventario, recarrega o mesmo, e fecha o painel
	public void ExcluirItem (int idSlot)
	{
		inventario.itensInventario.RemoveAt (idSlot);
		inventario.CarregarInventario ();
		painelItemInfo.SetActive (false);
		primeiroPainelItens.Select ();
	}

	// Aplica aprimoramento da arma
	public void AprimorarArma (int idArma)
	{
		int aprimoramento = aprimoramentoArmas[idArma];
		if (aprimoramento < 10)
		{
			aprimoramento++;
			aprimoramentoArmas[idArma] = aprimoramento;
		}
	}

	// Troca itens do inventario de posicao
	public void TrocarItens (int idSlot)
	{
		GameObject primeiroItem = inventario.itensInventario[0];
		GameObject itemAtual = inventario.itensInventario[idSlot];
		inventario.itensInventario[0] = itemAtual;
		inventario.itensInventario[idSlot] = primeiroItem;
		VoltarGameplay ();
	}

	// Adiciona um objeto ao invenatario
	public void ColetarItem (GameObject objetoColetado)
	{
		inventario.itensInventario.Add (objetoColetado);
	}

	// Utiliza uma pocao de cura ou de mana	
	public void UsarPocao (int idPocao)
	{
		if (quantidadePocoes[idPocao] > 0)
		{
			quantidadePocoes[idPocao]--;

			switch (idPocao)
			{
				// Pocao de cura
				case 0:
				{
					vidaAtual += 3;

					if (vidaAtual > vidaMaxima)
					{
						vidaAtual = vidaMaxima;
					}

					break;
				}

				// Pocao de mana
				case 1:
				{
					manaAtual += 3;

					if (manaAtual > manaMaxima)
					{
						manaAtual = manaMaxima;
					}

					break;
				}

				default:
				{
					break;
				}
			}
		}
	}

	// Formata uma string com indicadores para tags
	public string TextoFormatado (string frase)
	{
		string temp = frase;

		// Subtitui palavras especificas
		temp = temp.Replace ("cor=yellow", "<color=#FFFF00FF>");
		temp = temp.Replace ("cor=red", "<color=#ff0000ff>");
		temp = temp.Replace ("cor=orange", "<color=#ffa500ff>");
		temp = temp.Replace ("fimnegrito", "</b>");
		temp = temp.Replace ("negrito", "<b>");
		temp = temp.Replace ("fimcor", "</color>");

		return temp;
	}

	// Salva dados do jogador num slot armazenado
	public void Salvar ()
	{
		string caminho = string.Concat (Application.persistentDataPath, "/", PlayerPrefs.GetString ("slot"));

		// Cria arquivo e formatter 
		BinaryFormatter binaryFormatter = new BinaryFormatter ();
		FileStream fileStream = File.Create (caminho);

		// Envia dados
		DadosJogador dados = new DadosJogador ();
		dados.idIdioma = this.idIdioma;
	  	dados.numeroOuro = this.gold;
	  	dados.idPersonagem = this.playerID;
	  	dados.idArma = this.idArma;
	  	dados.idFlechaEquipada = this.idFlechaEquipada;
	  	dados.quantidadeFlechas = this.quantidadeFlechas;
	  	dados.quantidadePocoes = this.quantidadePocoes;
		dados.itensInventario = this.itensInventario;
	 	dados.aprimoramentosArmas = this.aprimoramentoArmas;

		// Verifica o inventario
		itensInventario.Clear ();

		foreach (GameObject item in inventario.itensInventario)
		{
			itensInventario.Add (item.name);
		}

		// Serializa dados para arquivo
		binaryFormatter.Serialize (fileStream, dados);
		fileStream.Close ();
	}

	// Carrega dados salvo num slot
	public void Carregar (string slot)
	{
		string caminho = string.Concat (Application.persistentDataPath, "/", slot);

		// Verifica se arquivo existe
		if (File.Exists (caminho))
		{
			// Abre arquivo e formatter 
			BinaryFormatter binaryFormatter = new BinaryFormatter ();
			FileStream fileStream = File.Open (caminho,  FileMode.Open);

			// Carrega dados
			DadosJogador dados = (DadosJogador) binaryFormatter.Deserialize (fileStream);
			this.idIdioma = dados.idIdioma;
			this.gold = dados.numeroOuro;
			this.playerID = dados.idPersonagem;
			this.idArma = dados.idArma;
			this.idArmaAtual = dados.idArma;
			this.idArmaInicial = dados.idArma;
			this.idFlechaEquipada = dados.idFlechaEquipada;
			this.quantidadeFlechas = dados.quantidadeFlechas;
			this.quantidadePocoes = dados.quantidadePocoes;
			this.itensInventario = dados.itensInventario;
			this.aprimoramentoArmas = dados.aprimoramentosArmas;

			// Verifica o inventario
			inventario.itensCarregados.Clear ();

			// Recupera dados 
			foreach (string item in itensInventario)
			{
				GameObject objeto = Resources.Load<GameObject>(string.Concat ("Prefabs/Armas/", item));
				inventario.itensCarregados.Add (objeto);
			}

			inventario.itensInventario.Add (armaInicial[playerID]);
			GameObject tempArma = Instantiate (armaInicial[playerID]);
			inventario.itensCarregados.Add (tempArma);

			vidaAtual = vidaMaxima;
			manaAtual = manaMaxima;

			// Fecha a carrega cena
			fileStream.Close ();
			MudarEstado (GameState.GAMEPLAY);
			string nomeCena = "PrimeiraFase 1";
			audioController.TrocarMusica (audioController.musicaFase1, nomeCena, true);
		}
		else 
		{
			NovoJogo ();
		}
	}

	// Define dados para um novo jogo
	public void NovoJogo ()
	{
		// Definir os valores iniciais do jogo
		playerID = PlayerPrefs.GetInt ("id_personagem");
		gold = 0;
		idArma = armaInicialPersonagem[playerID].idArma;
	
		idFlechaEquipada = 0;
		quantidadeFlechas[0] = 25;
		quantidadeFlechas[1] = 0;
		quantidadeFlechas[2] = 0;
		quantidadePocoes[0] = 3;
		quantidadePocoes[1] = 3;

		// Chama funcoes
		Salvar ();
		MudarEstado (GameState.GAMEPLAY);
		hud.VerificarHUDPersonagem ();
		Carregar (PlayerPrefs.GetString ("slot"));
	}

	// Toca o efeito de clique dos botoes
	public void TocarClick ()
	{
		audioController.TocarEfeito (audioController.efeitoClick, 1);
	}

	// ------------------- CORROTINA ------------------- //

	private IEnumerator FimDaConversa ()
	{
		yield return new WaitForEndOfFrame ();
		MudarEstado (GameState.GAMEPLAY);
	}
}