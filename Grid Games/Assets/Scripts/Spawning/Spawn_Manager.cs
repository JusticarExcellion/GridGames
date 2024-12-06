using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [HideInInspector] public Spawner[] Spawners;

    [Header("Player Prefab")]
    [SerializeField] private GameObject Player;

    [Header("Enemy Prefab")]
    [SerializeField] private GameObject Seeker;
    [SerializeField] private GameObject Interceptor;

    private int CurrentNumberOfSeekers;
    private int CurrentNumberOfInterceptors;

    private List<int> SeekerWaveNumbers;
    private List<int> InterceptorWaveNumbers;
    private int WaveNum;

    void Start()
    {
        WaveNum = 0;
    }

    public bool
    Initialize( in List<int> SeekersWave, in List<int> InterceptorWave)
    {
        CurrentNumberOfSeekers = 0;
        CurrentNumberOfInterceptors = 0;
        SeekerWaveNumbers = SeekersWave;
        InterceptorWaveNumbers = InterceptorWave;
        Spawners = FindObjectsOfType<Spawner>();
        if( Spawners.Length == 0 ) return false;
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
        //TODO: Pick a random spawner instead of doing this for each
        //TODO: Spawn wave of enemies
        foreach( Spawner currentSpawner in Spawners )
        {
            if( currentSpawner.ForcePlayerSpawn ) continue;

            if( CurrentNumberOfSeekers < SeekerWaveNumbers[WaveNum] )
            {
                SpawnEnemy( currentSpawner.transform.position, currentSpawner.StartingDirection, in Seeker );
                CurrentNumberOfSeekers++;
            }
            else if( CurrentNumberOfInterceptors < InterceptorWaveNumbers[WaveNum] )
            {
                SpawnEnemy( currentSpawner.transform.position, currentSpawner.StartingDirection, in Interceptor );
                CurrentNumberOfInterceptors++;
            }
            else
            {
                break;
            }
        }

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
        Transform EnemyTransform = EnemyInstance.transform;
        EnemyTransform.position = Location;

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
}
