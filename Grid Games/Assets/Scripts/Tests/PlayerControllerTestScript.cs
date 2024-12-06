using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTestScript : MonoBehaviour
{

    public InputHandler IH;

    private void
    Start()
    {
        IH = FindObjectOfType<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        GameInputStates userInput = IH.GetInput();
        if( userInput.buttonStates.start.buttonPressed && !LevelManager.Instance.Paused )
        {
            LevelManager.Instance.Pause();
        }
    }
}
