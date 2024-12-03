using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterTargetInfo
{
    public float AlignmentToTarget;
    public float Speed;
}

public class CycleAI : MonoBehaviour
{
    private AIManager Overlord;
    private Kinematic CharacterKinematic;
    private Steering CharacterSteering;

    [Header("AI Parameters:")]
    [SerializeField] private int SlowRadius;

    [Header("Cycle Parameters: ")]
    [SerializeField] private float MaxSpeed;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float MaxRotationalSpeed;
    [SerializeField] private float MinRotationalSpeed;

    private float CurrentSpeed;

    [Header("Components: ")]
    public Rigidbody COG_Rigidbody, Cycle_Rigidbody;

    [Header("Target Overide: ")]
    public Transform Target;

    void Start()
    {
        CharacterKinematic = new Kinematic();
        CharacterKinematic.Transform = this.transform;
        CharacterKinematic.Velocity = Vector3.zero;
        CharacterKinematic.Rotation = CharacterKinematic.Transform.rotation.eulerAngles.y;

        CurrentSpeed = MaxSpeed;
        Overlord = FindObjectOfType<AIManager>();
        //TODO: Used for testing purposes get rid of this
        if( Target ) CharacterSteering = AIResources.Seek( in Target, in CharacterKinematic );
    }

    void Update()
    {

        //TODO: Get Rid of this this is for testing purposes
        if(!Target)
        {
            if( FindNewTarget() )
            {
                Debug.Log("No More Consumables!");
            }
        }

        CharacterKinematic.Velocity = Cycle_Rigidbody.transform.InverseTransformDirection( Cycle_Rigidbody.velocity );
        CharacterKinematic.Rotation = CharacterKinematic.Transform.rotation.eulerAngles.y;
    }

    void FixedUpdate()
    {
        if( Target ) CharacterSteering = AIResources.Seek( in Target, in CharacterKinematic );
        CharacterKinematic.Transform.position = COG_Rigidbody.transform.position;
        Movement();
    }

    private void
    Movement()
    {
        //NOTE: If we have no target we stop
        if( !Target )
        {
            Brake();
            return;
        }

        Vector3 NormalizedSteeringVelocity = CharacterSteering.Velocity;
        NormalizedSteeringVelocity.Normalize();

        CharacterTargetInfo MoveInfo = new CharacterTargetInfo();
        MoveInfo.Speed = CharacterKinematic.Velocity.magnitude;
        print("Cycle Speed: " + MoveInfo.Speed );
        MoveInfo.AlignmentToTarget = Vector3.Dot( CharacterKinematic.Transform.right, NormalizedSteeringVelocity );

        Acceleration( in MoveInfo );

        //NOTE: We only brake if we're not aligned and we're going to fast
        if( ( MoveInfo.AlignmentToTarget > .25f || MoveInfo.AlignmentToTarget < -.25f ) && ( MoveInfo.Speed > MaxSpeed/2) ) // We want to move slowly until we are aligned
        {
            Brake();
        }

        //NOTE: Stops Rotational Jittering
        if( MoveInfo.AlignmentToTarget > .02f || MoveInfo.AlignmentToTarget < -.02f )
        {
            Rotation( in MoveInfo );
        }
    }

    private void
    Acceleration( in CharacterTargetInfo MoveInfo )
    {

        float Acceleration = 0.0f;

        if( CharacterSteering.Velocity.magnitude > SlowRadius )
        {
            Acceleration = 1f;
        }

        Vector3 NormalizedSteeringVelocity = CharacterSteering.Velocity;
        NormalizedSteeringVelocity.Normalize();

        float AlignmentToTarget = Vector3.Dot( CharacterKinematic.Transform.right, NormalizedSteeringVelocity );

        if( ( AlignmentToTarget > .25f || AlignmentToTarget < -.25f ) ) // We want to move slowly until we are aligned
        {
            Acceleration = .5f;
        }

        COG_Rigidbody.velocity = Vector3.Lerp( COG_Rigidbody.velocity, CurrentSpeed * Acceleration * transform.forward, Time.fixedDeltaTime * MoveSpeed);
    }

    private void
    Rotation( in CharacterTargetInfo MoveInfo ) //TODO: Character should rotate towards a specified direction, but only when the Cycle is moving
    {
        float speed = CharacterKinematic.Velocity.magnitude;
        if( speed > 0)
        {
            speed = 1.0f;
        }

        float rotationalSpeed = Mathf.Lerp( MaxRotationalSpeed, MinRotationalSpeed, speed  );

        //If we are moving at all then we can rotate, though the steering should be clamped to an upper and lower value depending on the current speed, with steering improving at lower speeds and shrinking at higher speeds
        Vector3 NormalizedSteeringVelocity = CharacterSteering.Velocity;
        NormalizedSteeringVelocity.Normalize();

        float steeringInput = Vector3.Dot( CharacterKinematic.Transform.right, NormalizedSteeringVelocity );

        if( steeringInput > 0 )
        {
            steeringInput = 1.0f;
        }
        else if( steeringInput < 0 )
        {
            steeringInput = -1.0f;
        }

        CharacterKinematic.Transform.Rotate(0, steeringInput * speed * rotationalSpeed * Time.fixedDeltaTime, 0, Space.World );
    }

    private void
    Brake()
    {
        float brakingPower = 10.0f;
        COG_Rigidbody.velocity *= brakingPower / 10;
    }

    public bool
    FindNewTarget() //TODO: Searches for targets and when there are no nearby targets then searches for a consumable
    {
        ConsumableManager CM = FindObjectOfType<ConsumableManager>();
        if( !CM )
        {
            print("NO CONSUMABLE MANAGER");
            return false;
        }

        float shortestDistance = 10000;

        foreach( GameObject consumable in CM.ConsumablesInScene )
        {
            Transform ObjectTransform = consumable.transform;
            Vector3 Direction = ObjectTransform.position - CharacterKinematic.Transform.position;
            float distance = Direction.magnitude;

            if( distance < shortestDistance )
            {
                Target = ObjectTransform;
                shortestDistance = distance;
            }
        }

        if( !Target )
        {
            Debug.Log("No New Targets");
            return false;
        }

        return true;
    }

}


