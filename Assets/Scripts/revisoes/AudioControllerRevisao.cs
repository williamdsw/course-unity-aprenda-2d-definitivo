using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioControllerRevisao : MonoBehaviour
{
    public AudioSource sourceMusic;
    public AudioSource sourceFX;

    [Header ("Musicas")]
    public AudioClip musicTitle;
    public AudioClip musicStage1;

    [Header ("Efeitos")]
    public AudioClip fxClick;
    public AudioClip fxSword;
    public AudioClip fxAxe;
    public AudioClip fxBow;
    public AudioClip fxStaff;
    public AudioClip fxChest;
    public AudioClip fxHit;
    public AudioClip fxSpawnLoot;
    public AudioClip fxLoot;
    public AudioClip fxDoor;

    // Configuracoes dos audios
    public float maxVolumeMusic;
    public float maxVolumeFX;

    // Configuracoes da troca de musica
    private AudioClip newMusic;
    private string nextScene;
    private bool gotoNextScene;

    // ------------------- FUNCOES UNITY ------------------- //

    // Carrega as configuracoes de audio do aparelho
    private void Start () 
    {
        DontDestroyOnLoad (this.gameObject);

        // Define pre-configuracoes
        if (PlayerPrefs.GetInt ("initial-values") == 0)
        {
            PlayerPrefs.SetInt ("initial-values", 1);
            PlayerPrefs.SetInt ("max-volume-music", 1);
            PlayerPrefs.SetInt ("max-volume-fx", 1);
        }

        // Carrega configuracoes
        maxVolumeMusic = PlayerPrefs.GetInt ("max-volume-music");
        maxVolumeFX = PlayerPrefs.GetInt ("max-volume-fx");

        ChangeMusic (this.musicTitle, "Title", true);
    }

    // ------------------- FUNCOES ------------------- //

    // Toca um efeito de acordo com volume especificado
    public void PlayFX (AudioClip pFxSound, float pVolume)
    {
        float tempVolume = (pVolume > maxVolumeFX ? maxVolumeFX : pVolume);

        // Vai trocar apenas uma vez
        sourceFX.volume = tempVolume;
        sourceFX.PlayOneShot (pFxSound);
    }

    // Passa valores e chama corrotina
    public void ChangeMusic (AudioClip pNewMusic, string pNextScene, bool pGotoNextScene)
    {
        // Passa valores
        this.newMusic = pNewMusic;
        this.nextScene = pNextScene;
        this.gotoNextScene = pGotoNextScene;

        StartCoroutine ("ChangeMusicCoroutine");
    }

    // ------------------- CORROTINAS ------------------- //

    // Altera o volume e troca a musica do Player
    private IEnumerator ChangeMusicCoroutine ()
    {
        // Abaixa volume da musica
        for (float volume = maxVolumeMusic; volume >= 0; volume -= 0.1f)
        {
            yield return new WaitForSecondsRealtime (0.1f);
            sourceMusic.volume = volume;
        }

        // Troca audio
        sourceMusic.volume = 0;
        sourceMusic.clip = newMusic;
        sourceMusic.Play ();

        // Aumenta volume da musica
        for (float volume = 0; volume <= maxVolumeMusic; volume += 0.1f)
        {
            yield return new WaitForSecondsRealtime (0.1f);
            sourceMusic.volume = volume;
        }

        if (gotoNextScene)
        {
            SceneManager.LoadScene (nextScene);
        }
    }
}