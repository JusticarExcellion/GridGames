using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum EngineStates
{
    Idle,
    Medium,
    Boost,
    None
}

public enum SpecialEffect
{
    TrailUp,
    AttackSound,
    Destroyed,
    Damaged,
    EnemyDamaged,
    BoostRecharge,
    EnemySpawn
}

public enum EnemyType
{
    Seeker,
    Interceptor
}

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    public AudioClip Music;

    [Header("SFX")]

    [Header("Player Audio")]
    public AudioClip Damaged;
    public AudioClip BoostRecharge;

    [Header("Engine Audio")]
    public AudioClip Idle;
    public AudioClip Medium;
    public AudioClip Boost;

    [Header("Trail Audio")]
    public AudioClip TrailUp;
    public AudioClip TrailAudio;

    [Header("Enemy Audio")]
    public AudioClip AttackSound;
    public AudioClip EnemyDestroyed;
    public AudioClip EnemyDamaged;
    public AudioClip EnemySpawn;
    public AudioClip SeekerAudio;
    public AudioClip InterceptorAudio;

    [Header("Consumables Audio")]
    public AudioClip HealingAudio;


    private Dictionary<EngineStates, AudioClip> EngineAudio;
    private AudioSource MusicSource;

    private int MasterVolume;
    private int MusicVolume;
    private int SFXVolume;

    public static AudioManager Instance { get; private set; }

    private void
    Awake()
    {
        if(Instance != null && Instance != this )
        {
            Destroy( this );
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad( this );
    }

    public bool
    InitializeAudio( in AudioMixerGroup AudioMixer, in AudioClip LevelMusic )
    {

        //NOTE: We'll need to change this to change music for each level

        if( LevelMusic )
        {
            string MusicPath = "Audio/Music/";
            MusicPath += LevelMusic.name;

            if( !LoadSound( out Music, MusicPath ) )
            {
                Debug.LogError("NO MUSIC LOADED");
                return false;
            }
        }
        else
        {
            if( !LoadSound( out Music, "Audio/Music/1 - Phat Phrog Studios - Samurai Spirit - EDM Music Collection - Cursed Bushido - LOOP" ) )
            {
                Debug.LogError("NO MUSIC LOADED");
                return false;
            }
        }

        if( !LoadSound( out Idle, "Audio/SFX/ENG_IDLE" ) )
        {
            Debug.Log("NO IDLE SOUND LOADED");
            return false;
        }

        if( !LoadSound( out Medium, "Audio/SFX/ENG_MEDIUM" ) )
        {
            Debug.Log("NO MEDIUM SOUND LOADED");
            return false;
        }

        if( !LoadSound( out Boost, "Audio/SFX/ENG_BOOST" ) )
        {
            Debug.Log("NO BOOST SOUND LOADED");
            return false;
        }

        if( !LoadSound( out TrailUp, "Audio/SFX/TrailUp" ) )
        {
            Debug.Log("NO TRAIL UP SOUND LOADED");
            return false;
        }

        if( !LoadSound( out TrailAudio, "Audio/SFX/TrailAudio" ) )
        {
            Debug.Log("Trail Audio Failed to load");
            return false;
        }

        if( !LoadSound( out AttackSound, "Audio/SFX/ENEMY_ATK" ) )
        {
            Debug.Log("NO ATTACK SOUND LOADED");
            return false;
        }

        if( !LoadSound( out EnemyDestroyed, "Audio/SFX/Destroyed" ) )
        {
            Debug.Log("NO DESTROYED SOUND LOADED");
            return false;
        }

        if( !LoadSound( out EnemyDamaged, "Audio/SFX/HIT_ENEMY" ) )
        {
            Debug.Log("NO HIT ENEMY SOUND LOADED");
            return false;
        }

        if( !LoadSound( out Damaged, "Audio/SFX/HIT_PLAYER" ) )
        {
            Debug.Log("NO HIT PLAYER SOUND LOADED");
            return false;
        }

        if( !LoadSound( out BoostRecharge, "Audio/SFX/PLAYER_BOOST_RECHARGE" ) )
        {
            Debug.Log("NO PLAYER BOOST RECHARGE SOUND LOADED");
            return false;
        }

        if( !LoadSound( out EnemySpawn, "Audio/SFX/ENEMY_SPAWN" ) )
        {
            Debug.Log("NO ENEMY SPAWN SOUND LOADED");
            return false;
        }

        if( !LoadSound( out SeekerAudio, "Audio/SFX/SEEKER_AMBIENCE" ) )
        {
            Debug.Log("NO SEEKER AMBIENCE SOUND LOADED");
            return false;
        }

        if( !LoadSound( out InterceptorAudio, "Audio/SFX/INTERCEPTOR_AMBIENCE" ) )
        {
            Debug.Log("NO INTERCEPTOR AMBIENCE SOUND LOADED");
            return false;
        }

        if( !LoadSound( out HealingAudio, "Audio/SFX/CONSUMABLE_AMIBENCE" ) )
        {
            Debug.Log("NO HEALING SOUND LOADED");
            return false;
        }

        EngineAudio = new Dictionary<EngineStates, AudioClip>()
        {
            { EngineStates.Idle, Idle },
            { EngineStates.Medium, Medium },
            { EngineStates.Boost, Boost }
        };

        //NOTE: Updating Volume values
        UpdateVolumeValues();

        if( !MusicSource ) MusicSource = this.gameObject.AddComponent<AudioSource>();
        if( MusicSource )
        {
            MusicSource.spatialBlend = 0f;
            MusicSource.loop = true;
            MusicSource.clip = Music;
            MusicSource.volume = GetMusicVolume();
            MusicSource.outputAudioMixerGroup = AudioMixer;
            MusicSource.Play();
        }
        else 
        {
            Debug.Log("Failed to create and initialize new music source");
            return false;
        }

        return true;
    }

    public void
    CleanUpAudio()
    {
        MusicSource.Stop();
        Destroy( MusicSource );
        MusicSource = null;
        EngineAudio = null;
    }

    public void
    GetAudioClipToPlay( in EngineStates EngineState, out AudioClip ClipToPlay )
    {
        ClipToPlay = EngineAudio[ EngineState ];
    }


    public void
    GetAudioClipToPlay( EnemyType enemyType, out AudioClip ClipToPlay )
    {
        ClipToPlay = null;
        if( enemyType == EnemyType.Seeker )
        {
            ClipToPlay = SeekerAudio;
        }
        else if(enemyType == EnemyType.Interceptor )
        {
            ClipToPlay = InterceptorAudio;
        }
    }

    public void
    UpdateVolumeValues()
    {
        SettingsManager SM = FindObjectOfType< SettingsManager >();
        if( SM )
        {
            MasterVolume = SM.GetAudio( AudioChannelType.Master );
            MusicVolume = SM.GetAudio( AudioChannelType.Music );
            SFXVolume = SM.GetAudio( AudioChannelType.SFX );
        }
        else
        {
            Debug.LogError("Failed To Retrieve Volume Values in memory: NO SETTINGS MANAGER");
        }
    }

    public float
    GetMusicVolume()
    {
        float Volume = ( MasterVolume / 100f );
        Volume *= ( MusicVolume / 100f );
        return Volume;
    }

    public float
    GetSoundEffectVolume()
    {
        float Volume = ( MasterVolume / 100f );
        Volume *= ( SFXVolume / 100f );
        return Volume;
    }

    public void
    PauseMusic()
    {
        MusicSource.Pause();
    }

    public void
    UnPauseMusic()
    {
        MusicSource.volume = GetMusicVolume();
        MusicSource.UnPause();
    }

    public void
    Restart()
    {
        MusicSource.Play();
    }

    public void
    PlaySpecialEffect( in AudioSource Audio, SpecialEffect EffectType )
    {
        AudioClip clip = null;
        float volume = GetSoundEffectVolume();

        switch( EffectType )
        {
            case SpecialEffect.TrailUp:
                clip = TrailUp;
                break;
            case SpecialEffect.Damaged:
                clip = Damaged;
                break;
            case SpecialEffect.EnemyDamaged:
                clip = EnemyDamaged;
                break;
            case SpecialEffect.Destroyed:
                clip = EnemyDestroyed;
                break;
            case SpecialEffect.AttackSound:
                clip = AttackSound;
                break;
            case SpecialEffect.BoostRecharge:
                clip = BoostRecharge;
                break;
            case SpecialEffect.EnemySpawn:
                clip = EnemySpawn;
                break;
        }

        if( clip ) Audio.PlayOneShot( clip, volume );
    }

    public void
    PlayConsumableAudio( in AudioSource Audio )
    {
        Audio.clip = HealingAudio;
        Audio.loop = true;
        Audio.volume = GetSoundEffectVolume();
        Audio.Play();
    }

    private bool
    LoadSound( out AudioClip SoundToLoad, string path )
    {
        SoundToLoad = Resources.Load( path ) as AudioClip;
        if( !SoundToLoad )
        {
            Debug.Log("Audio file at path: " + path + " failed to load");
            return false;
        }
        return true;
    }

}
