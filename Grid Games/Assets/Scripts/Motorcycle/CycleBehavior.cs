using UnityEngine;


public class CycleBehavior : MonoBehaviour
{
    private InputHandler UserInput;

    [Header("Character Parameters")]
    private int[] PlayerHealthLevels = { 4, 3 , 2 };
    private int MaxHealth;
    [SerializeField] private int health;

    [SerializeField] private Transform ProjectileSpawn;
    private static readonly int InvincibilityTime = 2;
    private float InvincibilityTimer;
    [HideInInspector] public ConsumableType CurrentConsumable;

    [Header("Boost Parameters")]
    [Range(0,5)]
    [SerializeField] private float BoostRechargeTimer = 1f;
    private float CurrentBoostRechargeTimer = 0;
    [Range(0,5)]
    [SerializeField] private float BoostTimer = 2.5f;
    private float CurrentBoostTimer = 0;
    private bool CanBoost = true;
    private bool Boost = false;
    private bool Paused = false;

    [Header("Motorcycle Paramters")]
    [SerializeField] private float MaxSpeed = 45;
    [SerializeField] private float BoostSpeed = 80;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float SteerStrength;
    [SerializeField] private float CycleTiltIncrement = .15f;
    [SerializeField] private float zTiltAngle = 45f;
    private float CurrentSpeed;

    [Range(0,1)]
    public float BrakingPower;

    private float RayLength;
    private RaycastHit hit;
    public LayerMask DrivableSurface;

    [SerializeField] private float GravityForce;
    private float CurrentVelocityOffset;

    [HideInInspector] public Vector3 Velocity;
    public Kinematic CycleKinematic;

    [Header("Connected Components")]
    public Rigidbody COG_Rigidbody, Cycle_Rigidbody;

    private Transform MyTransform;
    public LightTrailCollisions LightTrail;
    public PlayerManager PM;

    public AudioSource EngineAudio;
    public AudioSource SpecialAudio;
    public AudioClip CurrentAudioClip;

    private EngineStates CurrentState;
    private EngineStates PreviousState;

    //NOTE: Cycle Statistics
    private int TimesBoosted;
    private int LightTrailActivated;
    private int ConsumablesUsed;

    void Start()
    {
        COG_Rigidbody.transform.parent = null;
        Cycle_Rigidbody.transform.parent = null;
        CycleKinematic = new Kinematic();


        UserInput = FindObjectOfType<InputHandler>();
        MyTransform = this.transform;
        RayLength = COG_Rigidbody.GetComponent<SphereCollider>().radius + 0.75f;
        CurrentSpeed = MaxSpeed;

        EngineAudio.loop = true;
        CurrentState = EngineStates.Idle;
        PreviousState = EngineStates.None;
        CycleKinematic.Transform = MyTransform;
        SpawnManager SM = FindObjectOfType<SpawnManager>();
        health = 2;
        InvincibilityTimer = 0.0f;
        Boost = false;
        CurrentConsumable = ConsumableType.None;

        if( !SpecialAudio )
        SpecialAudio = this.gameObject.AddComponent< AudioSource >();
        SpecialAudio.loop = false;
        SpecialAudio.spatialBlend = 1.0f;

        //NOTE: Statistics setup
        TimesBoosted = 0;
        LightTrailActivated = 0;
        ConsumablesUsed = 0;
        int DifficultyLevel = LevelManager.Instance.DifficultyLevel;
        MaxHealth = PlayerHealthLevels[ DifficultyLevel ];
        health = MaxHealth;
    }

