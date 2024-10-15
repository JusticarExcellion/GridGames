using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{

    [HideInInspector] public List<Spawner> Spawners;
    [Header("Player Prefab")]
    [SerializeField] private GameObject Player;
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject Enemy;

    void Start()
    {
        foreach( Spawner currentSpawner in Spawners )
        {
            if( currentSpawner.ForcePlayerSpawn )
            {
                SpawnPlayer( currentSpawner.transform.position, currentSpawner.StartingDirection );
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

}
