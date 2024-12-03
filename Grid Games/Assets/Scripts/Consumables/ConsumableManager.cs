using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    [SerializeField] private Transform ConsumableSpawnPoints;
    [SerializeField] private List<GameObject> ConsumablesPrefab;

    [Header("Projectile Prefabs:")]
    public GameObject MissileProjectile;

    public List<GameObject> ConsumablesInScene { get; private set; }
    private int lastSelected;


    private void
    Start()
    {
        ConsumablesInScene = new List<GameObject>();
        GenerateConsumables();
    }

    private void
    GenerateConsumables()
    {
        foreach( Transform child in ConsumableSpawnPoints )
        {
            int selection = Random.Range(0, ConsumablesPrefab.Count - 1);
            if( selection == lastSelected )
            {
                selection--;
                if( selection < 0 )
                {
                    selection = ConsumablesPrefab.Count - 1;
                }
            }

            GameObject NewConsumable = ConsumablesPrefab[ selection ];

            GameObject go = Instantiate( NewConsumable, child.position, child.rotation );
            Vector3 NewPosition = go.transform.position;
            NewPosition.y ++;
            go.transform.position = NewPosition;

            ConsumablesInScene.Add( go );
            lastSelected = selection;
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
