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
    private Transform MyTransform;

    void Start()
    {
        MyTransform = this.transform;
    }
}
