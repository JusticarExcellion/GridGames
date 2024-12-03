using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
    [Header("Player Input:")]
    [SerializeField] private InputHandler PlayerInput;
    [SerializeField] private MainMenuManager MainMananger;
    [SerializeField] private List<BetterUI> MenuButtons;

    private int MenuCount;
    private bool ElementActive;
    private int CurrentSelectedOption;
    private GameInputStates PreviousUserInput;
    public static readonly float AnalogToleranceLevel = .2f;
    private float CurrentTimer;
    private float ResetTimer;

    private void
    OnEnable()
    {
        CurrentSelectedOption = 0;
        CurrentTimer = 0f;
    }

    void Start()
    {
        ElementActive = false;
        PreviousUserInput = new GameInputStates();
        MenuCount = MenuButtons.Count;
        MenuButtons[ CurrentSelectedOption ].Select();
        CurrentTimer = 0f;
        ResetTimer = .1f;
    }


    void Update()
    {
        GameInputStates userInput = PlayerInput.GetInput();
        HandleMenuInput( in userInput );
        PreviousUserInput = userInput;

        if( CurrentTimer > 0 )
        {
            CurrentTimer -= Time.deltaTime;
        }
    }

    private void
    HandleMenuInput( in GameInputStates userInput )
    {


        if( ElementActive )
        {
            ElementActive = MenuButtons[ CurrentSelectedOption ].HandleInput( in userInput, in PreviousUserInput );
            return;
        }

        //TODO: this is if a controller is already connected, then we handle which menu item is selected and performing the on click functions associated with the menu item
        if( CurrentTimer <= 0 )
        {

            if( userInput.dpad.y == -1f )
            {
                CurrentSelectedOption++;
            }
            else if( userInput.dpad.y == 1f )
            {
                CurrentSelectedOption--;
            }

            if( userInput.leftStick.y < -.8f )
            {
                CurrentSelectedOption++;
            }
            else if( userInput.leftStick.y > .8f )
            {
                CurrentSelectedOption--;
            }

            CurrentTimer = ResetTimer;
        }


        if( CurrentSelectedOption >= MenuCount )
        {
            CurrentSelectedOption = 0;
        }

        if( CurrentSelectedOption < 0)
        {
            CurrentSelectedOption = MenuCount - 1;
        }

        if( userInput.buttonStates.south.buttonPressed && (userInput.buttonStates.south.buttonPressed != PreviousUserInput.buttonStates.south.buttonPressed) )
        {
            MenuButtons[ CurrentSelectedOption ].Press();
        }

        MenuButtons[ CurrentSelectedOption ].SelectElement();
    }

    public void
    ActivateElement()
    {
        ElementActive = true;
    }

    public void
    DeactivateElement()
    {
        ElementActive = false;
    }

    public void
    ExitGame()
    {
        Debug.Log("Exiting...");
        Debug.Break();
    }

    public void
    OpenOptions()
    {
        MainMananger.ChangeToMenu( 2 );
        Debug.Log("Opening Options...");
    }

    public void
    BackToMainMenu()
    {
        MainMananger.ChangeToMenu( 0 );
        Debug.Log("Back To Main...");
    }

    public void
    ContinueGame()
    {
        Debug.Log("Continue Game...");
    }

    public void
    SelectLevel( int levelNumber )
    {
        Debug.LogFormat("Loading Level: {i}", levelNumber );
    }

    public void
    OpenSelectLevel()
    {
        MainMananger.ChangeToMenu( 1 );
        Debug.Log("Open Select Level");
    }

    public void
    OpenAudio()
    {
        MainMananger.ChangeToMenu( 3 );
        Debug.Log("Open Audio Options");
    }

    public void
    OpenGraphics()
    {
        MainMananger.ChangeToMenu( 4 );
        Debug.Log("Open Graphics Options");
    }

    public void
    OpenGame()
    {
        MainMananger.ChangeToMenu( 5 );
        Debug.Log("Open Game Options");
    }

    public void
    OpenInstructions()
    {
        MainMananger.ChangeToMenu( 6 );
        Debug.Log("Opening Instructions");
    }

    public void
    OpenCredits()
    {
        MainMananger.ChangeToMenu( 7 );
        Debug.Log("Opening Credits");
    }

}
