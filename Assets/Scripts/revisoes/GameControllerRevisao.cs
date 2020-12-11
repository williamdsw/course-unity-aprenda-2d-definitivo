using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum GameStateRevisao
{
	BEGIN_DIALOGUE,
	END_DIALOGUE,
	GAMEPLAY,
	ITEMS,
	LOADGAME,
	PAUSE
}

public class GameControllerRevisao : MonoBehaviour 
{
	// Variaveis de Objects
	public GameStateRevisao actualGameState;
	private InventarioRevisao inventarioRevisao;
	private HUDRevisao hudRevisao;
	private AudioControllerRevisao audioControllerRevisao;

	[Header ("Informacoes do Dano")]
	private string[] damageTypes;
	public GameObject[] damageEffect;
	public GameObject deathEffect;

	[Header ("Informacoes do Ouro")]
	private int numberGold;
	public TextMeshProUGUI numberGoldText;

	[Header ("Informacoes do Player")]
	private PlayerRevisao playerRevisao;
	public int playerID;
	public int actualPlayerID;
	public int playerMaxLife;
	public int playerActualLife;
	public int playerMaxMana;
	public int playerActualMana;
	public int weaponID;
	public int actualWeaponID;
	public int equippedArrowID;
	public int[] arrowsQuantity;
	public int[] potionsQuantity;

	[Header ("Banco de Dados de Personagens")]
	public string[] playerName;
	public Texture[] spriteSheetTexture;
	public int[] classID;
	public int initialWeaponID;
	public GameObject[] initialWeapons;
	public ItemModeloRevisao[] playerInitialWeapons;

	[Header ("Banco de Dados de Arma")]
	public List<string> weaponsName;
	public List<int> weaponsCost;
	public List<int> weaponClassID;						//0: Machado, Martelo, Espadas - 1: Espadas - 2: Staffs
	public List<Sprite> inventaryImages;
	public List<Sprite> weaponsSprite1;
	public List<Sprite> weaponsSprite2;
	public List<Sprite> weaponsSprite3;
	public List<Sprite> weaponsSprite4;
	public List<int> minDamages;
	public List<int> maxDamages;
	public List<int> damageTypesInt;
	public List<int> weaponImprovements;

	[Header ("Flechas")]
	public Sprite[] arrowIcons;
	public Sprite[] arrowImages;
	public GameObject[] arrowPrefabs;
	public float[] arrowVelocities;

	[Header ("Paineis")]
	public GameObject pausePanel;
	public GameObject itemsPanel;
	public GameObject itemInfoPanel;
	public bool isPaused;

	[Header ("Primeiro Elemento de Cada Painel")]
	public Button btnFirstPause;
	public Button btnFirstItems;
	public Button btnFirstItemInfo;

	[Header ("Materiais para iluminação")]
	public Material light2D;
	public Material default2D;

	[Header ("Configuração de Idiomas")]
	public List<string> languageFolders;
	public int languageID;
	
	// Controle de missoes
	public bool mission1Accepted;
	public bool mission1Finished;

	public List<string> inventaryItems;

	// Properties
	
	public int NumberGold
	{
		get { return this.numberGold; }
		set { numberGold = value; }
	}
	
	public string[] DamageTypes
	{
		get { return this.damageTypes; }
		set { damageTypes = value; }
	}

	// ------------------- FUNCOES UNITY ------------------- //

	private void Awake () 
	{
		// Resolve valor do ID errado na HUD
		playerID = PlayerPrefs.GetInt ("id_personagem", 0);
	}

	// Inicializa
	private void Start () 
	{
		// Mantem objeto na proxima cena
		DontDestroyOnLoad (this.gameObject);

		// Inicializa valores e objetos
		playerRevisao = FindObjectOfType<PlayerRevisao>();
		inventarioRevisao = FindObjectOfType (typeof (InventarioRevisao)) as InventarioRevisao;
		hudRevisao = FindObjectOfType<HUDRevisao>();
		audioControllerRevisao = FindObjectOfType (typeof (AudioControllerRevisao)) as AudioControllerRevisao;

		// Encontra elementos
		pausePanel = GameObject.Find ("Pause-Panel");
		itemsPanel = GameObject.Find ("Items-Panel");
		itemInfoPanel = GameObject.Find ("Item-Info-Panel");
		btnFirstPause = GameObject.Find ("Button-Close-Pause").GetComponent<Button>();
		btnFirstItems = GameObject.Find ("Button-Close-Items").GetComponent<Button>();
		btnFirstItemInfo = GameObject.Find ("Button-Close-Item-Info").GetComponent<Button>();

		BindEvents ();

		// Desativa paineis

		if (pausePanel != null)
		{
			pausePanel.SetActive (false);
		}

		if (itemsPanel != null)
		{
			itemsPanel.SetActive (false);
		}

		if (itemInfoPanel != null)
		{
			itemInfoPanel.SetActive (false);
		}

		LoadGame (PlayerPrefs.GetString ("slot"));
	}

