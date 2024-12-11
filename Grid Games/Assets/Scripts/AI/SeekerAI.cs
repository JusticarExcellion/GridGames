using UnityEngine;

public class SeekerAI : MonoBehaviour
{
    private Transform MyTransform;
    private Kinematic CharacterKinematic;
    private Kinematic TargetKinematic;
    private Steering Input;

    [Header("Character Parameters:")]
    public EnemyType type;
    [SerializeField] private int MaxSpeed;
    [SerializeField] private int AttackSpeed;
    [SerializeField] private int Health = 1;
    private int CurrentHealth;

    [Header("AI Parameters:")]
    [SerializeField] private int SlowRadius;
    [SerializeField] private int TargetRadius;
    [SerializeField] private float TimeToTarget;
    [SerializeField] private int AttackLookAhead;
    [SerializeField] private int AggressionDistance;
    [SerializeField] private Vector2 Offset;
    [SerializeField] private int FleeTime;
    [SerializeField] private int VulnerableDuration;
    private readonly int MaxMatchTime = 15;

    [Range(1,5)]
    [SerializeField] private float AttackTime;
    [HideInInspector] public float TimeToAttack;
    public bool Attacking;
    public AIState CurrentState;

    private float CurrentSpeed;

    [Range(0,3)]
    [SerializeField] private float InvincibilityTimer = 2;
    private float CurrentInvincibilityTimer = 0;
    private float StateTimer;

    private bool Dead = false;
    private bool Paused = false;
    private float DeathTimer;
    private float CurrentDeathTimer;

    private AudioClip Ambience;

    [Header("Target Overrides")]
    public TestingObject TestTarget;

    [Header("Connected Components:")]
    [SerializeField] private LightTrailCollisions LightTrail;
    [SerializeField] private GameObject SeekerModel;
    [SerializeField] private AudioSource SeekerAudio;
    [SerializeField] private AudioSource SpecialAudio;
    private AIManager Overlord;
    private SeekerAI Self;

    [Header("Debug")]
    [SerializeField] private bool DebugInfo;
    [SerializeField] private float VelocityRayLength;


    private void
    Start()
    {
        Self = this.gameObject.GetComponent<SeekerAI>();
        MyTransform = this.transform;
        CurrentState = AIState.Standby;
        CharacterKinematic = new Kinematic();
        CharacterKinematic.Transform = MyTransform;
        CurrentSpeed = MaxSpeed;
        Overlord = AIManager.Instance; //NOTE: Getting the AI Manager
        Overlord.AddSeeker( in Self );
        CurrentHealth = Health;
        DeathTimer = LightTrail.TrailTime;
        CurrentDeathTimer = 0;
        StateTimer = 0;
        AudioManager.Instance.GetAudioClipToPlay( type, out Ambience );
        if( !Ambience )
        {
            Debug.Log("Ambience faild to load");
        }
        SeekerAudio.clip = Ambience;
        float volume = AudioManager.Instance.GetSoundEffectVolume();
        SeekerAudio.loop = true;
        SeekerAudio.volume = volume;
        SeekerAudio.Play();
    }

    private void
    Update()
    {
        if( Paused ) return;
        float deltaTime = Time.deltaTime;
        if(!Dead)
        {
            SeekerStateUpdate();

            if( CurrentInvincibilityTimer > 0 )
            {
                CurrentInvincibilityTimer -= deltaTime;
            }
        }


        if( CurrentDeathTimer > 0 ) //NOTE: This is where we wait to destroy the Object to let the light trail completely die if it was on before destroying the object
        {
            CurrentDeathTimer -= deltaTime;
        }
        else if( CurrentDeathTimer < 0 )
        {
            Destroy( this.gameObject );
        }

        if( StateTimer > 0 )
        {
            StateTimer -= deltaTime;
        }
        else if( StateTimer < 0)
        {
            CurrentState = AIState.Chase;
            StateTimer = 0;
            LightTrail.StopEmitting();
            //NOTE: We remove the attacker after they flee
        }

        CharacterKinematic.Rotation = MyTransform.rotation.eulerAngles.y;
    }

    private void
    FixedUpdate()
    {
        if( Paused ) return;
        if(!Dead)
        {
            Movement();
            Rotation();
        }

        if( DebugInfo ) DebugMovement();
    }

    public void
    SetHealth( int MaxHealth )
    {
        Health = MaxHealth;
        CurrentHealth = Health;
    }

