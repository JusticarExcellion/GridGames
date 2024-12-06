using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Steering
{
    public Vector3 Velocity;
    public Quaternion Rotation;
}

public class Kinematic //NOTE: We want to contain the current Velocity of our character as well as their current rotation, rotation is just the angular motion
{
    public Transform Transform;
    public Vector3 Velocity;
    public float Rotation;
}

public enum AIState
{
    Standby,
    Wander,
    Seek,
    Chase,
    Match,
    Attack,
    Flee
}

public class AIResources
{
    //TODO: This is where we will implement all of the behaviors Seek, avoid, chase, search

    public static readonly int MaxPredictionTime = 5;

    //We need to be using a common structure to pass around and feed into the Actuator part of the AI
    public static Steering
    Seek( in Transform Target, in Kinematic Character ) // Seek a stationary target, Outputs: The direction the character moves and the angle the character needs to rotate towards
    {
        Steering Result = new Steering();
        Result.Velocity = Target.position - Character.Transform.position;
        //NOTE: We expect whoever is recieving the steering to decide whether they should slow down or whatever else they want to do

        Result.Rotation = Quaternion.LookRotation( Result.Velocity );

        //Debug.Log("Seek Result: \n\tVelocity:" + Result.Velocity + "\n\tRotation: " + Result.Rotation );

        return Result;
    }

    public static Steering
    Seek( in Kinematic Target, in Kinematic Character ) // Seeking a moving target
    {
        Steering Result = new Steering();
        Result.Velocity = Target.Transform.position - Character.Transform.position;

        Result.Rotation = Quaternion.LookRotation( Result.Velocity );

        return Result;
    }

    public static Steering
    Chase( in Kinematic Target, in Kinematic Character )
    {
        Steering Result = new Steering();
        Vector3 direction = Target.Transform.position - Character.Transform.position;
        float distance = direction.magnitude;
        float characterSpeed = Character.Velocity.magnitude;
        float prediction = 0.0f;

        if( characterSpeed <= distance / MaxPredictionTime )
        {
            prediction = MaxPredictionTime;
        }
        else
        {
            prediction = distance / characterSpeed;
        }

        //NOTE: Seek Behavior at the prediction target
        //TODO: This only works correctly seemingly in the -z direction does not chase properly otherwise
        Vector3 NewSeekTarget = Target.Transform.position + (Target.Velocity * prediction);

        /* DEBUG:
        Debug.Log( "Prediction Time: " + prediction );
        Debug.Log( "Target Velocity: " + Target.Velocity );
        Debug.Log( "NewSeekTarget: " + NewSeekTarget );
        */

        NewSeekTarget.y = 0;
        Result.Velocity = NewSeekTarget - Character.Transform.position;
        Result.Rotation = Quaternion.LookRotation( Result.Velocity );

        return Result;
    }


    public static Steering
    KinematicArrive( Vector3 Position, in Kinematic Character, int MaxSpeed,  int SlowRadius, int TargetRadius )
    {
        Steering Result = new Steering();
        Result.Rotation = Character.Transform.rotation;

        Vector3 direction = Position - Character.Transform.position;
        float distance = direction.magnitude;
        float targetSpeed = 0.0f;

        //TODO: Seeker is fidgeting once it reaches the target radius ofter it arrives it just needs to hold it's same speed and distance to the target
        //AI should also know if the player is braking

        if( distance < TargetRadius )
        {
            return Result;
        }

        if( distance > SlowRadius )
        {
            targetSpeed = MaxSpeed;
        }
        else
        {
            targetSpeed = MaxSpeed * distance / SlowRadius;
        }

        direction.y = 0;
        direction.Normalize();
        direction *= targetSpeed;
        Result.Velocity = direction;
        Result.Rotation = Quaternion.LookRotation( direction );

        return Result;
    }

    public static Steering
    Flee( in Kinematic Target, in Kinematic Character )
    {
        //TODO: Implement Flee
        Steering Result = new Steering();
        Result.Velocity = Character.Transform.forward;
        return Result;
    }

    public static Steering
    Attack( in Kinematic Target, in Kinematic Character, int AttackLookAhead )
    {
        Steering Result = new Steering();
        Vector3 direction = Target.Transform.position - Character.Transform.position;
        float distance = direction.magnitude;
        float characterSpeed = Character.Velocity.magnitude;
        float prediction = 0.0f;

        if( characterSpeed <= distance / MaxPredictionTime )
        {
            prediction = MaxPredictionTime;
        }
        else
        {
            prediction = distance / characterSpeed;
        }

        //NOTE: Seek Behavior at the prediction target
        Vector3 NewSeekTarget = Target.Transform.position + (Target.Velocity * prediction);
        NewSeekTarget += Target.Transform.forward * AttackLookAhead;

        /* DEBUG:
        Debug.Log( "Prediction Time: " + prediction );
        Debug.Log( "Target Velocity: " + Target.Velocity );
        Debug.Log( "NewSeekTarget: " + NewSeekTarget );
        */

        Result.Velocity = NewSeekTarget - Character.Transform.position;
        Result.Velocity.y = 0;
        Result.Rotation = Quaternion.LookRotation( Result.Velocity );

        return Result;
    }

}
