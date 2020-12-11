using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour 
{
	public AudioSource sourceMusica;		// Fonte de musica
	public AudioSource sourceEfeitos;		// Efeitos sonoros

	[Header ("Musicas")]
	public AudioClip musicaTitulo;
	public AudioClip musicaFase1;
	public AudioClip musicaMenu;

	[Header ("Efeitos")]
	public AudioClip efeitoClick;
	public AudioClip efeitoEspada;
	public AudioClip efeitoMachado;
	public AudioClip efeitoArco;
	public AudioClip efeitoCajado;
	public AudioClip efeitoBau;
	public AudioClip efeitoHit;
	public AudioClip efeitoSpawnMoedas;
	public AudioClip efeitoMoedas;
	public AudioClip efeitoPorta;

	// Configuracoes dos audios
	public float volumeMaximoMusica;
	public float volumeMaximoEfeitos;

	// Configuracoes da troca de musica
	private AudioClip novaMusica;
	private string novaCena;
	private bool vaiTrocarCena;

	// ------------------- FUNCOES UNITY ------------------- //

	// Carrega as configuracoes de audio do aparelho
	private void Start () 
	{
		DontDestroyOnLoad (this.gameObject);

		// Define pre-configuracoes
		if (PlayerPrefs.GetInt ("valoresIniciais") == 0)
		{
			PlayerPrefs.SetInt ("valoresIniciais", 1);
			PlayerPrefs.SetFloat ("volumeMaximoMusica", 1f);
			PlayerPrefs.SetFloat ("volumeMaximoEfeitos", 1f);
		}

		// Carrega configuracoes
		volumeMaximoMusica = PlayerPrefs.GetFloat ("volumeMaximoMusica");
		volumeMaximoEfeitos = PlayerPrefs.GetFloat ("volumeMaximoEfeitos");

		TrocarMusica (musicaTitulo, "Titulo", true);
	}

	// ------------------- FUNCOES ------------------- //

	// Toca um efeito de acordo com volume especificado
	public void TocarEfeito (AudioClip efeito, float volume)
	{
		float volumeTemporario = (volume > volumeMaximoEfeitos ? volumeMaximoEfeitos : volume);

		// Vai trocar apenas uma vez
		sourceEfeitos.volume = volumeTemporario;
		sourceEfeitos.PlayOneShot (efeito);
	}

	// Passa valores e chama corrotina
	public void TrocarMusica (AudioClip musicaNova, string nomeCena, bool vaiMudarCena)
	{
		// Passa valores
		novaMusica = musicaNova;
		novaCena = nomeCena;
		vaiTrocarCena = vaiMudarCena;

		StartCoroutine ("TrocarMusicaCorrotina");
	}

	// ------------------- CORROTINAS ------------------- //

	// Altera o volume e troca a musica do Player
	private IEnumerator TrocarMusicaCorrotina ()
	{
		// Abaixa volume da musica
		for (float volume = volumeMaximoMusica; volume >= 0; volume -= 0.1f)
		{
			// "WaitForSecondsRealtime" = O Time.timeScale nao interfere nessa funcao
			yield return new WaitForSecondsRealtime (0.1f);
			sourceMusica.volume = volume;
		}

		sourceMusica.volume = 0;

		// Troca audio
		sourceMusica.clip = novaMusica;
		sourceMusica.Play ();

		// Aumenta volume da musica
		for (float volume = 0; volume <= volumeMaximoMusica; volume += 0.1f)
		{
			// "WaitForSecondsRealtime" = O Time.timeScale nao interfere nessa funcao
			yield return new WaitForSecondsRealtime (0.1f);
			sourceMusica.volume = volume;
		}

		if (vaiTrocarCena)
		{
			SceneManager.LoadScene (novaCena);
		}
	}
}