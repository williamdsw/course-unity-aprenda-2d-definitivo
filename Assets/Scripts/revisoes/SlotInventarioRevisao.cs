using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SlotInventarioRevisao : MonoBehaviour 
{
    // Variaveis de controle
    public int slotID;

    // Variaveis de Components / Objetos
	private GameControllerRevisao gameControllerRevisao;
    private PainelItemInfoRevisao painelItemInfoRevisao;
    private AudioControllerRevisao audioControllerRevisao;
    public GameObject slotObject;

    // ------------------- FUNCOES UNITY ------------------- //

    private void Start () 
    {
        // Inicializa
        gameControllerRevisao = FindObjectOfType<GameControllerRevisao>();
        painelItemInfoRevisao = FindObjectOfType<PainelItemInfoRevisao>();
        audioControllerRevisao = FindObjectOfType<AudioControllerRevisao>();

        // Proprio botao do slot
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener (UseItem);
    }

    // ------------------- FUNCOES ------------------- //

    private void UseItem ()
    {
        if (slotObject != null)
        {
            // Abre painel de informacoes do item
            audioControllerRevisao.PlayFX (audioControllerRevisao.fxClick, 1f);
            painelItemInfoRevisao.slotID = this.slotID;
            painelItemInfoRevisao.slotObject = this.slotObject;
            painelItemInfoRevisao.LoadItemInformation ();
            var itemInfoPanel = gameControllerRevisao.itemInfoPanel;
            var btnFirstItemInfo = gameControllerRevisao.btnFirstItemInfo;
            gameControllerRevisao.OpenPanel (null, itemInfoPanel, btnFirstItemInfo, GameStateRevisao.ITEMS);
        }
    }
}