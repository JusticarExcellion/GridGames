using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{

    [HideInInspector] public List<Spawner> Spawners;
    [SerializeField] private int NumberOfEnemies;
    private int CurrentNumberOfEnemies;
    [Header("Player Prefab")]
    [SerializeField] private GameObject Player;
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject Enemy;
    private int SpawnNumber; //NOTE: Spawn Number starts at 1, though it is initialized to 0 at first usage it's value will be 1

    void Start()
    {
        SpawnNumber = 0;
        CurrentNumberOfEnemies = 0;
        foreach( Spawner currentSpawner in Spawners )
        {
            if( currentSpawner.ForcePlayerSpawn )
            {
                SpawnPlayer( currentSpawner.transform.position, currentSpawner.StartingDirection );
            }
            else
            {
                if( CurrentNumberOfEnemies < NumberOfEnemies )
                {
                    SpawnEnemy( currentSpawner.transform.position, currentSpawner.StartingDirection );
                    CurrentNumberOfEnemies++;
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void
    SpawnPlayer( Vector3 Location, Directions StartingDirection )
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
    SpawnEnemy( Vector3 Location, Directions StartingDirection )
    {

        GameObject EnemyInstance = Instantiate( Enemy );
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

    public int
    GetSpawnNumber()
    {
        SpawnNumber+=1;
        return SpawnNumber;
    }

}
