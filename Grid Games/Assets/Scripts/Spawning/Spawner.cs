using UnityEngine;

public enum Directions : int
{
    North,
    West,
    East,
    South
}

public class Spawner : MonoBehaviour
{
    public Directions StartingDirection;
    public bool ForcePlayerSpawn;
    private Spawn_Manager GlobalSpawnManager;
    private Transform MyTransform;

    void Start()
    {
        MyTransform = this.transform;
        GlobalSpawnManager = FindObjectOfType<Spawn_Manager>();
        GlobalSpawnManager.Spawners.Add( this );
    }
}
