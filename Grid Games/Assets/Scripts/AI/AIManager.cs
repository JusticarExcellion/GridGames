using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WaypointTracker
{
    public int Number;
    public Transform Transform;
}

public class AIManager : MonoBehaviour
{
    private int[] SeekerHealth = { 1, 1, 2 };
    private int[] InterceptorHealth = { 2, 3, 4 };
    [SerializeField] private Transform WaypointParent;
    private List<Transform> Waypoints;
    private List<SeekerAI> Seekers;
    private SeekerAI[] AttackingSeekers;
    private CycleBehavior Player;
    private bool PlayerDestroyed;
    private bool Paused;
    public int EnemiesDestroyed;
    public int EnemiesDestroyedThisWave;
    public int EnemiesThisWave;
    int DifficultyLevel;

    [Header("AI Testing")]
    public TestingObject TestTarget;

    public static AIManager Instance;

    void
    Awake()
    {
        if(Instance != null && Instance != this )
        {
            Destroy( this );
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad( this );
        AttackingSeekers = new SeekerAI[2];
        Seekers = new List<SeekerAI>();
        EnemiesDestroyed = 0;
        Paused = false;
    }

    void Start()
    {
        DifficultyLevel = LevelManager.Instance.DifficultyLevel;
        EnemiesThisWave = SpawnManager.Instance.EnemiesThisWave();
        if( WaypointParent )
        {
            Waypoints = new List<Transform>();
            foreach( Transform child in WaypointParent ) //Getting all of the waypoints on the map
            {
                Waypoints.Add( child );
            }
        }

    }

    void
    Update()
    {
        if( Paused ) return;

        if( !Player )
        {
            if( PlayerManager.instance ) 
            {
                Player = PlayerManager.instance.GetPlayer();
                if( Player == null )
                {
                    //NOTE: Check to see if player is destroyed
                    PlayerDestroyed = PlayerManager.instance.PlayerDestroyed;
                }
            }
        }

        //TODO: If there are no more seekers and there are no more waves left to spawn then show the success screen

        //NOTE: We give orders to all of the AI based on their personal information
    }

    public WaypointTracker
    GetNextWaypoint( in WaypointTracker WT )
    {
        WaypointTracker Result = new WaypointTracker();
        if( WT.Number >= Waypoints.Count - 1)
        {
            Result.Number = 0;
            Result.Transform = Waypoints[0];
        }
        else
        {
            int NewWaypointNum = WT.Number + 1;
            Result.Number = NewWaypointNum;
            Result.Transform = Waypoints[ NewWaypointNum ];
        }
            return Result;
    }

    public WaypointTracker
    StartWaypointTracking( Vector3 Position )
    {
        WaypointTracker Result = new WaypointTracker();
        int ShortestDistance = 10000;
        int ShortestTransformIndex = 0;

        for( int i = 0; i < Waypoints.Count - 1; i++ )
        {
            Transform waypoint = Waypoints[i];
            Vector3 ToWaypoint = waypoint.position - Position;
            if( ToWaypoint.magnitude < ShortestDistance )
            {
                ShortestDistance = (int)ToWaypoint.magnitude;
                ShortestTransformIndex = i;
            }
        }

        Result.Number = ShortestTransformIndex;
        Result.Transform = Waypoints[ ShortestTransformIndex ];

        return Result;
    }

    public List<Transform>
    GetAllWaypoints()
    {
        return Waypoints;
    }

    public void
    AddSeeker( in SeekerAI Seeker )
    {
        Seekers.Add( Seeker );
        switch( Seeker.type )
        {
            case EnemyType.Seeker:
                Seeker.SetHealth( SeekerHealth[DifficultyLevel] );
                break;
            case EnemyType.Interceptor:
                Seeker.SetHealth( InterceptorHealth[DifficultyLevel] );
                break;
        }
    }

    public void
    RemoveSeeker( in SeekerAI Seeker )
    {
        Debug.Log("Removed Seeker from AI Manager List: " + Seeker.gameObject );

        if( !Seekers.Remove( Seeker ) ) return;
        for(int i = 0; i < AttackingSeekers.Length; i++ )
        {
            if( AttackingSeekers[i] == Seeker )
            {
                AttackingSeekers[i] = null;
            }
        }

        EnemiesDestroyedThisWave++;
        EnemiesDestroyed++;
        if( EnemiesDestroyedThisWave >= EnemiesThisWave )
        {
            if( SpawnManager.Instance.SpawnAllEntities() )
            {
                EnemiesThisWave = SpawnManager.Instance.EnemiesThisWave();
                EnemiesDestroyedThisWave = 0;
            }
            else
            {
                LevelManager.Instance.EndLevel( false );
            }
        }
    }

    public List<SeekerAI>
    GetSeekersInScene()
    {
        return Seekers;
    }

    public bool
    AttackingDecision( in SeekerAI Questioner )
    {
        if( PlayerDestroyed ) return false; //NOTE: If player is destroyed do not attack

        for(int i = 0; i < AttackingSeekers.Length; i++ )
        {
            if( !AttackingSeekers[i] )
            {
                Debug.Log("Overlord Commands you to attack player");
                AttackingSeekers[i] = Questioner;
                return true;
            }
        }
        return false;
    }

    public Kinematic
    GetTarget( in SeekerAI Seeker )
    {
        if( !Player )
        {
            if( TestTarget )
            {
                Seeker.CurrentState = AIState.Chase;
                return TestTarget.MyKinematic;
            }
            else
            {
                return null;
            }
        }

        Seeker.CurrentState = AIState.Chase;
        return Player.CycleKinematic;
    }

    public SeekerAI[]
    GetAttackingSeekers()
    {
        return AttackingSeekers;
    }

    public void
    RemoveAttackSeeker( in SeekerAI AI )
    {
        for( int i = 0; i < AttackingSeekers.Length; i++ )
        {
            if( AI == AttackingSeekers[ i ] )
            {
                AttackingSeekers[ i ] = null;
            }
        }
    }

    public void
    EndLevel()
    {
        SeekerAI CurrentSeeker = null;
        for(int i = 0; i < AttackingSeekers.Length; i++ )
        {
            AttackingSeekers[i] = null;
        }

        while( Seekers.Count != 0 )
        {
            CurrentSeeker = Seekers[0];
            Seekers.Remove( CurrentSeeker );
            Destroy( CurrentSeeker.gameObject );
        }
    }

    public void
    Restart()
    {
        PlayerDestroyed = false ;
        EnemiesDestroyed = 0;
        EnemiesDestroyedThisWave = 0;
        EnemiesThisWave = SpawnManager.Instance.EnemiesThisWave();
    }

    public void
    Pause()
    {
        foreach( SeekerAI AI in Seekers )
        {
            AI.Pause();
        }
        Paused = true;
    }

    public void
    Unpause()
    {
        foreach( SeekerAI AI in Seekers )
        {
            AI.Unpause();
        }
        Paused = false;
    }

}
