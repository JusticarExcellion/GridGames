using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class BetterDropdown : BetterUI
{

    [SerializeField] private UnityEvent CallingFunction;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private DropdownType type;
    private int value;
    private int StartingValue;
    private float AnalogToleranceLevel;
    private int DropdownCount;

    private new void
    OnEnable()
    {
        SettingsManager SM = FindObjectOfType<SettingsManager>();
        if( type == DropdownType.Difficulty )
        {
            value = SM.GetDifficulty();
        }
        else if( type == DropdownType.GraphicLevel )
        {
            value = SM.GetGraphicalLevel();
        }
    }

    private new void
    OnDisable()
    {
        SettingsManager SM = FindObjectOfType<SettingsManager>();
        if( SM ) SM.SaveDropdown( value, type );
        else
        {
            Debug.Log("NO SETTINGS MANAGER FOUND!!!");
        }
    }

    private new void
    Start()
    {
        AnalogToleranceLevel = MenuControls.AnalogToleranceLevel;
        DropdownCount = dropdown.options.Count;
        dropdown.onValueChanged.AddListener( delegate { SetValueTo( dropdown ); } );

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

    private void
    SetValueTo( TMP_Dropdown myDropdown )
    {
        value = myDropdown.value;
    }

}
