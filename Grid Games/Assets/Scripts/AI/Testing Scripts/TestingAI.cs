using UnityEngine;

public class TestingAI : MonoBehaviour
{
    private AIManager Overlord;
    [SerializeField] private TrailRenderer LightTrail;

    [Header("AI Parameters")]
    [SerializeField] private float SatisfactionDistance;
    [SerializeField] private float MaxSpeed;

    [Range(1,15)]
    [SerializeField] private int ActivationTimer;
    private float CurrentTimer = 0.0f;

    private Transform MyTransform;
    private WaypointTracker MyCurrentWaypoint;

    [Header("Camera:")]
    [SerializeField] private GameObject VCamera;

    void
    Start()
    {
        Overlord = FindObjectOfType<AIManager>();
        MyTransform = this.transform;
        MyCurrentWaypoint = new WaypointTracker();
        MyCurrentWaypoint = Overlord.StartWaypointTracking( MyTransform.position );
    }

    void
    Update()
    {
        if( CurrentTimer < ActivationTimer )
        {
            CurrentTimer += Time.deltaTime;
        }
        else
        {
            CurrentTimer = 0.0f;
            LightTrail.emitting = !LightTrail.emitting;
        }

        MoveTowardsWaypoint( MyCurrentWaypoint.Transform.position );
    }


    private void
    MoveTowardsWaypoint( Vector3 Position )
    {
        Vector3 TowardsTarget = Position - MyTransform.position;
        if( TowardsTarget.magnitude <= SatisfactionDistance )
        {
            MyCurrentWaypoint = Overlord.GetNextWaypoint( in MyCurrentWaypoint );
            return;
        }
        TowardsTarget.Normalize();
        TowardsTarget *= MaxSpeed;
        MyTransform.position += TowardsTarget * Time.deltaTime;
        Quaternion NewRot = Quaternion.LookRotation( TowardsTarget );
        MyTransform.rotation = Quaternion.Slerp( MyTransform.rotation, NewRot, 0.01f);
    }


    public void
    FocusCamera()
    {
        VCamera.SetActive( true );
    }

}
