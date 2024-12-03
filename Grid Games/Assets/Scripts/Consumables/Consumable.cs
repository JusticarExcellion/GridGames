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

    void
    OnTriggerEnter( Collider other )
    {
        COGConnector collidingCycle = other.transform.GetComponent<COGConnector>();

        if( collidingCycle )
        {
            collidingCycle.PickUpConsumable( type );
            DestroyConsumable();
        }

    }

    private void
    DestroyConsumable()
    {
        ConsumableManager CM = FindObjectOfType<ConsumableManager>();
        CM.RemoveSelfFromConsumablesList( this.gameObject );
        //Perform Any other cleanup and activate the special effects here
        Destroy( this.gameObject );
    }
}
