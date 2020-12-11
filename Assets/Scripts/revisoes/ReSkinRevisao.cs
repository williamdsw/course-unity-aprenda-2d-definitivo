using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReSkinRevisao : MonoBehaviour 
{
	// Variaveis de controle
    public bool isPlayer;

    // Components / Outros objetos
    private GameControllerRevisao gameControllerRevisao;

    [Header ("Propriedades sobre os Sprites")]
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public string spriteSheetName;
    public string actualSpriteSheetName;
    private Dictionary<string, Sprite> spriteSheet;

    // ------------------- FUNCOES UNITY ------------------- //

    private void Start () 
    {
        // Inicializa
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        gameControllerRevisao = FindObjectOfType (typeof (GameControllerRevisao)) as GameControllerRevisao;

        // Atualiza spritesheet do player
        if (isPlayer)
        {
            spriteSheetName = gameControllerRevisao.spriteSheetTexture[gameControllerRevisao.playerID].name;
        }

        LoadSpriteSheet ();
    }

    // Assim que o Update e processado, esse aqui sera tambem
    private void LateUpdate () 
    {
        // Atualiza sprite do player
        if (isPlayer)
        {
            if (gameControllerRevisao.playerID != gameControllerRevisao.actualPlayerID)
            {
                spriteSheetName = gameControllerRevisao.spriteSheetTexture[gameControllerRevisao.playerID].name;
                gameControllerRevisao.actualPlayerID = gameControllerRevisao.playerID;
            }

            gameControllerRevisao.ValidateWeapon ();
        }

        // Carrega novo spritesheet 
        if (!actualSpriteSheetName.Equals (spriteSheetName))
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
        actualSpriteSheetName = spriteSheetName;
    }
}