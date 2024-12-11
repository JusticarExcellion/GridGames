using UnityEngine;

public class LightTrailCollisions : MonoBehaviour
{

    //TODO: Need to create the process for detecting if someone has hit a light trail
    //This will be very perfomance critical since we have more than one object going to be running this

    //TODO: This is causing damage and hits to objects that are not actually getting hit by the light trail, we need to figure why and how to stop it, or cheat it

    //NOTE: Need to think about the maximum amount of AI Enemies in a level to still be performant without any significant frame drops

    /* NOTE: This seems to be a decent solution for this needs a lot of testing:
     * https://www.reddit.com/r/Unity3D/comments/14hpici/detecting_collisions_on_trail_renderer_in_3d/
     */

    public TrailRenderer trailRenderer;
    public bool Emitting;
    public bool Paused;
    public float TrailTime;
    private float TrailActiveTime;
    private Transform MyTransform;
    //TODO: Track the number of times we activate the light trail
    //TODO: We could also track the number of seconds we have the light trail active
    [SerializeField] private AudioSource TrailAudioSource;
    [SerializeField] private bool AIControlled;
    [SerializeField] private LayerMask DamagingLayer;

    private void Start()
    {
        TrailTime = trailRenderer.time;
        MyTransform = this.transform;
        float volume = AudioManager.Instance.GetSoundEffectVolume();
        TrailAudioSource.clip = AudioManager.Instance.TrailAudio;
        TrailAudioSource.loop = true;
        TrailAudioSource.spatialBlend = 1f;
        TrailAudioSource.volume = volume;
        TrailActiveTime = 0f;
    }

    public void
    StartEmitting()
    {
        trailRenderer.emitting = true;
        TrailAudioSource.Play();
        Emitting = true;
    }

    public void
    StopEmitting()
    {
        trailRenderer.emitting = false;
        TrailAudioSource.Stop();
        Emitting = false;
    }

    public void
    Pause()
    {
        if( Emitting )
        {
            TrailAudioSource.Stop();
        }
        Paused = true;
    }

    public void
    UnPause()
    {
        if( Emitting )
        {
            TrailAudioSource.Play();
        }
        float volume = AudioManager.Instance.GetSoundEffectVolume();
        TrailAudioSource.volume = volume;
        Paused = false;
    }

    private void
    Update()
    {
        if( !Paused && Emitting ) TrailActiveTime += Time.deltaTime;
    }

    //NOTE: Test this and try to figure out a way to filter these results
    private void FixedUpdate()
    {
        if( !Emitting ) return;
        //We need to filter these results to ignore the current model when checking for other models to deal damage to
        for( int i = 0; i < trailRenderer.positionCount; i++ )
        {
            if ( i == trailRenderer.positionCount - 1)
            {
                continue;
            }

            float t = 1 / (float) trailRenderer.positionCount;

            float width = trailRenderer.widthCurve.Evaluate( t );

            Vector3 startPosition = trailRenderer.GetPosition( i );
            Vector3 endPosition = trailRenderer.GetPosition( i + 1 );
            Vector3 direction = endPosition - startPosition;
            float distance = Vector3.Distance( endPosition, startPosition );

            RaycastHit hit;

            if ( Physics.SphereCast( startPosition, width, direction, out hit, distance, DamagingLayer ) )
            {
                //NOTE: Damage Enemy
                if( AIControlled )
                {
                    PlayerCollision playerCollider = hit.collider.gameObject.GetComponent<PlayerCollision>();
                    playerCollider.player.DamageHealth();
                }
                else
                {
                    SeekerAI enemy = hit.collider.gameObject.GetComponent<SeekerAI>();
                    if( enemy )enemy.DamageHealth();
                }
            }
        }

    }

    public float
    GetTrailActiveTime()
    {
        float activeTime = Mathf.Round( TrailActiveTime * 100 ) * .01f;
        return activeTime;
    }
}
