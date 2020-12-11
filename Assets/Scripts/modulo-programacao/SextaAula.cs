using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 6 : TOMADA DE DECISAO "SWITCH" */
public class SextaAula : MonoBehaviour
{
    private int numero = 5;

    private void Start ()
    {
        /* "switch" = Outra tomada de decisao, otimizada para varios else if */
        switch (numero)
        {
            case 1:
            {
                print ("É o numero 1");
                break;
            }
            
            case 2:
            {
                print ("É o numero 2");
                break;
            }

            case 3:
            {
                print ("É o numero 3");
                break;
            }

            case 4:
            {
                print ("É o numero 4");
                break;
            }

            case 5:
            {
                print ("É o numero 5");
                break;
            }

            default:
            {
                print ("Não está entre 1 e 5");
                break;
            }
        }
    }
}