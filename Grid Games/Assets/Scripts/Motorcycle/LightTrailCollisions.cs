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
    private Transform MyTransform;
    [SerializeField] private bool AIControlled;
    [SerializeField] private LayerMask DamagingLayer;

    private void Start()
    {
        MyTransform = this.transform;
    }

    //NOTE: Test this and try to figure out a way to filter these results
    private void FixedUpdate()
    {
        if( !trailRenderer.emitting ) return;
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
                    SeekerAI enemy = hit.collider.gameObject.GetComponent<SeekerAI>();;
                    if( enemy )enemy.DamageHealth();
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
    }
}
