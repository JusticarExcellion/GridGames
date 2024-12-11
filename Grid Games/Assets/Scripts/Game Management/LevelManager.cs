using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public struct GameStatistics
{
    public int EnemiesDestroyed;
    public int TimesBoosted;
    public int LightTrailActivated;
    public float ActiveTrailTime;
    public int ConsumablesUsed;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public bool Paused = false;
    public bool GameOver = false;
    public int DifficultyLevel;
    public AudioMixerGroup AudioMixer;
    public AudioClip LevelMusic;

    //NOTE: Managers
    private UIManager UIM;
    private SpawnManager SpawnerManager;
    private PlayerManager PM;
    private AIManager AIM;
    private SceneChanger SC;
    private SettingsManager SM;
    private AudioManager AM;
    private ConsumableManager CM;

    [Header("Enemy Waves:")]
    [SerializeField] private List<int> SeekerList;
    [SerializeField] private List<int> InterceptorList;

    private void
    Awake()
    {
        if(Instance != null && Instance != this )
        {
            Destroy( this );
        }
        else
        {
            Instance = this;
        }
    }

    private void
    OnEnable()
    {
        SceneManager.sceneLoaded += InitializeLevel;
        Debug.Log("Level Manager Enabled!");
    }

    public void
    InitializeLevel( Scene scene, LoadSceneMode mode )
    {
        if( !SM ) SM = this.gameObject.AddComponent< SettingsManager >();
        if( SM ) DifficultyLevel = SM.GetDifficulty();
        else
        {
            Debug.LogError("ERROR: NO SETTINGS MANAGER FOUND IN SCENE!!!");
        }

        if( !AM ) AM = this.gameObject.AddComponent< AudioManager >();
        if( AM.InitializeAudio( in AudioMixer, in LevelMusic ) )
        {
            Debug.Log("Audio Manager Initialized...");
        }

        if( !SpawnerManager ) SpawnerManager = this.gameObject.AddComponent< SpawnManager >();

        if( SpawnerManager.Initialize( in SeekerList, in InterceptorList ) )
        {
            Debug.Log("Spawn Manager Initialized...");
        }

        if( SpawnerManager.SpawnPlayer() )
        {
            Debug.Log("Player Spawned...");
            if( !PM )
            {
                PM = this.gameObject.AddComponent< PlayerManager >();
            }
        }

        if( !CM ) CM = this.gameObject.AddComponent< ConsumableManager >();
        if( CM.Initialize( ) )
        {
            Debug.Log("Consumable Manager Initialized...");

            CM.GenerateAllConsumables();
        }

        if( !AIM ) AIM = this.gameObject.AddComponent< AIManager >();

        if( SpawnerManager.SpawnAllEntities() )
        {
            Debug.Log("All entities spawned");
        }

        if( !UIM ) UIM = this.gameObject.AddComponent(typeof( UIManager )) as UIManager;

        if( UIM.InitializeUI() )
        {
            Debug.Log("UI Manager Initialized...");
        }
        else
        {
            Debug.LogError("UI Failed to Initialize...");
        }

        if( !SC ) SC = this.gameObject.AddComponent< SceneChanger >();
    }

    public void
    RestartLevel()
    {
        //if restarting we just need to reinitalize everything again
        Debug.Log("Restarting Level...");
        SpawnerManager.RestartLevel();
        PlayerManager.instance.Restart();
        AIM.Restart();
        UIM.Restart();
        AM.Restart();
        GameOver = false;
    }

    public void
    Quit()
    {
        SC.LoadLevel( SceneChanger.MainMenuIndex );
        AM.CleanUpAudio();
        Destroy( this.gameObject ); // This Shouldn't cause any problems gets rid of all of the managers and this as well
    }

    public void
    Pause()
    {
        if( GameOver ) return;
        Debug.Log("Game Paused");
        Paused = true;
        AIM.Pause();
        PlayerManager.instance.Player.Pause();
        UIM.ShowPauseScreen();
        AM.PauseMusic();
        CM.Pause();
    }

    public void
    Unpause()
    {
        Debug.Log("Game Unpaused");
        Paused = false;
        AIM.Unpause();
        PlayerManager.instance.Player.Unpause();
        CM.UnPause();
        UIM.HidePauseScreen();
        AM.UpdateVolumeValues();
        AM.UnPauseMusic();
    }

    public void
    EndLevel( bool PlayerDestroyed )
    {
        if( GameOver ) return; // Prevents the game from ending twice
        GameStatistics gameStats;
        PM.Player.FillStatistics( out gameStats );
        gameStats.EnemiesDestroyed = AIM.EnemiesDestroyed;
        AIM.EndLevel();

        if( PlayerDestroyed )
        {
            UIM.ShowFailScreen( gameStats );
        }
        else
        {
            UIM.ShowSuccessScreen( gameStats );
        }

        PM.DestroyPlayerInstance();
        GameOver = true;
    }

    private void
    OnDisable()
    {
        SceneManager.sceneLoaded -= InitializeLevel;
    }
}
