using UnityEngine;


public class CycleBehavior : MonoBehaviour
{
    //TODO: We need to check if the motorcycle is player controlled
    //then we will get our input from the player, otherwise it will need an AI will need to created to move the cycle

    //TODO: We need to add sound cues based off the state of this, layering audio

    private InputHandler UserInput; // TODO: We want to get rid of this and not have the Cycle know about the user unless plans change

    [Header("Character Parameters")]
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
    public TrailRenderer LightTrail;
    public PlayerManager PM;

    public AudioSource EngineAudio;
    public AudioClip CurrentAudioClip;

    private EngineStates CurrentState;
    private EngineStates PreviousState;

    void Start()
    {
        COG_Rigidbody.transform.parent = null;
        Cycle_Rigidbody.transform.parent = null;
        CycleKinematic = new Kinematic();


        UserInput = this.GetComponent<InputHandler>();
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
    }

    void Update()
    {
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
            CanBoost = true;
            CurrentBoostRechargeTimer = 0;
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
        if( VI.acceleration > 0 )
        {
            CurrentState = EngineStates.Medium;
        }
        else
        {
            CurrentState = EngineStates.Idle;
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
            }
        }
        if( Controller.buttonStates.west.buttonPressed ){


            if( !Boost )
            {
                if( LightTrail.emitting ) //NOTE: Slowing down the cycle when the trail is active
                {
                    LightTrail.emitting = false;
                    CurrentSpeed = MaxSpeed;
                }
                else
                {
                    LightTrail.emitting = true;
                    CurrentSpeed = .75f * MaxSpeed;
                }
            }
            else
            {
                if( LightTrail.emitting ) //NOTE: Slowing down the cycle when the trail is active
                {
                    LightTrail.emitting = false;
                    CurrentSpeed = BoostSpeed;
                }
                else
                {
                    LightTrail.emitting = true;
                    CurrentSpeed = .75f * BoostSpeed;
                }
            }

        }
    }


    public void
    DamageHealth()
    {
        if( InvincibilityTimer <= 0 )
        {
            health--;
            print("Damaged Health");
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
    }

    private void
    FireMissile()
    {
        ConsumableManager cm = FindObjectOfType<ConsumableManager>();
        GameObject go = Instantiate( cm.MissileProjectile );
        go.transform.position = ProjectileSpawn.position;

    }

    public void
    HandleConsumableAction()
    {
        string text = "Using Consumable: ";
        switch( CurrentConsumable )
        {
            //TODO: Complete Implementation of the Consumables
            case ConsumableType.Heal:
                text += "Heal";
                Heal();
                break;
            case ConsumableType.Missile:
                text += "Missile";
                FireMissile();
                break;
            case ConsumableType.Test:
                text += "Test";
                break;
            //Make an Invincibility Consumable
            default:
                text = "No Consumable";
                break;
        }

        print( text );
        CurrentConsumable = ConsumableType.None;
    }

    private void
    DestroyCycle()
    {
        //TODO: This is where we activate and play graphical effects, sounds, and update intiate updates to global lists like the current number of Enemies in the scene
        Destroy( this.gameObject );
        print("Player Destroyed");
    }

    private void
    OnDestroy()
    {
    }

}
