using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PainelItemInfoRevisao : MonoBehaviour 
{
    // Components - Objects
    private GameControllerRevisao gameControllerRevisao;
    public GameObject slotObject;
    public GameObject[] improvementsGameObjects;

    // Variaveis de Controle
    public int slotID;
    private int improvement;
    private int weaponID;

    [Header ("Configuração do HUD")]
    public Image itemImage;
    public TMP_Text itemNameText;
    public TMP_Text weaponDamageText;

    [Header ("Botões")]
    public Button btnUpgrade;
    public Button btnEquip;
    public Button btnDelete;

    // ------------------- FUNCOES UNITY ------------------- //

    // Inicializa
    private void Start () 
    {
        gameControllerRevisao = FindObjectOfType (typeof (GameControllerRevisao)) as GameControllerRevisao;
        BindEvents ();
    }

    // ------------------- FUNCOES ------------------- //
    
    // Define eventos dos botoes
    public void BindEvents ()
    {
        if (btnUpgrade != null)
        {
            btnUpgrade.onClick.AddListener (null);
        }

        if (btnEquip != null)
        {
            btnEquip.onClick.AddListener (null);
        }

        if (btnDelete != null)
        {
            btnDelete.onClick.AddListener (null);
        }
    }

    // Carrega informacoes do item
    public void LoadItemInformation ()
    {
        // Recupera informacoes
        ItemRevisao itemRevisao = slotObject.GetComponent<ItemRevisao>();
        weaponID = itemRevisao.itemID;
        string damageType = gameControllerRevisao.DamageTypes[gameControllerRevisao.damageTypesInt[weaponID]];
        int minDamage = gameControllerRevisao.minDamages[weaponID];
        int maxDamage = gameControllerRevisao.maxDamages[weaponID];
        int improvement = gameControllerRevisao.weaponImprovements[weaponID];

        // Passa informacoes para UI
        itemNameText.text = gameControllerRevisao.weaponsName[weaponID];
        itemImage.sprite = gameControllerRevisao.inventaryImages[weaponID];
        weaponDamageText.text = string.Concat ("Damage: ", minDamage, " - ", maxDamage, " / ", damageType);

        LoadImprovement ();

        // Define controle do primeiro slot
        if (slotID == 0)
        {
            btnEquip.interactable = false;
            btnDelete.interactable = false;
        }
        else 
        {
            // Define se o usuario pode equipar um item pela classe
            int weaponClassID = gameControllerRevisao.weaponClassID[weaponID];
            int classID = gameControllerRevisao.classID[gameControllerRevisao.playerID];

            if (weaponClassID == classID)
            {
                btnEquip.interactable = true;
                btnDelete.interactable = true;
            }
            else 
            {
                btnEquip.interactable = false;
                btnDelete.interactable = false;
            }
        }
    }

    // Aprimora valor do item
    public void UpgradeItem ()
    {
        gameControllerRevisao.ImproveWeapon (slotID);
        LoadImprovement ();
    }

    // Equipa item para player
    public void EquipItem ()
    {
        slotObject.SendMessage ("UseItem", SendMessageOptions.DontRequireReceiver);
        gameControllerRevisao.ChangeItems (slotID);
    }

    // Descarta item do inventario
    public void DiscardItem ()
    {
        gameControllerRevisao.DeleteItem (slotID);
    }

    // Carrega aprimoramento
    public void LoadImprovement ()
    {
        improvement = gameControllerRevisao.weaponImprovements[weaponID];
        btnUpgrade.interactable = (improvement < 10);

        // Desativa objetos por padrao
        foreach (GameObject gameObject in improvementsGameObjects)
        {
            gameObject.SetActive (false);
        }

        // Ativando objetos pelo aprimoramento
        for (int i = 0; i < improvement; i++)
        {
            improvementsGameObjects[i].SetActive (true);
        }
    }
}