    void Update()
    {
        if( Paused ) return;
        float deltaTime = Time.deltaTime;
        HandleUserActions();

        if( InvincibilityTimer > 0 )
        {
            InvincibilityTimer -= deltaTime;
        }

        if( CurrentBoostTimer > 0 )
        {
            CurrentBoostTimer -= deltaTime;
        }
        else if( CurrentBoostTimer <= 0 && Boost )
        {
            Boost = false;
            CurrentBoostRechargeTimer = BoostRechargeTimer;
            //Debug.Log("Stop Boosting");
            CurrentSpeed = MaxSpeed;
        }

        if( CurrentBoostRechargeTimer > 0 )
        {
            CurrentBoostRechargeTimer -= deltaTime;
        }
        else if( CurrentBoostRechargeTimer < 0 && !CanBoost )
        {
            //TODO: This is somewhat bugged and gets hit mulitple times before it actually should be recharged
            CanBoost = true;
            CurrentBoostRechargeTimer = 0;
            AudioManager.Instance.PlaySpecialEffect( in SpecialAudio, SpecialEffect.BoostRecharge );
            //Debug.Log("Boost Recharge");
        }

        //NOTE: Do not touch this
        Velocity = Cycle_Rigidbody.transform.InverseTransformDirection( Cycle_Rigidbody.velocity );
        CycleKinematic.Velocity = Velocity;
        CycleKinematic.Rotation = MyTransform.rotation.eulerAngles.y;
        CurrentVelocityOffset = Velocity.z / CurrentSpeed;

    }

    void FixedUpdate()
    {
        if( Paused ) return;
        MyTransform.position = COG_Rigidbody.transform.position;
        Movement();
    }

    void
    Movement()
    {
        VehicleInput VI = UserInput.GetVehicleInput();

        if( Grounded() )
        {
            if ( VI.braking == 0.0f )
            {
                Acceleration( in VI );
            }
            Brake( in VI );
            Rotation( in VI );
            BikeTilt( in VI );
        }
        else
        {
            CurrentState = EngineStates.Idle;
            Gravity();
        }

        if( CurrentState != PreviousState )
        {
            EngineAudio.Stop();
            AudioManager.Instance.GetAudioClipToPlay( in CurrentState, out CurrentAudioClip );
            EngineAudio.clip = CurrentAudioClip;
            EngineAudio.Play();
        }

        PreviousState = CurrentState;
    }

    void
    Acceleration(in VehicleInput VI)
    {
        if( !Boost )
        {
            if( VI.acceleration > 0 )
            {
                CurrentState = EngineStates.Medium;
            }
            else
            {
                CurrentState = EngineStates.Idle;
            }
        }

        //TODO: We need to adjust this based on whether or not the light trail is active
        COG_Rigidbody.velocity = Vector3.Lerp( COG_Rigidbody.velocity, CurrentSpeed * VI.acceleration * transform.forward, Time.fixedDeltaTime * MoveSpeed);
    }

    void
    Rotation( in VehicleInput VI )
    {
        MyTransform.Rotate(0, VI.steering * VI.acceleration * CurrentVelocityOffset * SteerStrength * Time.fixedDeltaTime, 0, Space.World );
    }

    void
    BikeTilt( in VehicleInput VI )
    {
        float xRot = (Quaternion.FromToRotation( Cycle_Rigidbody.transform.up, hit.normal ) * Cycle_Rigidbody.transform.rotation).eulerAngles.x;
        float zRot = 0;
        if( CurrentVelocityOffset > 0 )
        {
            zRot = -zTiltAngle * VI.steering * CurrentVelocityOffset;
        }

        Quaternion targetRot = Quaternion.Slerp( Cycle_Rigidbody.transform.rotation, Quaternion.Euler( xRot, MyTransform.eulerAngles.y, zRot ), CycleTiltIncrement );

        Quaternion newRotation = Quaternion.Euler( targetRot.eulerAngles.x, MyTransform.eulerAngles.y, targetRot.eulerAngles.z );

        Cycle_Rigidbody.MoveRotation( newRotation );
    }

    void
    Brake( in VehicleInput VI )
    {
        if( VI.braking != 0.0f )
        {
            COG_Rigidbody.velocity *= BrakingPower;
            //TODO: Play a braking sound effect
        }
    }

    bool
    Grounded()
    {
        //NOTE: DEBUG INFO
        //Debug.DrawRay(COG_Rigidbody.transform.position, Vector3.down * RayLength, Color.green);

        if ( Physics.Raycast( COG_Rigidbody.transform.position, Vector3.down, out hit, RayLength, DrivableSurface ) )
        {
            return true;
        }

        //TODO: We need to stop playing the sound of the tyres making contact with the ground
        return false;
    }