	private void Update ()
	{
		if (actualGameState == GameStateRevisao.GAMEPLAY)
		{
			if (playerRevisao == null)
			{
				playerRevisao = FindObjectOfType<PlayerRevisao>();
			}

			// Atualiza texto do score
			string numberGoldString = numberGold.ToString ("N0");
			numberGoldString = numberGoldString.Replace (",", ".");
			numberGoldText.text = numberGoldString;

			//ValidateWeapon ();

			// Interacao dos menus 
			if (Input.GetButtonDown ("Cancel"))
			{
				// Pausa
				if (actualGameState != GameStateRevisao.PAUSE)
				{
					PauseGame ();
				}

				//Desativa paineis de itens
				if (actualGameState == GameStateRevisao.ITEMS)
				{
					if (itemInfoPanel.activeSelf)
					{
						itemInfoPanel.SetActive (false);
						btnFirstItems.Select ();
					}
					else if (itemsPanel.activeSelf)
					{
						OpenPanel (itemsPanel, pausePanel, btnFirstPause, GameStateRevisao.PAUSE);
						inventarioRevisao.ClearLoadedItems ();
					}
				}
			}
		}
	}

	// ------------------- FUNCOES ------------------- //

	// Valida uma a arma do personagem caso ele trocar
	public void ValidateWeapon ()
	{
		if (weaponClassID[weaponID] != classID[playerID])
		{
			weaponID = initialWeaponID;
			playerRevisao.ChangeWeapon (weaponID);
		}
	}

	// Pausa ou despausa o jogo
	public void PauseGame ()
	{
		bool pauseState = pausePanel.activeSelf;
		pauseState = !pauseState;
		pausePanel.SetActive (pauseState);

		if (pauseState)
		{
			// Pausa
			Time.timeScale = 0;
			ChangeGameState (GameStateRevisao.PAUSE);

			// Controla volume
			float tempVolume = audioControllerRevisao.maxVolumeMusic;
			tempVolume = (tempVolume / 20);
			audioControllerRevisao.sourceMusic.volume = tempVolume;

			btnFirstPause.Select ();
		}
		else 
		{
			// Despausa
			Time.timeScale = 1;
			ChangeGameState (GameStateRevisao.GAMEPLAY);
			audioControllerRevisao.sourceMusic.volume = audioControllerRevisao.maxVolumeMusic;
		}
	}

	// Altera o estado atual do Jogo
	public void ChangeGameState (GameStateRevisao nextState)
	{
		this.actualGameState = nextState;

		switch (nextState)
		{
			// Gameplay
			case GameStateRevisao.GAMEPLAY:
			{
				Time.timeScale = 1;
				break;
			}

			// Tela de pause ou itens
			case GameStateRevisao.ITEMS: case GameStateRevisao.PAUSE:
			{
				Time.timeScale = 0;
				break;
			}

			// Termino do dialogo com NPC
			case GameStateRevisao.END_DIALOGUE:
			{
				StartCoroutine ("EndDialogue");
				break;
			}

			default:
			{
				break;
			}
		}
	}

