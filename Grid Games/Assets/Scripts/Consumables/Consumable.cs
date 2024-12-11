using UnityEngine;

public enum ConsumableType
{
    Boost,
    Heal,
    Bomb,
    Missile,
    Test,
    None
}

public class Consumable : MonoBehaviour
{
    public ConsumableType type;
    public AudioSource Audio;

    public void
    Pause()
    {
        Audio.Stop();
    }

    public void
    UnPause()
    {
        AudioManager.Instance.PlayConsumableAudio( in Audio );
    }

    private void
    OnTriggerEnter( Collider other )
    {
        PlayerCollision collidingCycle = other.transform.GetComponent< PlayerCollision >();

        if( collidingCycle )
        {
            collidingCycle.player.PickUp( type );
            DestroyConsumable();
        }

    }

    private void
    DestroyConsumable()
    {
        ConsumableManager.Instance.RemoveSelfFromConsumablesList( this.gameObject );
        Destroy( this.gameObject );
    }
}
