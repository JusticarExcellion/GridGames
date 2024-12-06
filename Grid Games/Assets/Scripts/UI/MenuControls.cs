using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
    [Header("Player Input:")]
    private InputHandler PlayerInput;
    [SerializeField] private MenuManager MainMananger;
    [SerializeField] private List<BetterUI> MenuButtons;

    [Header("Menu Control")]
    [SerializeField] private MenuInfo ParentMenu;

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
        if( !PlayerInput )
        {
            PlayerInput = FindObjectOfType< InputHandler >();
            return;
        }
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

        if( userInput.buttonStates.start.buttonPressed )
        { // If Escape is pressed or start is pressed then we back out

            Back();
        }

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
    ContinueGame()
    {
        LevelManager.Instance.Unpause();
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
        Application.Quit();
    }

    public void
    Back()
    {
        if( ParentMenu )
        {
            MainMananger.ChangeToMenu( ParentMenu.MenuIndex );
        }
        else // If no parent then we get the level Manager and unpause the game
        {
            LevelManager.Instance.Unpause();
        }
    }

    public void
    OpenMain()
    {
        MainMananger.ChangeToMenu( MenuManager.MainIndex );
    }

    public void
    OpenOptions()
    {
        MainMananger.ChangeToMenu( MenuManager.OptionsIndex );
    }

    public void
    OpenInstructions()
    {
        MainMananger.ChangeToMenu( MenuManager.InstructionsIndex );
    }

    public void
    OpenCredits()
    {
        MainMananger.ChangeToMenu( MenuManager.CreditsIndex );
    }

    public void
    OpenAudio()
    {
        MainMananger.ChangeToMenu( MenuManager.AudioIndex );
    }

    public void
    OpenGame()
    {
        MainMananger.ChangeToMenu( MenuManager.GameIndex );
    }

    public void
    OpenLevelSelector()
    {
        MainMananger.ChangeToMenu( MenuManager.LevelSelectorIndex );
    }

    public void
    OpenGraphics()
    {
        MainMananger.ChangeToMenu( MenuManager.GraphicIndex );
    }

    public void
    BackToMainMenu()
    {
        //TODO: Get the level manager and tell it direct the scenemangare to load the first level

    }

    public void
    RestartLevel()
    {
        //TODO: Tell level manager to restart the level
        LevelManager.Instance.RestartLevel();
    }

}
