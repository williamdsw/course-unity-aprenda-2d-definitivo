using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class CarregarArmasRevisao : MonoBehaviour 
{
	// Variaveis de controle
	public string xmlFileName;

	// Objetos
	public GameControllerRevisao gameControllerRevisao;

	[Header ("Informações sobre as armas")]
	public List<string> names;
	public List<string> iconsNames;
	public List<Sprite> weaponIcons;
	public List<string> categories;
	public List<int> weaponClassIDs;
	public List<int> minDamages;
	public List<int> maxDamages;
	public List<int> damagesTypes;
	public List<Sprite> weaponSprites1;
	public List<Sprite> weaponSprites2;
	public List<Sprite> weaponSprites3;
	public List<Sprite> weaponSprites4;

	// Variaveis temporarias
	public List<Sprite> allSprites;
	public Sprite[] iconSprites;
	public Sprite[] swordSprites;
	public Sprite[] axeSprites;
	public Sprite[] bowSprites;
	public Sprite[] maceSprites;
	public Sprite[] hammerSprites;
	public Sprite[] staffSprites;
	private Dictionary<string, Sprite> weaponsSpritesheetDictionary;

	[Header ("SpriteSheets")]
	public Texture iconSpriteSheet;
	public Texture swordSpriteSheet;
	public Texture axeSpriteSheet;
	public Texture bowSpriteSheet;
	public Texture maceSpriteSheet;
	public Texture hammerSpriteSheet;
	public Texture staffSpriteSheet;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		gameControllerRevisao = FindObjectOfType (typeof (GameControllerRevisao)) as GameControllerRevisao;
		LoadSprites ();
		LoadXML ();
		LoadWeaponSprites ();
		RefreshGameController ();
	}

	// ------------------- FUNCOES ------------------- //

	// Responsavel por carregar Sprites
	private void LoadSprites ()
	{
		// Carrega os sprites
		string path = "Sprites/Weapons/";
		iconSprites = Resources.LoadAll<Sprite> (string.Concat (path, iconSpriteSheet.name));
		swordSprites = Resources.LoadAll<Sprite> (string.Concat (path, swordSpriteSheet.name));
		axeSprites = Resources.LoadAll<Sprite> (string.Concat (path, axeSpriteSheet.name));
		bowSprites = Resources.LoadAll<Sprite> (string.Concat (path, bowSpriteSheet.name));
		maceSprites = Resources.LoadAll<Sprite> (string.Concat (path, maceSpriteSheet.name));
		hammerSprites = Resources.LoadAll<Sprite> (string.Concat (path, hammerSpriteSheet.name));
		staffSprites = Resources.LoadAll<Sprite> (string.Concat (path, staffSpriteSheet.name));

		// Preenche lista com todos sprites
		AddToAllSprites (iconSprites);
		AddToAllSprites (swordSprites);
		AddToAllSprites (axeSprites);
		AddToAllSprites (bowSprites);
		AddToAllSprites (maceSprites);
		AddToAllSprites (hammerSprites);
		AddToAllSprites (staffSprites);

		// Adiciona sprites para Dictionary
		weaponsSpritesheetDictionary = allSprites.ToDictionary (sprite => sprite.name, sprite => sprite);
	}

	// Adiciona itens de um array de Sprites para uma lista
	private void AddToAllSprites (Sprite[] sprites)
	{
		foreach (Sprite sprite in sprites)
		{
			allSprites.Add (sprite);
		}
	}

	// Responsavel pela leitura do XML
	private void LoadXML ()
	{
		string path = string.Concat ("XML/", gameControllerRevisao.languageFolders[gameControllerRevisao.languageID], "/");
		path = string.Concat (path, xmlFileName);

		// Carrega o arquivo XML e carrega-o como documento XML
		TextAsset xmlData = (TextAsset) Resources.Load (path);
		XmlDocument document = new XmlDocument ();
		document.LoadXml (xmlData.text);

		// Itera tag root
		foreach (XmlNode weapons in document["Armas"].ChildNodes)
		{
			// Recupera atributo
			string attribute = weapons.Attributes["atributo"].Value;

			// Itera tags
			foreach (XmlNode weapon in weapons["armas"].ChildNodes)
			{
				switch (attribute)
				{
					case "nome":
					{
						names.Add (weapon.InnerText);
						break;
					}
					
					case "icone":
					{
						iconsNames.Add (weapon.InnerText);

						// Itera sprites e coloca icone na lista caso encontre pelo nome
						for (int i = 0; i < iconSprites.Length; i++)
						{
							if (iconSprites[i].name.Equals (weapon.InnerText))
							{
								weaponIcons.Add (iconSprites[i]);
								break;
							}
						}

						break;
					}

					case "categorias":
					{
						string category = weapon.InnerText;
						categories.Add (category);

						// Define tipo de ID da classe
						if (category.Equals ("Staff"))
						{
							weaponClassIDs.Add (2);
						}
						else if (category.Equals ("Arco"))
						{
							weaponClassIDs.Add (1);
						}
						else
						{
							weaponClassIDs.Add (0);
						}

						break;
					}

					case "dano-minimo":
					{
						minDamages.Add (int.Parse (weapon.InnerText));
						break;
					}

					case "dano-maximo":
					{
						maxDamages.Add (int.Parse (weapon.InnerText));
						break;
					}

					case "tipo-dano":
					{
						damagesTypes.Add (int.Parse (weapon.InnerText));
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
	private void LoadWeaponSprites ()
	{
		for (int i = 0; i < weaponIcons.Count; i++)
		{
			weaponSprites1.Add (weaponsSpritesheetDictionary[string.Concat (iconsNames[i], 0)]);
			weaponSprites2.Add (weaponsSpritesheetDictionary[string.Concat (iconsNames[i], 1)]);
			weaponSprites3.Add (weaponsSpritesheetDictionary[string.Concat (iconsNames[i], 2)]);
			Sprite cajado4 = (categories[i].Equals ("Staff") ? weaponsSpritesheetDictionary[string.Concat (iconsNames[i], 4)] : null);
			weaponSprites4.Add (cajado4);
		}
	}

	// Atualiza informacoes do GameController
	private void RefreshGameController ()
	{
		gameControllerRevisao.weaponsName = this.names;
		gameControllerRevisao.weaponsSprite1 = this.weaponSprites1;
		gameControllerRevisao.weaponsSprite2 = this.weaponSprites2;
		gameControllerRevisao.weaponsSprite3 = this.weaponSprites3;
		gameControllerRevisao.weaponsSprite4 = this.weaponSprites4;
		gameControllerRevisao.minDamages = this.minDamages;
		gameControllerRevisao.maxDamages = this.maxDamages;
		gameControllerRevisao.damageTypesInt = this.damagesTypes;
		gameControllerRevisao.weaponClassID = this.weaponClassIDs;
		gameControllerRevisao.inventaryImages = this.weaponIcons;
	}
}