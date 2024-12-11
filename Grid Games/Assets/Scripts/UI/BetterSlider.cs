using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BetterSlider : BetterUI
{

    [SerializeField] private UnityEvent CallingFunction;
    [SerializeField] private Slider slider;
    [SerializeField] private AudioChannelType ChannelType;
    private int CurrentValue;
    private float ResetTimer;
    private float CurrentTimer;
    private bool ControllerConnected;

    private new void
    OnEnable()
    {
        ControllerConnected = false;
        InputHandler Ih = FindObjectOfType<InputHandler>();
        ControllerConnected = Ih.ConnectedController;
        SettingsManager SM = FindObjectOfType<SettingsManager>();
        CurrentValue = SM.GetAudio( ChannelType );
        slider.value = (float)CurrentValue / 100;
    }

    private new void
    OnDisable()
    {
        SettingsManager SM = FindObjectOfType<SettingsManager>();
        SM.SaveAudio( ChannelType, CurrentValue );
        MainMenuAudio MMA = FindObjectOfType<MainMenuAudio>();
        if( MMA ) MMA.UpdateAudioSource();
    }

    private new void
    Start()
    {
        ResetTimer = .2f;
    }

    private void
    Update()
    {
        if( !ControllerConnected ) return;
        slider.value = (float)CurrentValue / 100;
        if( CurrentTimer > 0 )
        {
            CurrentTimer -= Time.deltaTime;
        }
    }

    public override void
    Press()
    {
        CallingFunction.Invoke();
        DoStateTransition( Selectable.SelectionState.Pressed, true );
    }

    public override void
    SelectElement()
    {
        Select();
        slider.Select();
    }

    public override bool
    HandleInput( in GameInputStates userInput, in GameInputStates PreviousUserInput )
    {

        if( CurrentTimer <= 0 )
        {

            if( userInput.dpad.x == -1f )
            {
                CurrentValue -= 5;
            }
            else if( userInput.dpad.x == 1f )
            {
                CurrentValue += 5;
            }

            if( userInput.leftStick.x < -.8f )
            {
                CurrentValue -= 5;
            }
            else if( userInput.leftStick.x > .8f )
            {
                CurrentValue += 5;
            }

            CurrentTimer = ResetTimer;
        }

        if( CurrentValue > 100 )
        {
            CurrentValue = 100;
        }

        if( CurrentValue < 0 )
        {
            CurrentValue = 0;
        }

        if( userInput.buttonStates.south.buttonPressed || userInput.buttonStates.east.buttonPressed )
        {
            return false;
        }

        return true;
    }

    public void
    UpdateCurrentValue()
    {
        CurrentValue = (int)( slider.value * 100 );
    }

}
