using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 2 :  Operacoes Basicas */
public class SegundaAula : MonoBehaviour
{
    /* Variaveis */
    private float a = 3f;
    private float b = 5f;
    private float adicao = 0;
    private float subtracao = 0;
    private float multiplicacao = 0;
    private float divisao = 0;

    private void Start ()
    {
        adicao = a + b;
        subtracao = a - b;
        multiplicacao = a * b;
        divisao = a / b;

        /* Printando operacoes aritmeticas */
        Debug.LogFormat ("Adição: 3 + 5 = {0}", adicao);
        Debug.LogFormat ("Substração: 3 - 5 = {0}", subtracao);
        Debug.LogFormat ("Multiplicação: 3 * 5 = {0}", multiplicacao);
        Debug.LogFormat ("Divisão: 3 / 5 = {0}", divisao);
    }
}