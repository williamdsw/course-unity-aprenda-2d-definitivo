using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 7 : ARRAY */
public class SetimaAula : MonoBehaviour
{
    /* Declarando um array */
    private int[] notas = new int[5] {2, 10, 5, 7, 8};

    private void Start ()
    {
        /* Pegando valor do array pelo indice */
        Debug.LogFormat ("Segundo elemento: {0}", notas[1]);

        /* Pegando numero itens do array */
        Debug.LogFormat ("Tamanho do array de notas: {0}", notas.Length);
    }    
}