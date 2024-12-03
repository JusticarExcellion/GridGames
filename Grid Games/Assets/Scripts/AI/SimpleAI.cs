using UnityEngine;

public class SimpleAI : MonoBehaviour
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
    public AudioSource EngineAudio;
    private AudioClip CurrentClip;
    private EngineStates CurrentState;
    private EngineStates PreviousState;

    void
    Start()
    {
        Overlord = FindObjectOfType<AIManager>();
        MyTransform = this.transform;
        MyCurrentWaypoint = new WaypointTracker();
        MyCurrentWaypoint = Overlord.StartWaypointTracking( MyTransform.position );
        EngineAudio.loop = true;
        CurrentState = EngineStates.Slow;
        PreviousState = EngineStates.None;
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

        if( CurrentState != PreviousState )
        {
            EngineAudio.Stop();
            AudioManager.Instance.GetAudioClipToPlay( in CurrentState,out CurrentClip );
            EngineAudio.clip = CurrentClip;
            EngineAudio.Play();
        }

        PreviousState = CurrentState;
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

}
