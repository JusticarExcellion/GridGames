using UnityEngine;
using UnityEngine.Assertions;

public class MotorcycleController : MonoBehaviour
{
    //TODO: PUT MODELS ON A DIFFERENT PHYSICS LAYER, than colliders and see how that works with the current leaning implementation
    public VehicleInput VInput;

    public float MotorTorque;
    public float BrakeTorque;
    public float MaxSpeed;
    public float SteeringRange;
    public float SteeringRangeAtMaxSpeed;
    public float CentreOfGravityOffset;
    public float CasterAngle; // The angle between the vertical axis of the front wheel to the steering axis of the motorcycle measured in degrees

    [Header("Motorcycle Rigidbody")]
    [SerializeField] private Rigidbody rigidbody;

    [Header("Wheels")]
    [SerializeField] private WheelControl FrontWheel;
    [SerializeField] private WheelControl BackWheel;

    [Header("Center of Gravity (COG)")]
    [SerializeField] private Transform COGTransform;

    [Header("Leaning")]
    public float MaxLeanAngle;

    private float WheelBaseLength;
    private float CurrentSpeed;
    private readonly float Gravity = 9.8f;
    private Transform MyTransform;

    void
    Start()
    {
        MotorTorque = 1000f;
        BrakeTorque = 300f;
        SteeringRange = 80f;
        MaxSpeed = 100.0f;
        SteeringRangeAtMaxSpeed = 20f;
        CentreOfGravityOffset = -1f;

        CasterAngle = 24.2f;
        MaxLeanAngle = 45.0f;

        rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.centerOfMass += Vector3.up * CentreOfGravityOffset;

        WheelBaseLength = FrontWheel.WheelModel.position.z - BackWheel.WheelModel.position.z;
        MyTransform = this.transform;
    }

    void
    Update()
    {
        Movement();
        //Lean();
    }

    void Movement()
    {
        float speedFactor = 0.0f;

        CurrentSpeed = Vector3.Dot( transform.forward, rigidbody.velocity );
        speedFactor = Mathf.InverseLerp( 0, MaxSpeed, CurrentSpeed );

        float currentMotorTorque = Mathf.Lerp( MotorTorque, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp( SteeringRange, SteeringRangeAtMaxSpeed, speedFactor );

        FrontWheel.wheelCollider.steerAngle = VInput.steering * currentSteerRange;
        if( VInput.acceleration >= 0.0f ) // If we are accelerating
        {
            FrontWheel.wheelCollider.motorTorque = VInput.acceleration * currentMotorTorque;
            BackWheel.wheelCollider.motorTorque = VInput.acceleration * currentMotorTorque;
        }
            FrontWheel.wheelCollider.brakeTorque = BrakeTorque * VInput.braking;
            BackWheel.wheelCollider.brakeTorque = BrakeTorque * VInput.braking;

    }

    //TODO:(Xander) We are rotating our motorcycle body currently, and it's meetingc with some mixed results, bike traction and vehicle settings need to be tuned, before this can be ironed out
    void Lean()
    {
        float steeringAngle = FrontWheel.wheelCollider.steerAngle;
        Vector3 currentRotation = MyTransform.rotation.eulerAngles;
        if( Mathf.Abs(steeringAngle) > 0.1f){

            float TurnRadius = WheelBaseLength / ( steeringAngle * Mathf.Cos(CasterAngle) );
            float LeanAngle = Mathf.Atan( ( CurrentSpeed * CurrentSpeed ) / ( Gravity * TurnRadius ));
            LeanAngle *= -180f/3.14f; // Converting to Degrees

            Vector3 newRotation = new Vector3( currentRotation.x, currentRotation.y, LeanAngle);

            COGTransform.rotation = Quaternion.Slerp( COGTransform.rotation, Quaternion.Euler( newRotation ), Time.deltaTime );
            //TODO: (Xander): Does using fixed delta time mess things up here?
        }
        else // If we're straigtening out we go to 0 degrees on Z
        {
            COGTransform.rotation = Quaternion.Slerp( COGTransform.rotation, Quaternion.Euler( new Vector3(currentRotation.x, currentRotation.y, 0) ), Time.deltaTime * 0.1f );
        }

    }

}
