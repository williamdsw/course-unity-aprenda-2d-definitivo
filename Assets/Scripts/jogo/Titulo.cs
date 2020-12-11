using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Titulo : MonoBehaviour 
{
	private AudioController audioController;

	// Components / Objetos
	public Button btnNovoJogo;
	public Button btnCarregarJogo;
	public Button btnOpcoes;
	public Button btnSair;

	[Header ("Painel Novo Jogo")]
	public GameObject painelNovoJogo;
	public Button btnNovoSlot1;
	public Button btnNovoSlot2;
	public Button btnNovoSlot3;
	public GameObject btnDelete1;
	public GameObject btnDelete2;
	public GameObject btnDelete3;
	public Button btnNovoJogoVoltar;

	[Header ("Painel Carregar Jogo")]
	public GameObject painelCarregarJogo;
	public Button btnCarregarSlot1;
	public Button btnCarregarSlot2;
	public Button btnCarregarSlot3;
	public Button btnCarregarJogoVoltar;

	[Header ("Painel Seleção de Personagem")]
	public GameObject painelSelecaoPersonagem;
	public Button btnBarbaro;
	public Button btnArqueiro;
	public Button btnMago;
	public TMP_Text textNomePersonagem;
	public Button btnSelecaoPersonagemVoltar;

	[Header ("Painel Opções")]
	public GameObject painelOpcoes;
	public Slider volumeMusicaSlider;
	public Slider volumeEfeitosSlider;
	public Button btnOpcoesVoltar;

	// ------------------- FUNCOES UNITY ------------------- //

	// Inicializa
	private void Start () 
	{
		audioController = FindObjectOfType<AudioController>();
		textNomePersonagem.text = string.Empty;
		DefinirEventos ();
		VerificarJogoSalvo ();
	}

	// ------------------- FUNCOES ------------------- //

	// Define eventos dos botoes
	private void DefinirEventos ()
	{
		// TITULO

		if (btnNovoJogo != null && btnCarregarJogo != null && btnOpcoes != null && btnSair != null)
		{
			btnNovoJogo.onClick.AddListener (delegate
			{
				TocarClick ();
				painelNovoJogo.SetActive (true);
				btnNovoSlot1.Select ();
			});

			btnCarregarJogo.onClick.AddListener (delegate
			{
				TocarClick ();
				painelCarregarJogo.SetActive (true);
				btnCarregarSlot1.Select ();
			});

			btnOpcoes.onClick.AddListener (delegate
			{
				TocarClick ();
				painelOpcoes.SetActive (true);
				volumeMusicaSlider.Select ();
			});

			btnSair.onClick.AddListener (delegate
			{
				Application.Quit ();
			});
		}

		// PAINEL NOVO JOGO

		if (btnNovoSlot1 != null && btnNovoSlot2 != null && btnNovoSlot3 != null)
		{
			btnNovoSlot1.onClick.AddListener (delegate 
			{
				TocarClick ();
				NovoJogo (1);
				painelNovoJogo.SetActive (false);
				painelSelecaoPersonagem.SetActive (true);
				btnBarbaro.Select ();
			});
			
			btnNovoSlot2.onClick.AddListener (delegate 
			{
				TocarClick ();
				NovoJogo (2);
				painelNovoJogo.SetActive (false);
				painelSelecaoPersonagem.SetActive (true);
				btnBarbaro.Select ();
			});

			btnNovoSlot3.onClick.AddListener (delegate 
			{
				TocarClick ();
				NovoJogo (3);
				painelNovoJogo.SetActive (false);
				painelSelecaoPersonagem.SetActive (true);
				btnBarbaro.Select ();
			});
		}

		if (btnDelete1 != null && btnDelete2 != null && btnDelete3 != null)
		{
			btnDelete1.GetComponent<Button>().onClick.AddListener (delegate 
			{
				TocarClick ();
				DeletarSlot (1);
			});
			
			btnDelete2.GetComponent<Button>().onClick.AddListener (delegate 
			{
				TocarClick ();
				DeletarSlot (2);
			});

			btnDelete3.GetComponent<Button>().onClick.AddListener (delegate 
			{
				TocarClick ();
				DeletarSlot (3);
			});
		}

		if (btnNovoJogoVoltar != null)
		{
			btnNovoJogoVoltar.onClick.AddListener (delegate 
			{
				TocarClick ();
				painelNovoJogo.SetActive (false);
				btnNovoJogo.Select ();
			});
		}

		// PAINEL CARREGAR JOGO
		
		if (btnCarregarSlot1 != null && btnCarregarSlot2 != null && btnCarregarSlot3 != null)
		{
			btnCarregarSlot1.onClick.AddListener (delegate 
			{
				TocarClick ();
				CarregarJogo (1);
			});

			btnCarregarSlot2.onClick.AddListener (delegate 
			{
				TocarClick ();
				CarregarJogo (2);
			});

			btnCarregarSlot3.onClick.AddListener (delegate 
			{
				TocarClick ();
				CarregarJogo (3);
			});
		}

		if (btnCarregarJogoVoltar != null)
		{
			btnCarregarJogoVoltar.onClick.AddListener (delegate 
			{
				TocarClick ();
				painelCarregarJogo.SetActive (false);
				btnCarregarJogo.Select ();
			});
		}

		// PAINEL SELECIONAR PERSONAGEM

		if (btnBarbaro != null && btnArqueiro != null && btnMago != null)
		{
			btnBarbaro.onClick.AddListener (delegate
			{
				TocarClick ();
				SelecionarPersonagem (0);
			});

			btnArqueiro.onClick.AddListener (delegate
			{
				TocarClick ();
				SelecionarPersonagem (5);
			});

			btnMago.onClick.AddListener (delegate
			{
				TocarClick ();
				SelecionarPersonagem (9);
			});
		}

		if (btnSelecaoPersonagemVoltar != null)
		{
			btnSelecaoPersonagemVoltar.onClick.AddListener (delegate
			{
				TocarClick ();
				painelSelecaoPersonagem.SetActive (false);
				painelNovoJogo.SetActive (true);
				btnNovoSlot1.Select ();
			});
		}

		// PAINEL OPCOES

		if (volumeMusicaSlider != null && volumeEfeitosSlider != null)
		{
			// Define valores dos Sliders
			volumeMusicaSlider.value = audioController.volumeMaximoMusica;
			volumeEfeitosSlider.value = audioController.volumeMaximoEfeitos;

			// Atualiza valores de acordo com valor do slider

			volumeMusicaSlider.onValueChanged.AddListener (delegate
			{
				float volumeTemporario = volumeMusicaSlider.value;
				audioController.volumeMaximoMusica = volumeTemporario;
				audioController.sourceMusica.volume = volumeTemporario;
				PlayerPrefs.SetFloat ("volumeMaximoMusica", volumeTemporario);
			});

			volumeEfeitosSlider.onValueChanged.AddListener (delegate
			{
				float volumeTemporario = volumeEfeitosSlider.value;
				audioController.volumeMaximoEfeitos = volumeTemporario;
				PlayerPrefs.SetFloat ("volumeMaximoEfeitos", volumeTemporario);
			});
		}

		if (btnOpcoesVoltar != null)
		{
			btnOpcoesVoltar.onClick.AddListener (delegate
			{
				TocarClick ();
				painelOpcoes.SetActive (false);
				btnOpcoes.Select ();
			});
		}
	}

	// Seleciona o personagem para a primeira fase
	private void SelecionarPersonagem (int id)
	{
		PlayerPrefs.SetInt ("id_personagem", id);
		SceneManager.LoadScene ("Load");
	}

	// Exibe nome do personagem de acordo com "PointerEnter" e "PointerExit"
	public void ExibirNomePersonagem (string nome)
	{
		textNomePersonagem.text = nome;
	}

	// Verifica jogos salvos e habilita / desabilita slots
	private void VerificarJogoSalvo ()
	{
		// Desabilita botoes
		btnCarregarJogo.interactable = false;
		btnCarregarSlot1.interactable = false;
		btnCarregarSlot2.interactable = false;
		btnCarregarSlot3.interactable = false;

		// Habilita botoes
		btnNovoJogo.interactable = true;
		btnNovoSlot1.interactable = true;
		btnNovoSlot2.interactable = true;
		btnNovoSlot3.interactable = true;

		// Desabilita botoes de excluir save
		btnDelete1.SetActive (false);
		btnDelete2.SetActive (false);
		btnDelete3.SetActive (false);

		// Verifica cada save e habilita botoes relacionados

		if (File.Exists (string.Concat (Application.persistentDataPath, "/playerdata1.dat")))
		{
			btnCarregarSlot1.interactable = true;
			btnNovoSlot1.interactable = false;
			btnDelete1.SetActive (true);
		}

		if (File.Exists (string.Concat (Application.persistentDataPath, "/playerdata2.dat")))
		{
			btnCarregarSlot2.interactable = true;
			btnNovoSlot2.interactable = false;
			btnDelete2.SetActive (true);
		}

		if (File.Exists (string.Concat (Application.persistentDataPath, "/playerdata3.dat")))
		{
			btnCarregarSlot3.interactable = true;
			btnNovoSlot3.interactable = false;
			btnDelete3.SetActive (true);
		}

		// Habilita botao de carregar jogo
		if (btnCarregarSlot1.interactable || btnCarregarSlot2.interactable || btnCarregarSlot3.interactable)
		{
			btnCarregarJogo.interactable = true;
		}
	}

	// Salva novo slot no PlayerPrefs
	private void NovoJogo (int slot)
	{
		string nome = "playerdata";
		nome = string.Concat (nome, slot, ".dat");
		PlayerPrefs.SetString ("slot", nome);
	}

	// Carrega slot existente no PlayerPrefs
	private void CarregarJogo (int slot)
	{
		string nome = "playerdata";
		nome = string.Concat (nome, slot, ".dat");
		PlayerPrefs.SetString ("slot", nome);
		SceneManager.LoadScene ("Load");
	}

	// Deleta um slot existente com base no ID do mesmo
	private void DeletarSlot (int slot)
	{
		string caminho = string.Concat (Application.persistentDataPath, "/playerdata", slot, ".dat");

		if (File.Exists (caminho))
		{
			File.Delete (caminho);
		}

		VerificarJogoSalvo ();
	}

	// Toca o efeito de clique dos botoes
	public void TocarClick ()
	{
		audioController.TocarEfeito (audioController.efeitoClick, 1);
	}
}