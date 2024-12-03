using UnityEngine;
using UnityEngine.UI;

public class BetterButton : BetterUI
{
    private bool ButtonPressed = false;
    public UnityEngine.UI.Button.ButtonClickedEvent onClick;

    private void
    Update()
    {
        bool ButtonPressedThisFrame = IsPressed();

        if( ButtonPressedThisFrame && !ButtonPressed )
        {
            ButtonPressed = true;
        }
        else if( !ButtonPressedThisFrame && ButtonPressed )
        {
            ButtonPressed = false;
            onClick.Invoke();
        }
    }

    public override void
    Press()
    {
        DoStateTransition( Selectable.SelectionState.Pressed, true );
        onClick.Invoke();
    }

}
