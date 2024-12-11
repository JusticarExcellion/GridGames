using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    private AudioSource MenuMusic;
    private SettingsManager Settings;

    [Header("Main Menu Music")]
    [SerializeField] AudioClip Music;


    void Start()
    {
        MenuMusic = this.GetComponent<AudioSource>();
        Settings = FindObjectOfType<SettingsManager>();
        MenuMusic.clip = Music;
        MenuMusic.loop = true;
        MenuMusic.Play();
        UpdateAudioSource();
    }

    public void
    UpdateAudioSource()
    {
        MenuMusic.Pause();
        //TODO: Get the audio values from memory and set the values here
        int MasterAudioLevel = Settings.GetAudio( AudioChannelType.Master );
        int MusicAudioLevel = Settings.GetAudio( AudioChannelType.Music );

        float Volume = ( MasterAudioLevel / 100f );
        Volume *= ( MusicAudioLevel / 100f );
        Debug.Log( "Music Audio Level " );
        Debug.Log("Menu Volume set to: " + Volume );
        MenuMusic.volume = Volume;
        MenuMusic.UnPause();
    }
}
