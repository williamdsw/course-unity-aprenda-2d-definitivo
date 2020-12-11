using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventarioRevisao : MonoBehaviour 
{
	[Header ("Propriedades do Slot")]
    public List<Button> slots;
    public List<Image> itemIcons;

    [Header ("Texto das quantidades")]
    public TextMeshProUGUI textQuantityLifePotion;
    public TextMeshProUGUI textQuantityManaPotion;
    public TextMeshProUGUI textQuantityDefaultArrow;
    public TextMeshProUGUI textQuantityBlueArrow;
    public TextMeshProUGUI textQuantityYellowArrow;
    
    [Header ("Quantidades de Cada Item")]
    public int quantityLifePotion;
    public int quantityManaPotion;
    public int quantityDefaultArrow;
    public int quantityBlueArrow;
    public int quantityYellowArrow;

    // Components / Objetos
    private GameControllerRevisao gameControllerRevisao;
    public List<GameObject> inventaryItems;
    public List<GameObject> loadedItems;

    // ------------------- FUNCOES UNITY ------------------- //

    // Inicializa
    private void Start () 
    {
        gameControllerRevisao = FindObjectOfType (typeof (GameControllerRevisao)) as GameControllerRevisao;
    }

    // ------------------- FUNCOES ------------------- //

    public void LoadInventory ()
    {
        // Desabilita botoes
        foreach (Button slot in slots)
        {
            slot.interactable = false;
        }

        // Desabilita sprites e objeto
        foreach (Image icon in itemIcons)
        {
            icon.sprite = null;
            icon.gameObject.SetActive (false);
        }

        // Define textos padrao
        textQuantityLifePotion.text = string.Concat ("x", gameControllerRevisao.potionsQuantity[0]);
        textQuantityManaPotion.text = string.Concat ("x", gameControllerRevisao.potionsQuantity[1]);
        textQuantityDefaultArrow.text = string.Concat ("x", gameControllerRevisao.arrowsQuantity[0]);
        textQuantityBlueArrow.text = string.Concat ("x", gameControllerRevisao.arrowsQuantity[1]);
        textQuantityYellowArrow.text = string.Concat ("x", gameControllerRevisao.arrowsQuantity[2]);

        ClearLoadedItems ();

        // Adiciona objetos e faz o slot ser clicavel
        int index = 0;
        foreach (GameObject item in inventaryItems)
        {
            GameObject temp = Instantiate (item);
            ItemRevisao itemInfoRevisao = temp.GetComponent<ItemRevisao>();
            loadedItems.Add (temp);

            // Botao slot
            slots[index].GetComponent<SlotInventarioRevisao>().slotObject = temp;
            slots[index].interactable = true;

            // Imagem slot
            itemIcons[index].sprite = gameControllerRevisao.inventaryImages[itemInfoRevisao.itemID];
            itemIcons[index].gameObject.SetActive (true);
            index++;
        }
    }

    // Destroi cada item carregado e limpa a lista
    public void ClearLoadedItems ()
    {
        foreach (GameObject item in loadedItems)
        {
            Destroy (item);
        }

        loadedItems.Clear ();
    }
}