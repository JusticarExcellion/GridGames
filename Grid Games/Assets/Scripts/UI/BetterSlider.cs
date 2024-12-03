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

    private new void
    OnEnable()
    {
        SettingsManager SM = FindObjectOfType<SettingsManager>();
        CurrentValue = SM.GetAudio( ChannelType );
    }

    private new void
    OnDisable()
    {
        SettingsManager SM = FindObjectOfType<SettingsManager>();
        SM.SaveAudio( ChannelType, CurrentValue );
    }

    private new void
    Start()
    {
        //TODO: Set values to the saved values
        ResetTimer = .2f;
    }

    private void
    Update()
    {
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

}
