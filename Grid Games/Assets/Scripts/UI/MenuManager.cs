using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuManager : MonoBehaviour
{
    [Header("Menus:")]
    [SerializeField] private List<GameObject> MenuList;
    private SceneChanger SC;
    private int CurrentSelectedMenu;

    public static int MainIndex = 0;
    public static int LevelSelectorIndex = 1;
    public static int OptionsIndex = 2;
    public static int AudioIndex = 3;
    public static int GraphicIndex = 4;
    public static int GameIndex = 5;
    public static int InstructionsIndex = 6;
    public static int CreditsIndex = 7;

    private void
    Start()
    {
        SC = FindObjectOfType< SceneChanger >();
        RestartMenu();
    }

    public void
    RestartMenu()
    {
        foreach( GameObject Menu in MenuList )
        {
            if( Menu ) Menu.SetActive( false );
        }

        CurrentSelectedMenu = 0;
        if( MenuList.Count > 0) MenuList[ CurrentSelectedMenu ].SetActive( true );
    }

    public void
    ChangeToMenu( int index )
    {
        if( MenuList.Count > 0 )
        {
            MenuList[ CurrentSelectedMenu ].SetActive( false );
            MenuList[ index ].SetActive( true );
            CurrentSelectedMenu = index;
        }
        else
        {
            Debug.Log( "Menu is empty" );
        }
    }
}
