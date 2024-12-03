using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private List<GameObject> Menus;
    private int CurrentSelectedMenu;

    private void
    Start()
    {
        CurrentSelectedMenu = 0;
        ChangeToMenu( CurrentSelectedMenu );
    }

    public void
    ChangeToMenu( int MenuNumber )
    {
        Menus[ CurrentSelectedMenu ].SetActive( false );
        Menus[ MenuNumber ].SetActive( true );
        CurrentSelectedMenu = MenuNumber;
    }

}
