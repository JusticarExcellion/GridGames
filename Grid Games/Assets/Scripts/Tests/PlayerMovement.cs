using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //NOTE: (Xander): We may want to think about a unified class for all motorcycles that the AI and Player can interact with and make calls on.
    //NOTE: (Xander): We need to start thinking about the player controller and finetuning the camera.

    MovementInput PlayerMovementInput;
    Transform MyTransform;
    [SerializeField] InputSystem_test InputSystem;

    [SerializeField] GameObject Motorcycle;
    Transform MotorcycleTransform;
    Rigidbody MotorcycleRigidbody;

    [SerializeField] Rigidbody COG;

    private Vector3 Velocity;
    private float CurrentVelocityOffset;
    private float ZTiltAngle;

    private float BoostSpeed;
    private float MaxSpeed;
    private float Speed;

    private float StartingBrakePower;
    private float BrakePowerIncrease;
    private float BrakePower;

    private float currentBoostPeriod;
    private float BoostPeriod;
    private float RotationSpeed;
    private bool Paused;

    void Start()
    {
        MyTransform = this.transform;
        MotorcycleTransform = Motorcycle.transform;
        MotorcycleRigidbody = Motorcycle.GetComponent<Rigidbody>();
        MotorcycleTransform.parent = null;
        COG.transform.parent = null;

        Velocity = Vector3.zero;
        CurrentVelocityOffset = 0.0f;
        ZTiltAngle = 30.0f;

        StartingBrakePower = 2.0f;
        BrakePowerIncrease = 1.0f;
        BrakePower = StartingBrakePower;
        Speed = 15.0f;
        MaxSpeed = 15.0f;
        BoostSpeed = 25.0f;
        currentBoostPeriod = 0.0f;
        BoostPeriod = 3.0f * 60.0f; //TODO: Is there a way to get the framerate? If so we should replace this line.

        RotationSpeed = 50.0f;
        Paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        Velocity = MotorcycleTransform.InverseTransformDirection( MotorcycleRigidbody.velocity );
        CurrentVelocityOffset = Velocity.z / MaxSpeed;
    }

    void FixedUpdate()
    {
        //TODO: Condense the amount of work we are doing here, we should only rotate while moving, we break and accelerate independently, we only tilt while rotating, etc. etc.
        HandleMovement();
        HandleRotation();

        MotorcycleRigidbody.MoveRotation( MyTransform.rotation );
        MyTransform.position = COG.transform.position;

    }

    public void HandleMovement()
    {
        PlayerMovementInput = InputSystem.GetMovementInput();
        //Note: Operate on Values
        Vector3 LinearVelocity = MyTransform.forward * PlayerMovementInput.acceleration;
        LinearVelocity.Normalize();
        LinearVelocity.y = 0;
        LinearVelocity *= Speed;

        if( currentBoostPeriod > 0.0f )
        {
            LinearVelocity = MyTransform.forward * BoostSpeed;
            currentBoostPeriod -= 1.0f;
        }
        else
        {
            RotationSpeed = 50.0f;
        }

        COG.velocity = LinearVelocity;
    }

    public void HandleRotation()
    {
        //Turning the Game Object
        //TODO: This needs to be affected by the Linear Velocity so we can't turn in place while braking
        float torqueAmount = RotationSpeed * PlayerMovementInput.acceleration * PlayerMovementInput.horizontalInput * Time.fixedDeltaTime;

        MyTransform.Rotate( 0, torqueAmount, 0, Space.World ); // Rotating Motorcycle
    }

    public void HandleBraking()
    {
        COG.velocity *= Speed / BrakePower;
        BrakePower+=BrakePowerIncrease;
    }

    public void ResetBreaking()
    {
        BrakePower = StartingBrakePower;
    }

    public void Boost()
    {
        if( currentBoostPeriod <= 0.0f)
        {
            currentBoostPeriod = BoostPeriod;
            RotationSpeed *= 2;
            Debug.Log("Boost");
        }
    }

    public void Activate()
    {
        Debug.Log("Activate");
    }

    public void Use()
    {
        Debug.Log("USE");
    }

    public void Pause()
    {
        if( !Paused )
        {
            Time.timeScale = 0;
            Paused = true;
        }
        else
        {
            Time.timeScale = 1;
            Paused = false;
        }
    }

}
