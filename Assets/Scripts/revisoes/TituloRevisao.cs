using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TituloRevisao : MonoBehaviour 
{
    private AudioControllerRevisao audioControllerRevisao;

    // Components / Objetos
    public Button btnNewGame;
    public Button btnLoadGame;
    public Button btnOptions;
    public Button btnQuit;

    [Header ("New Game Panel Objects")]
    public GameObject panelNewGame;
    public Button[] btnNewSlots;
    public GameObject[] btnDeleteSlots;
    public Button btnNewGameBack;

    [Header ("Load Game Panel Objects")]
    public GameObject panelLoadGame;
    public Button[] btnLoadSlots;
    public Button btnLoadGameBack;

    [Header ("Select Character Objects")]
    public GameObject panelSelectCharacter;
    public Button[] btnCharacters;
    public TMP_Text textCharacterName;
    public Button btnSelectCharacterBack;

    [Header ("Options Objects")]
    public GameObject panelOptions;
    public Slider sliderMusicVolume;
    public Slider sliderFXVolume;
    public Button btnOptionsBack;


    // ------------------- FUNCOES UNITY ------------------- //

    private void Start ()
    {
        audioControllerRevisao = FindObjectOfType<AudioControllerRevisao>();
        textCharacterName.text = string.Empty;
        BindEvents ();
        VerifySavedGames ();
    }

    // ------------------- FUNCOES ------------------- //

    // Atribui evento de click para botao
    private void BindEvents ()
    {
        // TITULO

        if (btnNewGame != null && btnLoadGame != null && btnOptionsBack != null && btnQuit != null)
        {
            btnNewGame.onClick.AddListener (delegate
            {
                PlayClick ();
                panelNewGame.SetActive (true);
                btnNewSlots[0].Select ();
            });

            btnLoadGame.onClick.AddListener (delegate
            {
                PlayClick ();
                panelLoadGame.SetActive (true);
                btnLoadSlots[0].Select ();
            });

            btnOptionsBack.onClick.AddListener (delegate
            {
                PlayClick ();
                panelOptions.SetActive (true);
                sliderMusicVolume.Select ();
            });

            btnQuit.onClick.AddListener (delegate
            {
                Application.Quit ();
            });
        }

        // PAINEL NOVO JOGO

        int slot = 1;

        // Define evento click de cada botao
        foreach (Button button in btnNewSlots)
        {
            if (button != null)
            {
                button.onClick.AddListener (delegate
                {
                    PlayClick ();
                    NewGame (slot);
                    panelNewGame.SetActive (false);
                    panelSelectCharacter.SetActive (true);
                    btnCharacters[0].Select ();
                });

                slot++;
            }
        }

        slot = 1;

        // Define evento click de cada botao
        foreach (GameObject item in btnDeleteSlots)
        {
            if (item != null)
            {
                Button btn = item.GetComponent<Button>();
                btn.onClick.AddListener (delegate
                {
                    DeleteSlot (slot);
                });
            }
        }

        if (btnNewGameBack != null)
        {
            btnNewGameBack.onClick.AddListener (delegate
            {
                PlayClick ();
                panelNewGame.SetActive (false);
                btnNewGame.Select ();
            });
        }

        // PAINEL CARREGAR JOGO

        slot = 1;

        // Define evento click de cada botao
        foreach (Button button in btnLoadSlots)
        {
            if (button != null)
            {
                button.onClick.AddListener (delegate
                {
                    PlayClick ();
                    LoadGame (slot);
                });

                slot++;
            }
        }

        if (btnLoadGameBack != null)
        {
            btnLoadGameBack.onClick.AddListener (delegate
            {
                PlayClick ();
                panelLoadGame.SetActive (false);
                btnLoadGame.Select ();
            });
        }

        // PAINEL SELECIONAR PERSONAGEM

        int[] playerIDs = new int[] {0, 5, 9};

        // Define evento click de cada botao
        for (int i = 0; i < btnCharacters.Length; i++)
        {
            Button button = btnCharacters[i];

            if (button != null)
            {
                button.onClick.AddListener (delegate
                {
                    PlayClick ();
                    SelectCharacter (playerIDs[i]);
                });
            }
        }

        if (btnSelectCharacterBack != null)
        {
            btnSelectCharacterBack.onClick.AddListener (delegate
            {
                PlayClick ();
                panelSelectCharacter.SetActive (false);
                panelNewGame.SetActive (true);
                btnNewSlots[0].Select ();
            });
        }

        // PAINEL OPCOES

        if (sliderMusicVolume != null && sliderFXVolume != null)
        {
            // Define valores dos Sliders
            sliderMusicVolume.value = audioControllerRevisao.maxVolumeMusic;
            sliderFXVolume.value = audioControllerRevisao.maxVolumeFX;

            // Atualiza valores de acordo com valor do slider

            sliderMusicVolume.onValueChanged.AddListener (delegate
            {
                float tempVolume = sliderMusicVolume.value;
                audioControllerRevisao.maxVolumeMusic = tempVolume;
                audioControllerRevisao.sourceMusic.volume = tempVolume;
                PlayerPrefs.SetFloat ("max_volume_music", tempVolume);
            });

            sliderFXVolume.onValueChanged.AddListener (delegate
            {
                float tempVolume = sliderFXVolume.value;
                audioControllerRevisao.maxVolumeFX = tempVolume;
                PlayerPrefs.SetFloat ("max_volume_fx", tempVolume);
            });
        }

        if (btnOptionsBack != null)
        {
            btnOptionsBack.onClick.AddListener (delegate
            {
                PlayClick ();
                panelOptions.SetActive (false);
                btnOptions.Select ();
            });
        }
    }

    // Seleciona o personagem para a primeira fase
    private void SelectCharacter (int id)
    {
        PlayerPrefs.SetInt ("player_id", id);
        SceneManager.LoadScene ("Load");
    }

    // Exibe nome do personagem de acordo com "PointerEnter" e "PointerExit"
    public void ShowCharacterName (string name)
    {
        textCharacterName.text = name;
    }

    // Verifica jogos salvos e habilita / desabilita slots
    public void VerifySavedGames ()
    {
        // Desabilita botoes
        btnLoadGame.interactable = false;

        foreach (Button button in btnLoadSlots)
        {
            button.interactable = false;
        }

        foreach (GameObject obj in btnDeleteSlots)
        {
            obj.SetActive (false);
        }

        // Verifica cada save e habilita botoes relacionados
        for (int i = 0; i < btnNewSlots.Length; i++)
        {
            string path = string.Concat (Application.persistentDataPath, "/playerdata", i, ".dat");

            if (File.Exists (path))
            {
                btnLoadSlots[i].interactable = true;
                btnNewSlots[i].interactable = false;
                btnDeleteSlots[i].SetActive (true);
            }
        }

        // Habilita botao de carregar jogo
        for (int i = 0; i < btnLoadSlots.Length; i++)
        {
            if (btnLoadSlots[i].interactable)
            {
                btnLoadGame.interactable = true;
                break;
            }
        }
    }

    // Salva novo slot no PlayerPrefs
    public void NewGame (int slot)
    {
        string name = "playerdata";
        name = string.Concat (name, slot, ".dat");
        PlayerPrefs.SetString ("slot", name);
    }

    // Carrega slot existente no PlayerPrefs
    public void LoadGame (int slot)
    {
        string name = "playerdata";
        name = string.Concat (name, slot, ".dat");
        PlayerPrefs.SetString ("slot", name);
        SceneManager.LoadScene ("Load");
    }

    // Deleta um slot existente com base no ID do mesmo
    public void DeleteSlot (int slot)
    {
        string path = string.Concat (Application.persistentDataPath, "/playerdata", slot, ".dat");

        if (File.Exists (path))
        {
            File.Delete (path);
        }

        VerifySavedGames ();
    }

    // Toca o efeito de clique dos botoes
    private void PlayClick ()
    {
        audioControllerRevisao.PlayFX (audioControllerRevisao.fxClick, 1f);
    }
}