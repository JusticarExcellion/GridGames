using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance { get; private set; }

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
    }

    public void
    InitializeLevel()
    {
        //TODO: Create the Audio Manager and get the instance
        //TODO: Create AI Manager and get the AI Manager Instance
        //TODO: Create the Spawn Manager and Spawn all the entities
        //TODO: After spawning all entities get the player instance
    }

    public void
    EndLevel()
    {
        //TODO: Destroy Everything and Show and end of level screen of the camera rotating aronud the player slowly
    }
}
