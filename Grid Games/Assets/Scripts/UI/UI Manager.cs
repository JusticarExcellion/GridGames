using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    [Header("User Input")]
    public InputHandler IH;

    [Header("UI Screens")]
    public GameObject PauseMenu;
    public GameObject FailScreen;
    public GameObject SuccessScreen;
    public ConsumableSlot ConsumableUI;

    private void
    Awake()
    {
        if( Instance == null )
        {
            Instance = this;
        }
        else if( Instance != this )
        {
            Destroy( this );
        }
        DontDestroyOnLoad( this );
    }

    private void
    Start()
    {
        IH = FindObjectOfType< InputHandler >();
    }

    //NOTE: We need all of the UI menus to already be in the scene or to initialize the canvas and all of the menus
    public bool
    InitializeUI()
    {

        GameObject[] gos = GameObject.FindGameObjectsWithTag( "Pause" );
        if( gos.Length > 1 || gos.Length == 0 ) return false;
        PauseMenu = gos[0];

        GameObject[] EndScreens = GameObject.FindGameObjectsWithTag( "EndScreens" );
        foreach( GameObject go in EndScreens )
        {
            if( go.name == "SuccessScreen" )
            {
                SuccessScreen = go;
            }
            else if( go.name == "FailScreen" )
            {
                FailScreen = go;
            }

        }

        ConsumableUI = FindObjectOfType< ConsumableSlot >();
        if( !FailScreen || !SuccessScreen || !PauseMenu ) return false;

        FailScreen.SetActive( false );
        SuccessScreen.SetActive( false );
        PauseMenu.SetActive( false );
        ConsumableUI.Deactivate();

        //TODO: Any other UI Element needs to be created here
        return true;
    }

    public void
    ShowFailScreen( GameStatistics gameStats )
    {
        FailScreen.SetActive( true );
        SuccessScreen.SetActive( false );
        ConsumableUI.gameObject.SetActive( false );
        EndScreen endScreen = FailScreen.GetComponent<EndScreen>();
        endScreen.DisplayStats( in gameStats );
    }

    public void
    ShowSuccessScreen( GameStatistics gameStats )
    {
        SuccessScreen.SetActive( true );
        FailScreen.SetActive( false );
        ConsumableUI.gameObject.SetActive( false );
        EndScreen endScreen = SuccessScreen.GetComponent<EndScreen>();
        endScreen.DisplayStats( in gameStats );
    }

    public void
    Restart()
    {
        FailScreen.SetActive( false );
        SuccessScreen.SetActive( false );
        Debug.Log("UI Restart");
    }

    public void
    ShowPauseScreen()
    {
        PauseMenu.SetActive( true );
        ConsumableUI.gameObject.SetActive( false );
        MenuManager menu =  PauseMenu.GetComponent< MenuManager >();
        if( !menu )
        {
            Debug.Log("No Menu Manager ");
            return;
        }
        menu.RestartMenu();
    }

    public void
    HidePauseScreen()
    {
        PauseMenu.SetActive( false );
        ConsumableUI.gameObject.SetActive( true );
    }

    public void
    ShowConsumable()
    {
        ConsumableUI.Activate();
    }

    public void
    HideConsumable()
    {
        ConsumableUI.Deactivate();
    }
}
