using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 9 : FOR */
public class NonaAula : MonoBehaviour
{
    /* Declarando um array */
    private int[] notas = new int[5] {2, 10, 5, 7, 8};

    private void Start ()
    {
        /* For: Outro loop para iterar arrays e colecoes */ 
        for (int i = 0; i < notas.Length; i++)
        {
            /* Exibe indice e valor */
            Debug.LogFormat ("Nota: {0} - Valor: {1}", (i + 1), notas[i]);
        }

        print ("Fim do FOR");
    }
}