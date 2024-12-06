using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class PlayerManager : MonoBehaviour
{
    //TODO: This will manage the player's current state and be responsible for reacting to player input and taking the appropriate action depending on the context
    public static PlayerManager instance;
    public CycleBehavior Player; //NOTE: When the player is spawned it subscribes itself to be the main player instance this sholud be set to null at the end of the level
    public bool PlayerDestroyed {get; private set;}

    void
    Awake() //NOTE: Singleton
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
    }

    private void
    Update()
    {
        if( !Player && !PlayerDestroyed)
        {
            Player = FindObjectOfType<CycleBehavior>();
        }
    }

    public CycleBehavior
    GetPlayer()
    {
        if( PlayerDestroyed ) return null;
        else return Player;
    }

    public void
    DestroyedPlayer()
    {
        PlayerDestroyed = true;
        LevelManager.Instance.EndLevel( true );
    }

    public void
    DestroyPlayerInstance()
    {
        Destroy( Player.Cycle_Rigidbody.gameObject );
        Destroy( Player.COG_Rigidbody.gameObject );
        Destroy( Player.gameObject );
    }

}
