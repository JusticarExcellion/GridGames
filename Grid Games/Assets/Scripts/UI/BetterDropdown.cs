using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class BetterDropdown : BetterUI
{

    [SerializeField] private UnityEvent CallingFunction;
    [SerializeField] private TMP_Dropdown dropdown;
    private int value; // TODO: Change this so it shows the right value that's saved locally, this should ask a singleton what it's value is
    private int StartingValue;
    private float AnalogToleranceLevel;
    private int DropdownCount;

    private new void
    OnEnable()
    {
        SettingsManager SM = FindObjectOfType<SettingsManager>();
        value = SM.GetDifficulty();
    }

    private new void
    OnDisable()
    {
        SettingsManager SM = FindObjectOfType<SettingsManager>();
        SM.SaveDifficulty( value );
    }

    private new void
    Start()
    {
        value = 0;
        AnalogToleranceLevel = MenuControls.AnalogToleranceLevel;
        DropdownCount = dropdown.options.Count;
    }

    public override void
    Press()
    {
        DoStateTransition( Selectable.SelectionState.Pressed, true );
        CallingFunction.Invoke();
        dropdown.Show();
        StartingValue = value;
    }

    private void
    Update()
    {
        dropdown.value = value;
        dropdown.RefreshShownValue();
    }

    public override bool
    HandleInput( in GameInputStates userInput, in GameInputStates PreviousUserInput )
    {
        if( userInput.buttonStates.east.buttonPressed )
        {
            CancelSelection();
            return false;
        }

        if( PreviousUserInput.dpad.y != userInput.dpad.y )
        {
            if( userInput.dpad.y == -1f )
            {
                value++;
            }
            else if( userInput.dpad.y == 1f )
            {

                value--;
            }
        }
 
        if( ( userInput.leftStick.y - PreviousUserInput.leftStick.y ) > AnalogToleranceLevel || ( userInput.leftStick.y - PreviousUserInput.leftStick.y ) < -AnalogToleranceLevel )
        {
            if( userInput.leftStick.y < -.8f )
            {
                value++;
            }
            else if( userInput.leftStick.y > .8f )
            {

                value--;
            }
        }

        if( value < 0 )
        {
            value = DropdownCount - 1;
        }

        if( value >= DropdownCount)
        {
            value = 0;
        }

        if( userInput.buttonStates.south.buttonPressed )
        {
            ChooseSelection();
            return false;
        }

        return true;
    }

    private void
    CancelSelection()
    {
        value = StartingValue;
        dropdown.Hide();
    }

    private void
    ChooseSelection()
    {
        dropdown.Hide();
    }

}
