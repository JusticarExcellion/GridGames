using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public static ConsumableManager Instance;
    private Transform ConsumableSpawnPoints;
    private GameObject HealthPrefab;
    private List< ConsumableSpawner > Spawners;
    public List< GameObject > ConsumablesInScene;

    //TODO: Get all of the consumable Spawners, spawn all of the consumables then hand the spawners a reference to the object. Once it becomes null, spawners will start a timer before respawning the consumable once the timer goes off spawner will ask consumable manager to spawn a new consumable at it's coordinates.

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

        Spawners = new List< ConsumableSpawner >();
        ConsumablesInScene = new List< GameObject >();
    }

    public bool
    Initialize(  )
    {
        //TODO: Get the consumable prefab, provide the audio clip to the consumable
        HealthPrefab = Resources.Load("Consumables/HealthConsumable") as GameObject;
        if( !HealthPrefab )
        {
            Debug.LogError("NO PREFAB IN RESOURCES: HEALTH CONSUMABLE");
            return false;
        }

        ConsumableSpawner[] ConsumableSpawnPoints = FindObjectsOfType< ConsumableSpawner >();

        foreach( ConsumableSpawner SpawnPoint in ConsumableSpawnPoints )
        {
            ConsumableSpawner CS = SpawnPoint.GetComponent< ConsumableSpawner >();
            if( !CS ) return false;
            Spawners.Add( CS );
        }

        return true;
    }

    public void
    GenerateAllConsumables()
    {
        foreach( ConsumableSpawner Spawner in Spawners )
        {
           GameObject go = Instantiate( HealthPrefab );
           go.transform.position = Spawner.transform.position;
           Consumable goConsumableScript = go.GetComponent< Consumable >();
           Spawner.NewConsumableSpawn( in goConsumableScript );
           AudioManager.Instance.PlayConsumableAudio( in goConsumableScript.Audio );
           ConsumablesInScene.Add( go );
           //Debug.Log("New Consumable Spawned at: " + Spawner.transform.position );
        }
    }

    public void
    GenerateConsumable( in ConsumableSpawner Spawner )
    {
        GameObject go = Instantiate( HealthPrefab );
        go.transform.position = Spawner.transform.position;
        Consumable goConsumableScript = go.GetComponent< Consumable >();
        Spawner.NewConsumableSpawn( in goConsumableScript );
        AudioManager.Instance.PlayConsumableAudio( in goConsumableScript.Audio );
        ConsumablesInScene.Add( go );
        Debug.Log("New Consumable Spawned at: " + Spawner.transform.position );
    }

    public void
    Pause()
    {
        foreach( GameObject go in ConsumablesInScene )
        {
            Consumable goConsumableScript = go.GetComponent< Consumable >();
            goConsumableScript.Pause();
        }

        foreach( ConsumableSpawner Spawner in Spawners )
        {
            Spawner.Pause();
        }

    }

    public void
    UnPause()
    {
        foreach( GameObject go in ConsumablesInScene )
        {
            Consumable goConsumableScript = go.GetComponent< Consumable >();
            goConsumableScript.UnPause();
        }

        foreach( ConsumableSpawner Spawner in Spawners )
        {
            Spawner.UnPause();
        }
    }

    public void
    RemoveSelfFromConsumablesList( GameObject ObjectToDelete )
    {
        ConsumablesInScene.Remove( ObjectToDelete );
        print("Consumable Removed From List");
    }

    public List<GameObject>
    GetConsumables()
    {
        return ConsumablesInScene;
    }
}
