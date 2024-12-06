using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [Header("User Input")]
    public InputHandler IH;

    [Header("UI Screens")]
    public GameObject PauseMenu;
    public GameObject FailScreen;
    public GameObject SuccessScreen;

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
        if( !FailScreen || !SuccessScreen || !PauseMenu ) return false;

        FailScreen.SetActive( false );
        SuccessScreen.SetActive( false );
        PauseMenu.SetActive( false );

        //TODO: Any other UI Element needs to be created here
        return true;
    }

    public void
    ShowFailScreen()
    {
        FailScreen.SetActive( true );
    }

    public void
    ShowSuccessScreen()
    {
        SuccessScreen.SetActive( true );
    }

    public void
    ShowPauseScreen()
    {
        PauseMenu.SetActive( true );
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
    }

    public void
    UpdateEnemiesCounter()
    {
    }

}
