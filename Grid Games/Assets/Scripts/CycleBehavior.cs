using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class CycleBehavior : MonoBehaviour
{

    private InputHandler UserInput; // TODO: We want to get rid of this and not have the Cycle know about the user unless plans change
    [SerializeField] private float MaxSpeed, MoveSpeed, SteerStrength, CycleTiltIncrement = .15f, zTiltAngle = 45f;
    [Range(1,10)]
    public float BrakingPower;

    private float RayLength;
    private RaycastHit hit;
    public LayerMask DrivableSurface;

    [SerializeField] private float GravityForce;
    private float CurrentVelocityOffset;

    [HideInInspector] public Vector3 Velocity;

    public Rigidbody COG_Rigidbody, Cycle_Rigidbody;
    Transform MyTransform;

    void Start()
    {
        COG_Rigidbody.transform.parent = null;
        Cycle_Rigidbody.transform.parent = null;

        UserInput = this.GetComponent<InputHandler>();
        MyTransform = this.transform;
        RayLength = COG_Rigidbody.GetComponent<SphereCollider>().radius + 0.75f;
    }

    void Update()
    {

        Velocity = Cycle_Rigidbody.transform.InverseTransformDirection( Cycle_Rigidbody.velocity );
        CurrentVelocityOffset = Velocity.z / MaxSpeed;

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
                Rotation( in VI );
            }
            Brake( in VI );
            BikeTilt( in VI );
        }
        else
        {
            Gravity();
        }

    }

    void
    Acceleration(in VehicleInput VI)
    {
        COG_Rigidbody.velocity = Vector3.Lerp( COG_Rigidbody.velocity, MaxSpeed * VI.acceleration * transform.forward, Time.fixedDeltaTime * MoveSpeed);
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
            COG_Rigidbody.velocity *= BrakingPower / 10;
        }
    }

    bool
    Grounded()
    {
        //NOTE: DEBUG INFO
        Debug.DrawRay(COG_Rigidbody.transform.position, Vector3.down * RayLength, Color.green);

        if ( Physics.Raycast( COG_Rigidbody.transform.position, Vector3.down, out hit, RayLength, DrivableSurface ) )
        {
            return true;
        }

        return false;
    }

    void
    Gravity()
    {
        COG_Rigidbody.AddForce( GravityForce * Vector3.down, ForceMode.Acceleration );
    }

}
