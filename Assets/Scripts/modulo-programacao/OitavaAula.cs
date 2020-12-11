using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 8 : WHILE */
public class OitavaAula : MonoBehaviour
{
    /* Declarando um array */
    private int[] notas = new int[5] {2, 10, 5, 7, 8};

    private void Start ()
    {
        /* Variavel para indice */
        int index = 0;

        /* while  = serve para iterar um array */
        while (index < notas.Length)
        {
            /* Exibe valor e incrementa */
            Debug.LogFormat ("Nota: {0} - Valor: {1}", (index + 1), notas[index]);
            index++;
        }

        print ("Fim do WHILE");
    }
}