using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    [Header("Missile Parameters: ")]
    [SerializeField] private int LifeTime;
    private float CurrentLifetime;
    [Range(1,35)]
    [SerializeField] private int MaxSpeed, MaxRotationalSpeed;

    private Transform Target;
    private Transform MyTransform;

    private void
    Start()
    {
        MyTransform = this.transform;
        Target = null;
        CurrentLifetime = LifeTime;
    }

    private void
    Update()
    {
        if( CurrentLifetime > 0 )
        {
            CurrentLifetime -= Time.deltaTime;
        }
        else
        {
            Explode();
        }

        Movement();
    }

    private bool
    SeekNewTarget() //NOTE: We call seek new target when the previous target was destroyed
    {
        List<SeekerAI> EnemiesInScene = AIManager.Instance.GetSeekersInScene();
        float shortestDistance = 10000;

        foreach( SeekerAI Enemy in EnemiesInScene )
        {
            Transform enemyTransform = Enemy.transform;
            Vector3 DirectionToEnemy = enemyTransform.position - MyTransform.position;
            float Distance = DirectionToEnemy.magnitude;
            if( Distance < shortestDistance )
            {
                shortestDistance = Distance;
                Target = enemyTransform;
            }
        }

        if( shortestDistance > ( CurrentLifetime * MaxSpeed ) ) //NOTE: if our closest Enemy is too far away then we have no new target
        {
            return false;
        }

        return true;
    }

    private void
    Movement() //TODO: make this a chasing behavior it needs to predict where the target is heading and move towards that direction
    {

        Vector3 NewPosition = MyTransform.forward * (MaxSpeed * Time.deltaTime);
        MyTransform.position += NewPosition;


        if( !Target )
        {
            if( !SeekNewTarget() )
            {
                print("No Eligible Target Found!!!");
                return;
            }

            //DEBUG: print("New Target: " + Target );
        }
        else
        {
            Rotation();
        }

    }

    private void
    Rotation()
    {
        Vector3 direction = Target.position - MyTransform.position;
        direction.Normalize();

        Quaternion newRotation = Quaternion.LookRotation( direction );
        MyTransform.rotation = Quaternion.Slerp(  MyTransform.rotation, newRotation, 0.01f / MaxRotationalSpeed );
    }

    private void
    Explode()
    {
        print("KABOOM!!!");
        Destroy( this.gameObject );
    }

    private void
    OnCollisionEnter( Collision other )
    {
        // if we hit an enemy than we want to deal damage to it and lit it handle destroying itself
    }

    //TODO: Need to activate Graphical effects, play audio, and cleanup, destroy and spawn an explosion
}
