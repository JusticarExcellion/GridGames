using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [HideInInspector] public Spawner[] Spawners;

    public static SpawnManager Instance;
    private GameObject Player;
    private GameObject Seeker;
    private GameObject Interceptor;
    private GameObject SpawnSphere;

    private List<int> SeekerWaveNumbers;
    private List<int> InterceptorWaveNumbers;
    public int WaveNum;
    public int TotalWaves;
    public int TotalNumberOfEnemies;

    private void
    Awake()
    {
        if(Instance != null && Instance != this )
        {
            Destroy( this );
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad( this );
    }

    public bool
    Initialize( in List<int> SeekersWave, in List<int> InterceptorWave)
    {
        SeekerWaveNumbers = SeekersWave;
        InterceptorWaveNumbers = InterceptorWave;
        Spawners = FindObjectsOfType<Spawner>();

        if( Spawners.Length == 0 ) return false;

        TotalNumberOfEnemies = 0;
        foreach( int WaveEnemies in SeekerWaveNumbers )
        {
            TotalNumberOfEnemies += WaveEnemies;
        }

        foreach( int WaveEnemies in InterceptorWaveNumbers )
        {
            TotalNumberOfEnemies += WaveEnemies;
        }

        TotalWaves = SeekerWaveNumbers.Count;
        WaveNum = 0;

        //Loading Prefabs
        Player = Resources.Load("Player_Cycle") as GameObject;
        Seeker = Resources.Load("Seeker") as GameObject;
        Interceptor = Resources.Load("Interceptor") as GameObject;
        SpawnSphere = Resources.Load("SpawnSphere") as GameObject;

        if( !Player )
        {
            Debug.LogError("No Player Prefab Found in Resources Folder");
            return false;
        }

        if( !Seeker )
        {
            Debug.LogError("No Seeker Prefab Found in Resources Folder");
            return false;
        }

        if( !Interceptor )
        {
            Debug.LogError("No Interceptor Prefab Found in Resources Folder");
            return false;
        }

        if( !SpawnSphere )
        {
            Debug.LogError("No SpawnSphere Prefab Found in Resources Folder");
            return false;
        }


        return true;
    }

    public bool
    SpawnPlayer()
    {
        bool playerSpawned = false;
        foreach( Spawner currentSpawner in Spawners )
        {
            if( currentSpawner.ForcePlayerSpawn )
            {
                SpawnPlayerInstance( currentSpawner.transform.position, currentSpawner.StartingDirection );
                playerSpawned = true;
                break;
            }
        }

        if( !playerSpawned )
        {
            Debug.Log("No Player Spawn Located!");
            return false;
        }

        return true;
    }

    public bool
    SpawnAllEntities()
    {
        if( WaveNum > TotalWaves - 1 )
        { //NOTE: End level if we hit the last wave
            return false;
        }

        int CurrentNumberOfSeekers = 0;
        int CurrentNumberOfInterceptors = 0;
        int CurrentWaveSeekers = SeekerWaveNumbers[WaveNum];
        int CurrentWaveInterceptors = InterceptorWaveNumbers[WaveNum];
        int totalEnemiesInWave = CurrentWaveSeekers + CurrentWaveInterceptors;


        //NOTE: Filling temporary list
        List< Spawner > temp = new List< Spawner >();
        foreach( Spawner spawn in Spawners )
        {
            temp.Add( spawn );
        }

        for( int i = 0; i < totalEnemiesInWave; i++)
        {
            int randomSpawner = Random.Range( 0, temp.Count - 1  );
            Spawner currentSpawner = temp[ randomSpawner ];
            if( currentSpawner.ForcePlayerSpawn )
            {
                i--;
                continue;
            }

            if( CurrentNumberOfSeekers < SeekerWaveNumbers[WaveNum] )
            {
                SpawnEnemy( currentSpawner.transform.position, currentSpawner.StartingDirection, in Seeker );
                Debug.Log("Spawn Seeker At: " + currentSpawner.transform.position );
                CurrentNumberOfSeekers++;
            }
            else if( CurrentNumberOfInterceptors < InterceptorWaveNumbers[WaveNum] )
            {
                SpawnEnemy( currentSpawner.transform.position, currentSpawner.StartingDirection, in Interceptor );
                Debug.Log("Spawn Interceptor At: " + currentSpawner.transform.position );
                CurrentNumberOfInterceptors++;
            }

            temp.Remove( currentSpawner );
        }

        temp = null;

        WaveNum++;

        return true;
    }

    private void
    SpawnPlayerInstance( Vector3 Location, Directions StartingDirection )
    {
        GameObject PlayerInstance = Instantiate( Player );
        Transform PlayerTransform = PlayerInstance.transform;
        PlayerTransform.position = Location;

        switch( StartingDirection )
        {
            case Directions.North:
                break;
            case Directions.West:
                PlayerTransform.Rotate(0.0f, 270.0f, 0.0f);
                break;
            case Directions.East:
                PlayerTransform.Rotate(0.0f, 90.0f, 0.0f);
                break;
            case Directions.South:
                PlayerTransform.Rotate(0.0f, 180.0f, 0.0f);
                break;
        }

        Camera.main.transform.position = Location;
    }

    private void
    SpawnEnemy( Vector3 Location, Directions StartingDirection, in GameObject EnemyPrefab )
    {

        GameObject EnemyInstance = Instantiate( EnemyPrefab );
        GameObject SpawnSphereInstance = Instantiate( SpawnSphere );
        Transform EnemyTransform = EnemyInstance.transform;
        Transform SpawnSphereTransform = SpawnSphereInstance.transform;
        EnemyTransform.position = Location;
        SpawnSphereTransform.position = Location;

        switch( StartingDirection )
        {
            case Directions.North:
                break;
            case Directions.West:
                EnemyTransform.Rotate(0.0f, 270.0f, 0.0f);
                break;
            case Directions.East:
                EnemyTransform.Rotate(0.0f, 90.0f, 0.0f);
                break;
            case Directions.South:
                EnemyTransform.Rotate(0.0f, 180.0f, 0.0f);
                break;
        }

    }

    private void
    CleanUpLevel()
    {
        //NOTE: Cleaning up Seekers that were being destroyed as the game ended
        SeekerAI[] SeekersStillAlive = FindObjectsOfType< SeekerAI >();
    }

    public void
    RestartLevel()
    {
        WaveNum = 0;
        CleanUpLevel();
        SpawnPlayer();
        SpawnAllEntities();
    }

    public int
    EnemiesThisWave()
    {
        int Result = SeekerWaveNumbers[ WaveNum - 1 ];
        Result += InterceptorWaveNumbers[ WaveNum - 1 ];
        return Result;
    }
}
