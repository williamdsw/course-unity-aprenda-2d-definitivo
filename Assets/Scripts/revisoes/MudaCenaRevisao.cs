using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MudaCenaRevisao : MonoBehaviour 
{
    // Objetos
	private EfeitoFadeRevisao efeitoFadeRevisao;
    private GameControllerRevisao gameControllerRevisao;

    // Variaveis de controle
    public string nextScene;

    // ------------------- FUNCOES UNITY ------------------- //

    // Inicializa 
    private void Start () 
    {
        efeitoFadeRevisao = FindObjectOfType<EfeitoFadeRevisao>();
        gameControllerRevisao = FindObjectOfType<GameControllerRevisao>();
    }

    // ------------------- FUNCOES ------------------- //

    public void Interact ()
    {
        StartCoroutine ("ChangeScene");
    }

    // ------------------- CORROTINAS ------------------- //

    // Aplica efeito fade-in e muda cena
    private IEnumerator ChangeScene ()
    {
        efeitoFadeRevisao.FadeIn ();
        yield return new WaitWhile (() => efeitoFadeRevisao.blackoutImage.color.a < 0.9f);

        if (nextScene.Equals ("Titulo"))
        {
            Destroy (gameControllerRevisao.gameObject);
        }

        SceneManager.LoadScene (nextScene);
    }
}