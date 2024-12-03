using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnManager : MonoBehaviour
{

    [HideInInspector] public List<TestingSpawner> Spawners;
    [SerializeField] private int NumberOfEnemies;
    private int CurrentNumberOfEnemies;
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject Enemy;
    private int SpawnNumber; //NOTE: Spawn Number starts at 1, though it is initialized to 0 at first usage it's value will be 1

    void Start()
    {
        SpawnNumber = 0;
        CurrentNumberOfEnemies = 0;
        foreach( TestingSpawner currentSpawner in Spawners )
        {
            if( CurrentNumberOfEnemies < NumberOfEnemies )
            {
                SpawnEnemy( currentSpawner.transform.position, currentSpawner.StartingDirection, currentSpawner.FocusCamera );
                CurrentNumberOfEnemies++;
            }
            else
            {
                break;
            }
        }
    }

    private void
    SpawnEnemy( Vector3 Location, Directions StartingDirection, bool FocusCamera )
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

        if( FocusCamera )
        {
            //We need to activate a virtual camera and follow the motorcycle around
            TestingAI EnemyAI = EnemyInstance.GetComponent<TestingAI>();
            EnemyAI.FocusCamera();
        }

        //TODO: Add AI to current AI's in the scene
    }

    public int
    GetSpawnNumber()
    {
        SpawnNumber+=1;
        return SpawnNumber;
    }

}
