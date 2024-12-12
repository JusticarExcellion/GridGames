using UnityEngine;

public class LaserGrid : MonoBehaviour
{
    [Range(1,20)]
    [SerializeField] private float Force;
    [SerializeField] private ForceMode Mode;
    private void
    OnCollisionEnter( Collision other )
    {
        CycleBehavior player = null;
        if( other.gameObject != null ) player = other.gameObject.GetComponent<PlayerCollision>().player;

        if( player )
        {
            Rigidbody playerRigidbody = player.COG_Rigidbody;
            playerRigidbody.velocity = Vector3.zero;
            //NOTE: we push the player back and up relative to the surface normal
            Vector3 ForceVector = this.transform.up;
            Vector3 ContactPosition = other.GetContact(0).point;
            Vector3 BackwardsVector = other.GetContact(0).normal;
            BackwardsVector *= -1;
            ForceVector += BackwardsVector;
            ForceVector *= Force;
            playerRigidbody.AddForce(ForceVector, Mode);
            Debug.DrawRay( ContactPosition, ForceVector,  Color.red, 1.0f );
            player.DamageHealth();
        }
        else //The collision is against an AI
        {
            //NOTE: Check to see if it collided with an ai or something else
        }
    }
}
