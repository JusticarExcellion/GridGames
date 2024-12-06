using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance { get; private set; }
    public bool Paused = false;
    private UIManager UIM;
    private SpawnManager SpawnerManager;
    private PlayerManager PM;
    private AIManager AIM;
    //Reference to the Player Manager

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
        Debug.Log("Level Manager Started");
        InitializeLevel();
    }

    public void
    InitializeLevel()
    {
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
    }

    public void
    RestartLevel()
    {
        //if restarting we just need to reinitalize everything again
        Debug.Log("Restarting Level...");
    }

    public void
    Pause()
    {
        Debug.Log("Game Paused");
        Time.timeScale = 0.0f;
        Paused = true;
        UIM.ShowPauseScreen();
    }

    public void
    Unpause()
    {
            Debug.Log("Game Unpaused");
            Time.timeScale = 1.0f;
            Paused = false;
            UIM.HidePauseScreen();
    }

    public void
    EndLevel( bool PlayerDestroyed )
    {
        //NOTE: Cleaning Up enemies
        List<SeekerAI> ActiveAI = AIM.GetSeekersInScene();

        //NOTE: Filling temp list
        List<SeekerAI> temp = new List<SeekerAI>();
        foreach( SeekerAI AI in ActiveAI )
        {
            temp.Add(AI);
        }

        foreach( SeekerAI AI in temp )
        {
            AI.DestroySeeker();
        }
        PM.DestroyPlayerInstance();

        if( PlayerDestroyed )
        {
            UIM.ShowFailScreen();
        }
        else
        {
            UIM.ShowSuccessScreen();
        }
    }
}
