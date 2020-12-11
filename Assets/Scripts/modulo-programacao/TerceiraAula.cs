using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 3 : OPERADORES RELACIONAIS - TOMADA DE DECISAO */
public class TerceiraAula : MonoBehaviour
{
    /* Variaveis */
    private int a = 2;
    private int b = 3;

    private void Start ()
    {
        /* Printando valores */
        Debug.LogFormat ("Valor de a = {0}", a);
        Debug.LogFormat ("Valor de b = {0}", b);

        /* == (Se for igual) */ 
        if (a == b)
        {
            print ("A e B são iguais sem else");
        }

        /* != (Se for diferente) */
        if (a != b)
        {
            print ("A e B são diferentes sem else ");
        }

        /* == (Se for igual) (Senao) */
        if (a == b)
        {
            print ("A é igual a B");
        }
        else
        {
            print ("A é diferente de B");
        }
    }
}