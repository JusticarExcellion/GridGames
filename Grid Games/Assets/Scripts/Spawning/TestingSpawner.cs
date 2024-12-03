using UnityEngine;

public class TestingSpawner : MonoBehaviour
{
    public Directions StartingDirection;
    public bool FocusCamera;
    private TestSpawnManager GlobalSpawnManager;
    private Transform MyTransform;

    void Start()
    {
        MyTransform = this.transform;
        GlobalSpawnManager = FindObjectOfType<TestSpawnManager>();
        GlobalSpawnManager.Spawners.Add( this );
    }
}
