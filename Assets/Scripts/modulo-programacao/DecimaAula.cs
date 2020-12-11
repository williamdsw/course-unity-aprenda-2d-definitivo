using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 10 : FOREACH */
public class DecimaAula : MonoBehaviour
{
    /* Declarando um array */
    private int[] notas = new int[5] {2, 10, 5, 7, 8};

    private void Start ()
    {
        /* foreach = um for aprimorado, especial para ler array e colecoes */
        foreach (int nota in notas)
        {
            Debug.LogFormat ("Valor da nota: {0}", nota);
        }

        print ("Fim do FOREACH");
    }
}