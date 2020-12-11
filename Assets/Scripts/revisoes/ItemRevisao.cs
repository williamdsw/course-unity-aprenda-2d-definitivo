using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRevisao : MonoBehaviour 
{
    // Variaveis de Controle
	public int itemID;

    // Variaveis de Objetos / Components
    private GameControllerRevisao gameControllerRevisao;

    // ------------------- FUNCOES UNITY ------------------- //

    // Inicializa
    private void Start () 
    {
        gameControllerRevisao = FindObjectOfType<GameControllerRevisao>();    
    }

    // ------------------- FUNCOES ------------------- //

    // Usa item enviando o ID do proprio
    public void UseItem ()
    {
        gameControllerRevisao.UseWeaponItem (itemID);
    }
}