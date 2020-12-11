using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncoesBasicasRevisao : MonoBehaviour
{
    // Awake () = E chamado quando a instancia do script e carregada
    private void Awake () 
    {
        print ("Começou o Awake");
    }

    // Start () = E chamado quando o script esta habilitado
    private void Start () 
    {
        print ("Começou o Start");
    }

    // Update () = E chamado a cada frame do jogo (0.2 seg)
    private void Update () 
    {
        print ("Rodando o Update...");
    }

    // FixedUpdate () = E chamado a cada frame fixo (recomendado para operacoes com fisica)
    private void FixedUpdate () 
    {
        print ("Rodando o Fixed Update");
    }

    // LateUpdate () = E chamado apos todos comandos do Update
    private void LateUpdate () 
    {
        print ("Rodando o Late Update");
    }

    // COLLIDER

    // OnCollisionEnter2D () = Quando objeto colide com algo (Necessario Collider)
    private void OnCollisionEnter2D (Collision2D other) 
    {
        print ("Colidiu com algo");
    }

    // OnCollisionStay2D () = Quando objeto esta colidindo com algo (Necessario Collider)
    private void OnCollisionStay2D (Collision2D other) 
    {
        print ("Continua colidindo com algo");
    }

    // OnCollisionExit2D () = Quando objeto parou de colidir com algo (Necessario Collider)
    private void OnCollisionExit2D (Collision2D other) 
    {
        print ("Parou de colidir com algo");
    }

    // TRIGGER

    // OnTriggerEnter2D = Quando engatilha (colide) com algo (Necessario Collider e IsTrigger habilitado)
    private void OnTriggerEnter2D (Collider2D other) 
    {
        print ("Gatilhou com algo");
    }

    // OnTriggerStay2D = Quando continua gatilhando (colidindo) com algo (Necessario Collider e IsTrigger habilitado)
    private void OnTriggerStay2D (Collider2D other) 
    {
        print ("Gatilhando com algo");
    }

    // OnTriggerExit2D = Quando desengatilha (sai da colisao) com algo (Necessario Collider e IsTrigger habilitado)
    private void OnTriggerExit2D (Collider2D other) 
    {
        print ("Saiu do gatilho");
    }
}