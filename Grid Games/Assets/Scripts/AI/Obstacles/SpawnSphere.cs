using UnityEngine;

public class SpawnSphere : MonoBehaviour
{

    private readonly float DeathTimer = .15f;
    private float CurrentCountdownTimer;
    [SerializeField] private AudioSource SphereAudio;

    void Start()
    {
        CurrentCountdownTimer = DeathTimer;
        AudioManager.Instance.PlaySpecialEffect( in SphereAudio, SpecialEffect.EnemySpawn );
    }

    void Update()
    {
        if( CurrentCountdownTimer < 0 ) Destroy( this.gameObject );
        CurrentCountdownTimer -= Time.deltaTime;
    }
}