	// Define eventos 'onclick' dos botoes
	public void BindEvents ()
	{
		// Encontra objetos
		Button btnPauseClose = GameObject.Find ("Button-Close-Pause").GetComponent<Button>();
		Button btnOpenItems = GameObject.Find ("Button-Open-Items").GetComponent<Button>();
		Button btnCloseItems = GameObject.Find ("Button-Close-Items").GetComponent<Button>();
		Button btnCloseItemInfo = GameObject.Find ("Button-Close-Item-Info").GetComponent<Button>();

		// Despausa
		if (btnPauseClose != null)
		{
			btnPauseClose.onClick.AddListener (delegate
			{
				PauseGame ();
				PlayClick ();
			}); 
		}

		// Abre menu de itens
		if (btnOpenItems != null)
		{
			btnOpenItems.onClick.AddListener (delegate
			{
				OpenPanel (pausePanel, itemsPanel, btnFirstItems, GameStateRevisao.ITEMS);
				PlayClick ();
				inventarioRevisao.LoadInventory ();
			});
		}

		// Fecha menu de itens
		if (btnCloseItems != null)
		{
			btnCloseItems.onClick.AddListener (delegate
			{
				OpenPanel (itemsPanel, pausePanel, btnFirstPause, GameStateRevisao.PAUSE);
				PlayClick ();
				inventarioRevisao.ClearLoadedItems ();
			});
		}

		// Fecha menu de informacao do item
		if (btnCloseItemInfo != null)
		{
			btnCloseItemInfo.onClick.AddListener (delegate
			{
				OpenPanel (itemInfoPanel, null, btnFirstItems, GameStateRevisao.ITEMS);
				PlayClick ();
			});
		}
	}

	// Fecha painel atual, abre o proximo painel, seleciona objeto para navegacao e muda status
	public void OpenPanel (GameObject actualPanel, GameObject nextPanel, Selectable firstSelected, GameStateRevisao nextState)
	{
		if (actualPanel != null)
		{
			actualPanel.SetActive (false);
		}

		if (nextPanel != null)
		{
			nextPanel.SetActive (true);
		}

		if (firstSelected != null)
		{
			firstSelected.Select ();
		}

		ChangeGameState (nextState);
	}

	// Troca arma do usuario
	public void UseWeaponItem (int id)
	{
		playerRevisao.ChangeWeapon (id);
	}

	// Fecha paineis e volta gameplay
	public void ReturnGameplay ()
	{
		pausePanel.SetActive (false);
		itemsPanel.SetActive (false);
		itemInfoPanel.SetActive (false);
		ChangeGameState (GameStateRevisao.GAMEPLAY);
	}

	// Exclui item do inventario, recarrega o mesmo, e fecha o painel
	public void DeleteItem (int slotID)
	{
		inventarioRevisao.inventaryItems.RemoveAt (slotID);
		inventarioRevisao.LoadInventory ();
		itemInfoPanel.SetActive (false);
		btnFirstItems.Select ();
	}

	// Aplica aprimoramento da arma
	public void ImproveWeapon (int weaponID)
	{
		int improvement = weaponImprovements[weaponID];
		if (improvement < 10)
		{
			improvement++;
			weaponImprovements[weaponID] = improvement;
		}
	}

	// Troca itens do inventario de posicao
	public void ChangeItems (int slotID)
	{
		GameObject firstItem = inventarioRevisao.inventaryItems[0];
		GameObject actualItem = inventarioRevisao.inventaryItems[slotID];
		inventarioRevisao.inventaryItems[0] = actualItem;
		inventarioRevisao.inventaryItems[slotID] = firstItem;
		ReturnGameplay ();
	}

	// Adiciona um objeto ao invenatario
	public void CollectItem (GameObject collectedObject)
	{
		inventarioRevisao.inventaryItems.Add (collectedObject);
	}

