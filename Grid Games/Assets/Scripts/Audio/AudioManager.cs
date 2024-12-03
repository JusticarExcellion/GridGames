using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EngineStates
{
    Idle,
    Slow,
    Medium,
    Fast,
    None
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip Idle;
    [SerializeField] private AudioClip Slow;
    [SerializeField] private AudioClip Medium;
    [SerializeField] private AudioClip Fast;

    private Dictionary<EngineStates, AudioClip> EngineAudio;

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
        InitializeAudio();
    }

    public bool
    InitializeAudio()
    {
        EngineAudio = new Dictionary<EngineStates, AudioClip>()
        {
            { EngineStates.Idle, Idle },
            { EngineStates.Slow, Slow },
            { EngineStates.Medium, Medium },
            { EngineStates.Fast, Fast }
        };

        return true;
    }

    public void
    CleanUpAudio()
    {
        EngineAudio = null;
    }

    public void
    GetAudioClipToPlay( in EngineStates EngineState, out AudioClip ClipToPlay )
    {
        ClipToPlay = EngineAudio[ EngineState ];
    }

}
