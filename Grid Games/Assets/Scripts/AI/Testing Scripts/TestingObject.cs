using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TestingAIState
{
    Forward,
    Wander,
    ZigZag,
    Nothing
}

public class TestingObject : MonoBehaviour
{
    public Kinematic MyKinematic;
    public float Speed;
    public float RotationalSpeed;
    public TestingAIState state;
    [Header("AI Follow Targets")]
    public Vector2 MatchOffset;

    private void
    Start()
    {
        MyKinematic = new Kinematic();
        MyKinematic.Transform = this.transform;
        state = TestingAIState.Forward;
    }

    private void
    Update()
    {
        Movement();
    }


    private void
    Movement()
    {
        if( state == TestingAIState.Forward )
        {
            MyKinematic.Velocity = MyKinematic.Transform.forward * Speed;
            MyKinematic.Transform.position += MyKinematic.Velocity * Time.deltaTime;
        }
        else
        {
        }
    }

}