	// Utiliza uma pocao de cura ou de mana	
	public void UsePotion (int potionID)
	{
		if (potionsQuantity[potionID] > 0)
		{
			potionsQuantity[potionID]--;

			switch (potionID)
			{
				// pocao de cura
				case 0:
				{
					playerActualLife += 3;

					if (playerActualLife > playerMaxLife)
					{
						playerActualLife = playerMaxLife;
					}

					break;
				}

				// pocao de mana
				case 1:
				{
					playerActualMana += 1;

					if (playerActualMana > playerMaxMana)
					{
						playerActualMana = playerMaxMana;
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
	public string FormatText (string text)
	{
		string temp = text;

		// Subtitui palavras especificas
		temp = temp.Replace ("color=yellow", "<color=#FFFF00FF>");
		temp = temp.Replace ("color=red", "<color=#ff0000ff>");
		temp = temp.Replace ("color=orange", "<color=#ffa500ff>");
		temp = temp.Replace ("endbold", "</b>");
		temp = temp.Replace ("bold", "<b>");
		temp = temp.Replace ("endcolor", "</color>");

		return temp;
	}

	// Salva dados do jogador num slot armazenado
	public void SaveGame ()
	{
		string path = string.Concat (Application.persistentDataPath, "/", PlayerPrefs.GetString ("slot"));

		// Abre arquivo e formatter 
		using (FileStream fileStream = File.Create (path))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter ();

			// Envia dados
			DadosJogadorRevisao dadosJogadorRevisao = new DadosJogadorRevisao ();
			dadosJogadorRevisao.LanguageID = this.languageID;
			dadosJogadorRevisao.NumberGold = this.numberGold;
			dadosJogadorRevisao.PlayerID = this.playerID;
			dadosJogadorRevisao.WeaponID = this.weaponID;
			dadosJogadorRevisao.EquippedArrowID = this.equippedArrowID;
			dadosJogadorRevisao.ArrowQuantities = this.arrowsQuantity;
			dadosJogadorRevisao.PotionQuantities = this.potionsQuantity;
			dadosJogadorRevisao.InventaryItems = this.inventaryItems;
			dadosJogadorRevisao.WeaponImprovements = this.weaponImprovements;

			// Verifica o inventario
			this.inventaryItems.Clear ();

			foreach (GameObject item in inventarioRevisao.inventaryItems)
			{
				this.inventaryItems.Add (item.name);
			}

			// Serializa dados para arquivo
			binaryFormatter.Serialize (fileStream, dadosJogadorRevisao);
		}
	}

	// Carrega dados salvo num slot
	public void LoadGame (string slot)
	{
		string path = string.Concat (Application.persistentDataPath, "/", slot);

		// Verifica se arquivo existe
		if (File.Exists (path))
		{
			// Abre arquivo e formatter 
			using (FileStream fileStream = File.Open (path, FileMode.Open))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter ();

				// Carrega dados
				DadosJogadorRevisao dadosJogadorRevisao = binaryFormatter.Deserialize (fileStream) as DadosJogadorRevisao;
				this.languageID = dadosJogadorRevisao.LanguageID;
				this.numberGold = dadosJogadorRevisao.NumberGold;
				this.playerID = dadosJogadorRevisao.PlayerID;
				this.weaponID = dadosJogadorRevisao.WeaponID;
				this.actualWeaponID = dadosJogadorRevisao.WeaponID;
				this.initialWeaponID = dadosJogadorRevisao.WeaponID;
				this.arrowsQuantity = dadosJogadorRevisao.ArrowQuantities;
				this.potionsQuantity = dadosJogadorRevisao.PotionQuantities;
				this.inventaryItems = dadosJogadorRevisao.InventaryItems;
				this.weaponImprovements = dadosJogadorRevisao.WeaponImprovements;

				// Verifica o inventario
				inventarioRevisao.inventaryItems.Clear ();

				// Recupera dados 
				foreach (string item in this.inventaryItems)
				{
					GameObject obj = Resources.Load<GameObject> (string.Concat ("Prefabs/Weapons/", item));
					inventarioRevisao.inventaryItems.Add (obj);
				}

				inventarioRevisao.inventaryItems.Add (initialWeapons[weaponID]);
				GameObject temp = Instantiate (initialWeapons[weaponID]);
				inventarioRevisao.loadedItems.Add (temp);

				playerActualLife = playerMaxLife;
				playerActualMana = playerMaxMana;

				// Toca musica e carrega a cena
				string nextScene = "PrimeiraFase";
				audioControllerRevisao.ChangeMusic (audioControllerRevisao.musicStage1, nextScene, true);
			}
		}
		else 
		{
			NewGame ();
		}
	}

	// Define dados para um novo jogo
	public void NewGame ()
	{
		// Definir os valores iniciais do jogo
		playerID = PlayerPrefs.GetInt ("player_id");
		numberGold = 0;
		weaponID = playerInitialWeapons[weaponID].weaponID;

		equippedArrowID = 0;
		arrowsQuantity[0] = 25;
		arrowsQuantity[1] = 0;
		arrowsQuantity[2] = 0;
		potionsQuantity[0] = 3;
		potionsQuantity[1] = 3;

		// Chama funcoes
		SaveGame ();
		ChangeGameState (GameStateRevisao.GAMEPLAY);
		hudRevisao.VerifyPlayerHUD ();
		LoadGame (PlayerPrefs.GetString ("slot"));
	}

	// Toca o efeito de clique dos botoes
	private void PlayClick ()
	{
		audioControllerRevisao.PlayFX (audioControllerRevisao.fxClick, 1f);
	}

	// ------------------- CORROTINAS ------------------- //

	private IEnumerator EndDialogue ()
	{
		yield return new WaitForEndOfFrame ();
		ChangeGameState (GameStateRevisao.GAMEPLAY);
	}
}