    void
    Gravity()
    {
        COG_Rigidbody.AddForce( GravityForce * Vector3.down, ForceMode.Acceleration );
    }

    private void
    HandleUserActions()
    {
        GameInputStates Controller = UserInput.GetInput(); // TODO: We need to get rid of this

        if( Controller.buttonStates.start.buttonPressed )
        { //Pause the game
            LevelManager.Instance.Pause();
        }
        if( Controller.buttonStates.north.buttonPressed )
        {
            HandleConsumableAction();
        }
        if( Controller.buttonStates.south.buttonPressed )
        {
            if( CanBoost )
            {
                //Debug.Log("Boosting");
                CurrentBoostTimer = BoostTimer;
                CurrentSpeed = BoostSpeed;
                Boost = true;
                CanBoost = false;
                //TODO: Switch Engine State to boost
                CurrentState = EngineStates.Boost;
                TimesBoosted++;
            }
        }
        if( Controller.buttonStates.west.buttonPressed )
        {

            if( !Boost )
            {
                if( LightTrail.Emitting ) //NOTE: Slowing down the cycle when the trail is active
                {
                    LightTrail.StopEmitting();
                    CurrentSpeed = MaxSpeed;
                }
                else
                {
                    LightTrail.StartEmitting();
                    CurrentSpeed = .75f * MaxSpeed;
                    LightTrailActivated++;
                }
            }
            else
            {
                if( LightTrail.Emitting ) //NOTE: Slowing down the cycle when the trail is active
                {
                    LightTrail.StopEmitting();
                    CurrentSpeed = BoostSpeed;
                }
                else
                {
                    LightTrail.StartEmitting();
                    CurrentSpeed = .75f * BoostSpeed;
                    LightTrailActivated++;
                }
            }

            AudioManager.Instance.PlaySpecialEffect( in SpecialAudio, SpecialEffect.TrailUp );

        }
    }


    public void
    DamageHealth()
    {
        if( InvincibilityTimer <= 0 )
        {
            health--;
            AudioManager.Instance.PlaySpecialEffect( in SpecialAudio, SpecialEffect.Damaged );
            InvincibilityTimer = InvincibilityTime;
        }

        if( health < 1 )
        {
            DestroyCycle();
            PlayerManager.instance.DestroyedPlayer();
        }
    }

    private void
    Heal()
    {
        health++;
        if( health > MaxHealth )
        {
            health = MaxHealth;
        }

        Debug.Log( "Healed! Current Health = " + health );
    }

    private void
    FireMissile()
    {
    }

    public void
    HandleConsumableAction()
    {
        string text = "Using Consumable: ";
        ConsumablesUsed++;
        switch( CurrentConsumable )
        {
            //TODO: Complete Implementation of the Consumables
            case ConsumableType.Heal:
                text += "Heal";
                Heal();
                UIManager.Instance.HideConsumable();
                break;
            //Make an Invincibility Consumable
            default:
                text = "No Consumable";
                ConsumablesUsed--;
                break;
        }

        print( text );
        CurrentConsumable = ConsumableType.None;
    }

    public void
    PickUp( ConsumableType type )
    {
        CurrentConsumable = type;
        UIManager.Instance.ShowConsumable();
    }

    private void
    DestroyCycle()
    {
        Destroy( this.gameObject );
        print("Player Destroyed");
    }

    private void
    OnDestroy()
    {
    }

    public void
    Pause()
    {
        Paused = true;
        COG_Rigidbody.velocity = Vector3.zero;
        EngineAudio.Stop();
        LightTrail.Pause();
    }

    public void
    Unpause()
    {
        Paused = false;
        EngineAudio.Play();
        LightTrail.UnPause();
    }

    public void
    FillStatistics( out GameStatistics gameStats )
    {
        gameStats = new GameStatistics();
        gameStats.TimesBoosted = TimesBoosted;
        gameStats.LightTrailActivated = LightTrailActivated;
        gameStats.ConsumablesUsed = ConsumablesUsed;
        gameStats.ActiveTrailTime = LightTrail.GetTrailActiveTime();
    }

}
