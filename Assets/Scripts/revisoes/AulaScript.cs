using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AulaScript : MonoBehaviour
{
    // Variaveis
    public bool isGrounded = true;
    public int idAnimation = 0;
    private Animator animator;

    private void Start ()
    {
        // Pegando component para a variavel
        animator = this.GetComponent<Animator> ();
    }

    private void Update ()
    {
        // Input.GetAxisRaw () = Pega valores float dos eixos "Horizontal" e "Vertical" de acordo com as setas do teclado */
        float horizontal = Input.GetAxisRaw ("Horizontal");
        float vertical = Input.GetAxisRaw ("Vertical");

        // Se agachar
        if (vertical < 0)
        {
            idAnimation = 2;
        }
        else if (horizontal != 0)
        {
            idAnimation = 1;
        }
        else
        {
            idAnimation = 0;
        }

        // Se apertar o botao de ataque e estiver em pe
        if (Input.GetButtonDown ("Fire1") & vertical >= 0)
        {
            // Ativa o trigger de ataque
            animator.SetTrigger ("attack");
        }

        // Setando parametros
        animator.SetBool ("grounded", isGrounded);
        animator.SetInteger ("id_animation", idAnimation);
    }
}