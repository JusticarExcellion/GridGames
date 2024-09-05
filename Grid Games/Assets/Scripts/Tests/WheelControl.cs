using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{

    public Transform WheelModel;

    [HideInInspector] public WheelCollider wheelCollider;

    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    void Update()
    {
        Vector3 position;
        Quaternion rotation;

        wheelCollider.GetWorldPose( out position, out rotation );

        WheelModel.transform.position = position;
        WheelModel.transform.rotation = rotation;

    }
}
