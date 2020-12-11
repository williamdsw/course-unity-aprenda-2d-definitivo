using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReSkin : MonoBehaviour 
{
	// Variaveis de controle
    public bool isPlayer;

    // Components / Outros objetos
    private GameController gameController;

    [Header ("Propriedades sobre os Sprites")]
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public string spriteSheetName;								// Nome do spritesheet que queremos utilizar
    public string loadedSpriteSheetName;						// Nome do spritesheet em uso (atual)
    private Dictionary<string, Sprite> spriteSheet;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Start () 
	{
		// Inicializa
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		gameController = FindObjectOfType (typeof (GameController)) as GameController;

		// Atualiza spritesheet do player
		if (isPlayer)
		{
			spriteSheetName = gameController.nomeSpritesheet[gameController.playerID].name;	
		}

		LoadSpriteSheet ();
	}

	// Assim que o Update e processado, esse aqui sera tambem
	private void LateUpdate () 
	{
		// Atualiza sprite do player
		if (isPlayer)
		{
			if (gameController.playerID != gameController.playerAtualID)
			{
				spriteSheetName = gameController.nomeSpritesheet[gameController.playerID].name;	
				gameController.playerAtualID = gameController.playerID;
			}

			gameController.ValidarArma ();
		}

		// Carrega novo spritesheet 
		if (!loadedSpriteSheetName.Equals (spriteSheetName))
		{
			LoadSpriteSheet ();
		}

		spriteRenderer.sprite = spriteSheet[spriteRenderer.sprite.name];
	}

	// ------------------- FUNCOES ------------------- //
	
	private void LoadSpriteSheet ()
	{
		// Carrega todos os sprites de acordo com nome do spritesheet
		sprites = Resources.LoadAll<Sprite> (spriteSheetName);
		spriteSheet = sprites.ToDictionary (sprite => sprite.name, sprite => sprite);
		loadedSpriteSheetName = spriteSheetName;
	}
}