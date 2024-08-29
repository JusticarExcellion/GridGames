using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    Transform UTransform;
    [SerializeField] Transform FollowTarget;
    readonly float HorizontalSpeed = 4.0f;
    readonly float VerticalSpeed = 4.0f;
    // Start is called before the first frame update
    void Start()
    {
        UTransform = this.transform;
        if(!FollowTarget) Debug.LogError("You Suck");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 NewPosition = UTransform.position;
        //NOTE: This is all just filler code to see if things work
        if( Input.GetKey(KeyCode.W) ) // Up
        {
            NewPosition.x += 1.0f* Time.deltaTime;
        }
        if( Input.GetKey(KeyCode.A) ) // Left
        {
            NewPosition.z -= 1.0f* Time.deltaTime;
        }
        if( Input.GetKey(KeyCode.S) ) // Down
        {
            NewPosition.x -= 1.0f* Time.deltaTime;
        }
        if( Input.GetKey(KeyCode.D) ) // Right
        {
            NewPosition.z += 1.0f * Time.deltaTime;
        }
        UTransform.position = NewPosition;

        //Rotation
        float HorizontalRotation = Input.GetAxis("Mouse X") * HorizontalSpeed;
        float VerticalRotation = Input.GetAxis("Mouse Y") * VerticalSpeed;

        FollowTarget.Rotate(HorizontalRotation, VerticalRotation, 0);
        //Clamping Rotation
        Quaternion FollowRotation = FollowTarget.rotation;
        FollowRotation.y = Mathf.Clamp(FollowRotation.x, 0.0f, 90.0f);
        FollowTarget.rotation = FollowRotation;
    }
}
