using UnityEngine;
using UnityEngine.UI;

public class BetterUI : Selectable
{
    public virtual void
    SelectElement() //NOTE: Should only be overidden when the UI element needs extra functionality
    {
        this.Select();
    }

    public virtual void
    Press()
    {
        Debug.LogFormat( this.gameObject.name + ": Pressed!");
    }

    public virtual bool
    HandleInput( in GameInputStates userInput, in GameInputStates PreviousInput )
    {
        return false;
    }

}
