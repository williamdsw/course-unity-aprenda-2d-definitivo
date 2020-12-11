using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 1 : Variaveis, tipo de Dados, Print e Debug, Atribuicao de Valores */
public class PrimeiraAula : MonoBehaviour
{
    /* Variaveis */
    public string tipoTexto;
    public int tipoInteiro;
    public float tipoFlutuante;
    public bool tipoBooleano;

    /* Instancia de outra classe */
    public Camera minhaCamera;

    private void Start ()
    {
        /* Printando texto */
        print ("Hello World pelo print !");
        Debug.Log ("Hello World pelo Debug Log!");
        Debug.LogError ("Testando LogError do Debug");
        Debug.LogWarning ("Testando LogWarning do Debug");

        /* Definindo valores para as variaveis */
        tipoTexto = "Trent Reznor";
        tipoInteiro = 666;
        tipoFlutuante = 6.66F;
        tipoBooleano = true;

        /* printando essas variaveis */
        print (tipoTexto);
        print (tipoInteiro);
        print (tipoFlutuante);
        print (tipoBooleano);

        tipoInteiro = 0;
    }

    private void Update ()
    {
        tipoInteiro++;
    }
}