    private void
    SeekerStateUpdate()
    {
        if( TargetKinematic == null ) //NOTE: If we have no target then we do nothing
        {
            CurrentState = AIState.Standby;
            TargetKinematic = Overlord.GetTarget( in Self );
            return;
        }

        Vector3 ToTarget = TargetKinematic.Transform.position - CharacterKinematic.Transform.position;
        //TODO: AI should make behavioral decisions here that should be determined by the AI manager feeding it target information, then the unit makes a decision based on the target information
        //TODO: Turn this into a switch statemnet with delegated Behavioors for each state
        switch( CurrentState )
        {
            case AIState.Seek:
                Input = AIResources.Seek( in TargetKinematic, in CharacterKinematic );
                break;
            case AIState.Chase:

                if( ToTarget.magnitude < AggressionDistance )
                {
                    bool ShouldAttack = Overlord.AttackingDecision( Self );
                    if( ShouldAttack )
                    {
                        CurrentState = AIState.Match;
                        StateTimer = MaxMatchTime;
                    }
                    else  //NOTE: If the attack spots are filled up then we need to move away
                    {
                        CurrentState = AIState.Vulnerable;
                    }
                }

                Input = AIResources.Chase( in TargetKinematic, in CharacterKinematic );
                break;

            case AIState.Match:

                if( StateTimer == 0 )
                {
                    CurrentState = AIState.Attack;
                }

                Vector3 TargetOffset = new Vector3( Offset.x, 0, Offset.y );
                Vector3 FromTargetToMyPosition = MyTransform.position - TargetKinematic.Transform.position;


                FromTargetToMyPosition.Normalize();
                float dotProduct = Vector3.Dot( TargetKinematic.Transform.right, FromTargetToMyPosition );

                if( dotProduct < 0 )
                {
                    TargetOffset.x *= -1;
                }


                Vector3 TargetPosition = TargetKinematic.Transform.TransformPoint( TargetOffset );

                Vector3 FromMeToOffset = TargetPosition - MyTransform.position;
                if( FromMeToOffset.magnitude <= TargetRadius && !Attacking )
                {
                    //TODO: We need to know if their is another enemy that is about to attack and AI Manager will control the order of who attack's first
                    TimeToAttack = 0;
                    Attacking = true;
                }

                if( Attacking )
                {
                    if( TimeToAttack < AttackTime ) // Once we reach our target radius we will start the timer to attack the player
                    {
                        TimeToAttack += Time.deltaTime;
                        //Debug.Log( "Time to Attack: " + TimeToAttack );
                    }
                    else // AI ends their timer and is now attacking the target
                    {
                        CurrentState = AIState.Attack;
                        Debug.Log("Attacking...");
                        CurrentSpeed = MaxSpeed;
                        AudioManager.Instance.PlaySpecialEffect( in SpecialAudio, SpecialEffect.AttackSound );
                        LightTrail.StartEmitting();
                    }
                }

                Input = AIResources.KinematicArrive( TargetPosition, in CharacterKinematic, (int)CurrentSpeed,  SlowRadius, TargetRadius );
                break;

            case AIState.Attack:
                ToTarget += TargetKinematic.Transform.forward * AttackLookAhead;

                if( ToTarget.magnitude < TargetRadius )
                {
                    CurrentState = AIState.Vulnerable;
                    StateTimer = VulnerableDuration;
                    ResetCharacterParameters();
                }

                Input = AIResources.Attack( in TargetKinematic, in CharacterKinematic, AttackLookAhead );
                break;

            case AIState.Flee:

                if( StateTimer == 0 )
                {
                    CurrentState = AIState.Chase;
                    LightTrail.StopEmitting();
                }

                Input = AIResources.Flee( in TargetKinematic, in CharacterKinematic );
                break;

            case AIState.Vulnerable:

                if( StateTimer == 0 )
                {
                    CurrentState = AIState.Flee;
                    StateTimer = FleeTime;
                    LightTrail.StopEmitting();
                    AIManager.Instance.RemoveAttackSeeker( in Self );
                }

                //NOTE: Just continue straight for a little bit
                Input = AIResources.Vulnerable( in TargetKinematic, in CharacterKinematic );
                break;
        }

    }

    private void
    Movement()
    {
        Vector3 NewVelocity = Input.Velocity;

        if( CurrentState != AIState.Match )
        {
            NewVelocity.Normalize();
            NewVelocity *= CurrentSpeed;
        }

        NewVelocity.y = 0;

        CharacterKinematic.Velocity = NewVelocity;
        MyTransform.position += NewVelocity * Time.deltaTime;
        //Debug.Log("Moving: " + NewVelocity );
    }

    private void
    Rotation()
    {
        MyTransform.rotation = Quaternion.Slerp( MyTransform.rotation, Input.Rotation, 0.5f );
    }

    private void
    DebugMovement()
    {
        Vector3 NewPosition = Input.Velocity;
        NewPosition *= VelocityRayLength;

        Debug.DrawRay( MyTransform.position, NewPosition , Color.red );
    }

    private void
    MoveTowardsFollowOffset()
    {
        //TODO: Get the Vector 2 offset, apply the adjustments on the x and z axes onto the targets transform positions, find the closest offset based on the dot product, then run kinematic arrive on that position
    }

    //TODO: Local Avoidance
    private int
    CollisionDetection()
    {
        return 0;
    }

    private void
    ResetCharacterParameters()
    {
        CurrentSpeed = MaxSpeed;
        Attacking = false;
    }

    public void
    DamageHealth()
    {
        if( CurrentInvincibilityTimer <= 0 )
        {
            CurrentHealth--;
            AudioManager.Instance.PlaySpecialEffect( in SpecialAudio, SpecialEffect.EnemyDamaged );
            Debug.Log(this.gameObject.name + ": Damaged!");
            CurrentInvincibilityTimer = InvincibilityTimer;
        }

        if( CurrentHealth < 1 && !Dead )
        {
            Dead = true;
            Debug.Log( this.gameObject.name + ": Destroyed!" );
            DestroySeeker();
        }
    }

    public void
    DestroySeeker()
    {
        SeekerAudio.Stop();
        AudioManager.Instance.PlaySpecialEffect( in SpecialAudio, SpecialEffect.Destroyed );
        Overlord.RemoveSeeker( in Self );
        CurrentDeathTimer = LightTrail.TrailTime;
        LightTrail.StopEmitting();
        Destroy( SeekerModel );
    }

    private void
    OnDestroy()
    {
        SeekerAudio.Stop();
    }

    public void
    Pause()
    {
        Paused = true;
        LightTrail.Pause();
        SeekerAudio.Stop();
    }

    public void
    Unpause()
    {
        Paused = false;
        LightTrail.UnPause();
        float volume = AudioManager.Instance.GetSoundEffectVolume();
        SeekerAudio.volume = volume;
        SeekerAudio.Play();
    }
}

