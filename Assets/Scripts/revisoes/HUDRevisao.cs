using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDRevisao : MonoBehaviour 
{
	// Objetos
	private GameControllerRevisao gameControllerRevisao;
	private PlayerRevisao playerRevisao;

	[Header ("Imagens da barra de vida")]
	public List<Image> lifebarImages;
	public Sprite halfLifeSprite;
	public Sprite fullLifeSprite;

	[Header ("Imagens da barra de mana")]
	public List<Image> manabarImages;
	public Sprite halfManaSprite;
	public Sprite fullManaSprite;

	[Header ("Paineis, Imagem e Texto da Flecha")]
	public GameObject manaPanel;
	public GameObject arrowPanel;
	public Image arrowIcon;
	public TMP_Text arrowQuantityText;

	[Header ("Caixas e Textos das poções de HP e MP")]
	public GameObject HPBox;
	public GameObject MPBox;
	public TMP_Text quantityHPBoxText;
	public TMP_Text quantityMPBoxText;
	public RectTransform boxARectTransform;
	public RectTransform boxBRectTransform;
	public Vector2 boxAPosition;
	public Vector2 boxBPosition;

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa
	private void Start ()
	{
		gameControllerRevisao = FindObjectOfType (typeof (GameControllerRevisao)) as GameControllerRevisao;
		playerRevisao = FindObjectOfType (typeof (PlayerRevisao)) as PlayerRevisao;

		// Desativa paineis
		manaPanel.SetActive (false);
		arrowPanel.SetActive (false);
		HPBox.SetActive (false);
		MPBox.SetActive (false);

		VerifyPlayerHUD ();
		
		// Posiciona caixas
		boxAPosition = boxARectTransform.anchoredPosition;
		boxBPosition = boxBRectTransform.anchoredPosition;
	}

	private void Update () 
	{
		LifebarControl ();
		PotionBoxesPosition ();

		// Controla barra de mana ou quantidade de flechas
		if (manaPanel.activeSelf)
		{
			ManabarControl ();
		}
		else if (arrowPanel.activeSelf)
		{
			// Faz troca dos tipos de flechas
			if (Input.GetButtonDown ("ButtonL"))
			{
				int length = gameControllerRevisao.arrowIcons.Length;
				gameControllerRevisao.equippedArrowID = (gameControllerRevisao.equippedArrowID == 0 ? length - 1 : gameControllerRevisao.equippedArrowID--);
			}
			else if (Input.GetButtonDown ("ButtonR"))
			{
				int length = gameControllerRevisao.arrowIcons.Length;
				gameControllerRevisao.equippedArrowID = (gameControllerRevisao.equippedArrowID == length - 1 ? 0 : gameControllerRevisao.equippedArrowID++);
			}

			// Atualiza informacoes
			arrowIcon.sprite = gameControllerRevisao.arrowIcons[gameControllerRevisao.equippedArrowID];
			arrowQuantityText.text = string.Concat ("x", gameControllerRevisao.arrowsQuantity[gameControllerRevisao.equippedArrowID]);
		}

		// Define quantidades na UI
		quantityHPBoxText.text = gameControllerRevisao.potionsQuantity[0].ToString ();
		quantityMPBoxText.text = gameControllerRevisao.potionsQuantity[1].ToString ();
	}

	// ------------------- FUNCOES ------------------- //

	// Faz o controle dos sprites de vida
	private void LifebarControl ()
	{
		// Calcula porcentagem de vida
		float lifePercent = ((float) playerRevisao.ActualLife / (float) playerRevisao.MaxLife);

		// Ativa pocao de cura
		if (Input.GetButtonDown ("ItemA") && lifePercent < 1)
		{
			gameControllerRevisao.UsePotion (0);
		}

		// Reseta sprites
		foreach (Image image in lifebarImages)
		{
			image.enabled = true;
			image.sprite = fullLifeSprite;
		}

		// Controla sprites
		if (lifePercent == 1)
		{ }
		else if (lifePercent >= 0.9f)
		{
			lifebarImages[4].sprite = halfLifeSprite;
		}
		else if (lifePercent >= 0.8f)
		{
			lifebarImages[4].enabled = false;
		}
		else if (lifePercent >= 0.7f)
		{
			lifebarImages[4].enabled = false;
			lifebarImages[3].sprite = halfLifeSprite;
		}
		else if (lifePercent >= 0.6f)
		{
			lifebarImages[4].enabled = false;
			lifebarImages[3].enabled = false;
		}
		else if (lifePercent >= 0.5f)
		{
			lifebarImages[4].enabled = false;
			lifebarImages[3].enabled = false;
			lifebarImages[2].sprite = halfLifeSprite;
		}
		else if (lifePercent >= 0.4f)
		{
			lifebarImages[4].enabled = false;
			lifebarImages[3].enabled = false;
			lifebarImages[2].enabled = false;
		}
		else if (lifePercent >= 0.3f)
		{
			lifebarImages[4].enabled = false;
			lifebarImages[3].enabled = false;
			lifebarImages[2].enabled = false;
			lifebarImages[1].sprite = halfLifeSprite;
		}
		else if (lifePercent >= 0.2f)
		{
			lifebarImages[4].enabled = false;
			lifebarImages[3].enabled = false;
			lifebarImages[2].enabled = false;
			lifebarImages[1].enabled = false;
		}
		else if (lifePercent >= 0.1f)
		{
			lifebarImages[4].enabled = false;
			lifebarImages[3].enabled = false;
			lifebarImages[2].enabled = false;
			lifebarImages[1].enabled = false;
			lifebarImages[0].sprite = halfLifeSprite;
		}
		else if (lifePercent <= 0)
		{
			lifebarImages[4].enabled = false;
			lifebarImages[3].enabled = false;
			lifebarImages[2].enabled = false;
			lifebarImages[1].enabled = false;
			lifebarImages[0].enabled = false;
		}

		// Mostra barra caso haja pocoes
		HPBox.SetActive (gameControllerRevisao.potionsQuantity[0] > 0);
	}

	// Faz o controle dos sprites da barra de mana
	private void ManabarControl ()
	{
		// Calcula porcentagem de mana
		float manaPercent = ((float) gameControllerRevisao.playerActualMana / (float) gameControllerRevisao.playerMaxMana);

		// Ativa pocao de mana
		if (Input.GetButtonDown ("ItemB") && manaPercent < 1)
		{
			gameControllerRevisao.UsePotion (1);
		}

		// Reseta sprites
		foreach (Image image in manabarImages)
		{
			image.enabled = true;
			image.sprite = fullManaSprite;
		}

		// Controla sprites
		if (manaPercent == 1)
		{ }
		else if (manaPercent >= 0.9f)
		{
			manabarImages[4].sprite = halfManaSprite;
		}
		else if (manaPercent >= 0.8f)
		{
			manabarImages[4].enabled = false;
		}
		else if (manaPercent >= 0.7f)
		{
			manabarImages[4].enabled = false;
			manabarImages[3].sprite = halfManaSprite;
		}
		else if (manaPercent >= 0.6f)
		{
			manabarImages[4].enabled = false;
			manabarImages[3].enabled = false;
		}
		else if (manaPercent >= 0.5f)
		{
			manabarImages[4].enabled = false;
			manabarImages[3].enabled = false;
			manabarImages[2].sprite = halfManaSprite;
		}
		else if (manaPercent >= 0.4f)
		{
			manabarImages[4].enabled = false;
			manabarImages[3].enabled = false;
			manabarImages[2].enabled = false;
		}
		else if (manaPercent >= 0.3f)
		{
			manabarImages[4].enabled = false;
			manabarImages[3].enabled = false;
			manabarImages[2].enabled = false;
			manabarImages[1].sprite = halfManaSprite;
		}
		else if (manaPercent >= 0.2f)
		{
			manabarImages[4].enabled = false;
			manabarImages[3].enabled = false;
			manabarImages[2].enabled = false;
			manabarImages[1].enabled = false;
		}
		else if (manaPercent >= 0.1f)
		{
			manabarImages[4].enabled = false;
			manabarImages[3].enabled = false;
			manabarImages[2].enabled = false;
			manabarImages[1].enabled = false;
			manabarImages[0].sprite = halfManaSprite;
		}
		else if (manaPercent <= 0)
		{
			manabarImages[4].enabled = false;
			manabarImages[3].enabled = false;
			manabarImages[2].enabled = false;
			manabarImages[1].enabled = false;
			manabarImages[0].enabled = false;
		}

		// Mostra barra caso haja pocoes
		MPBox.SetActive (gameControllerRevisao.potionsQuantity[1] > 0);
	}

	// Posiciona as caixas indicativas de pocoes de acordo com as quantidades das mesmas
	public void PotionBoxesPosition ()
	{
		if (gameControllerRevisao.potionsQuantity[0] > 0)
		{
			HPBox.GetComponent<RectTransform>().anchoredPosition = boxAPosition;
			MPBox.GetComponent<RectTransform>().anchoredPosition = boxBPosition;
		}
		else 
		{
			HPBox.GetComponent<RectTransform>().anchoredPosition = boxBPosition;
			MPBox.GetComponent<RectTransform>().anchoredPosition = boxAPosition;
		}
	}

	public void VerifyPlayerHUD ()
	{
		// Ativa de acordo com classe (tipo) de personagem
		if (gameControllerRevisao.classID[gameControllerRevisao.playerID] == 1)
		{
			arrowIcon.sprite = gameControllerRevisao.arrowIcons[gameControllerRevisao.equippedArrowID];
			arrowPanel.SetActive (true);
		}
		else if (gameControllerRevisao.classID[gameControllerRevisao.playerID] == 2)
		{
			manaPanel.SetActive (true);
		}
	}
}