using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AULA 4 : GET KEY DOWN */
public class QuartaAula : MonoBehaviour
{
    private void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {
            print ("Apertou espaço");
        }
    }
}