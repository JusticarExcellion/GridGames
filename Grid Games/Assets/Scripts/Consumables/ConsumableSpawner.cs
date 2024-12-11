using UnityEngine;

public class ConsumableSpawner : MonoBehaviour
{
    ConsumableSpawner Self;
    public Consumable ConsumableScript;
    public bool ConsumableActive;
    [SerializeField] private int ConsumableRespawnTimer;
    private float CurrentRespawnTimer;
    private bool Paused;

    private void
    Start()
    {
        Self = this.GetComponent< ConsumableSpawner >();
        Paused = false;
        CurrentRespawnTimer = 0f;
    }

    public void
    Pause()
    {
        Paused = true;
    }

    public void
    UnPause()
    {
        Paused = false;
    }

    public void
    NewConsumableSpawn( in Consumable newConsumable )
    {
        ConsumableScript = newConsumable;
        ConsumableActive = true;
    }

    private void
    Update()
    {
        if( Paused ) return;

        if( ConsumableActive && !ConsumableScript )
        {
            CurrentRespawnTimer = ConsumableRespawnTimer;
            ConsumableActive = false;
        }


        if( CurrentRespawnTimer > 0 )
        {
            CurrentRespawnTimer -= Time.deltaTime;
        }
        else if( CurrentRespawnTimer < 0 )
        {
            ConsumableManager.Instance.GenerateConsumable( in Self );
            CurrentRespawnTimer = 0f;
        }
    }
}
