using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class PlayerManager : MonoBehaviour
{
    //TODO: This will manage the player's current state and be responsible for reacting to player input and taking the appropriate action depending on the context
    public static PlayerManager instance;
    public bool Playing = false; //NOTE: Used for when we are switching the state of the game, i.e. menu, pausing, loading, etc.

    void Start() //NOTE: Singleton
    {
        if( instance == null )
        {
            instance = this;
        }
        else if( instance != this )
        {
            Destroy( this );
        }
        DontDestroyOnLoad( this );
        Playing = true;
    }

    void Update()
    {
        if( Playing )
        {
        }
    }

}
