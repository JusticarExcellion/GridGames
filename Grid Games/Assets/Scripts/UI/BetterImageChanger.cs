using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BetterImageChanger : BetterUI
{
    [Header("Instruction Slideshow Images:")]
    [SerializeField] private List<Sprite> InstructionImages;

    [Header("Activate Element:")]
    [SerializeField] private UnityEvent ActivateFunction;

    private Image InstructionImage;
    private int CurrentInstruction;
    private float CurrentTimer;
    private float ResetTimer;

    private new void
    OnEnable()
    {
        CurrentInstruction = 0;
        CurrentTimer = 0f;
    }

    private new void
    Start()
    {
        CurrentInstruction = 0;
        InstructionImage = this.GetComponent<Image>();
        CurrentTimer = 0f;
        ResetTimer = .4f;
    }

    private void
    Update()
    {
        if( CurrentTimer > 0 )
        {
            CurrentTimer -= Time.deltaTime;
        }
    }

    public override void
    SelectElement()
    {
        this.Select();
        ActivateFunction.Invoke();
        Debug.Log( this.gameObject.name + ": Activated!!!");
    }

    public override bool
    HandleInput( in GameInputStates userInput, in GameInputStates PreviousInput )
    {
        //DEBUG:
        int PreviousInstruction = CurrentInstruction;

        if( userInput.dpad.y == -1f && ( userInput.dpad.y != PreviousInput.dpad.y ) ||  ( ( userInput.leftStick.y < -.8f && PreviousInput.leftStick.y > -.8f ) || ( userInput.leftStick.y > .8f && PreviousInput.leftStick.y < .8f) ) )
        {
            //NOTE: Bug where the Image changer is still active but the next element is selected, has to do with the menu control input being handled or selecting the next option, only occurs when moving left stick to the left
            return false;
        }

        if( CurrentTimer <= 0 )
        {
            if( userInput.dpad.x == -1f )
            {
                CurrentInstruction--;
            }
            else if( userInput.dpad.x == 1f )
            {
                CurrentInstruction++;
            }

            if( userInput.leftStick.x < -.8f )
            {
                CurrentInstruction--;
            }
            else if( userInput.leftStick.x > .8f )
            {
                CurrentInstruction++;
            }

            CurrentTimer = ResetTimer;
        }

        if( PreviousInstruction != CurrentInstruction )
        {
            Debug.Log("Current Instruction: " + CurrentInstruction );
        }

        return true;
    }
}
