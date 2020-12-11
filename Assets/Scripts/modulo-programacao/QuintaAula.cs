using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 5 : OPERADORES LOGICOS, AND e OR */
public class QuintaAula : MonoBehaviour
{
    private float a = 2;
    private float b = 3;
    private float c = 5;

    private void Start ()
    {
        /* Utilizando "E" (&&) = Quando ambas condicoes forem true ou false */
        if (a * b > 5 && c - b > a)
        {
            print ("Deu no E (&&)");
        }
        else
        {
            print ("Não deu no E (&&)");
        }

        /* Utilizando "OU" (||) = Quando uma das condicoes for true */
        if (a * b > 5 || c - b > a)
        {
            print ("Deu no OU (||)");
        }
        else
        {
            print ("Não deu no OU (||)");
        }

        /* Utilizando "NOT" (!) = Para negar uma condição */
        if (!(a * b == 5))
        {
            print ("Negacao de a * b = 5 é true");
        }
    }
}