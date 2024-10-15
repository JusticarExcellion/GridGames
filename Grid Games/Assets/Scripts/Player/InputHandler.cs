using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [HideInInspector]public bool ConnectedController = false; //TODO: This is public so we can notify our camera movement script whether or not to activate rotation smoothing because the player is using a mouse instead of an analog stick
    //private Vector2 MousePosition; May Need this to get the mouse delta and smooth teh input

    void
    Awake()
    {
        StartCoroutine( CheckForControllers() );
        //MousePosition = new Vector2();
    }

    private void
    GetInput_Controller( out GameInputStates gameInput )
    {

        gameInput = new GameInputStates();

        //NOTE: Gamepad buttons
        gameInput.buttonStates.north.buttonPressed = Gamepad.current.buttonNorth.wasPressedThisFrame;
        gameInput.buttonStates.south.buttonPressed = Gamepad.current.buttonSouth.wasPressedThisFrame;
        gameInput.buttonStates.west.buttonPressed = Gamepad.current.buttonWest.wasPressedThisFrame;
        gameInput.buttonStates.east.buttonPressed = Gamepad.current.buttonEast.wasPressedThisFrame;

        //NOTE: DPAD
        gameInput.dpad = Gamepad.current.dpad.ReadValue();

        //NOTE: Analog Sticks
        gameInput.leftStick = Gamepad.current.leftStick.ReadValue();
        gameInput.rightStick = Gamepad.current.rightStick.ReadValue();

        //NOTE: Triggers
        gameInput.leftTrigger = Gamepad.current.leftTrigger.ReadValue();
        gameInput.rightTrigger = Gamepad.current.rightTrigger.ReadValue();

        //NOTE: Start
        gameInput.buttonStates.start.buttonPressed = Gamepad.current.startButton.wasPressedThisFrame;
    }

    private void
    GetInput_Keyboard( out GameInputStates gameInput ) // TODO: This may need to be looked at and tweaked
    {
        gameInput = new GameInputStates();

        //NOTE: Gamepad buttons
        gameInput.buttonStates.north.buttonPressed = Input.GetKeyDown( KeyCode.Z );
        gameInput.buttonStates.south.buttonPressed = Input.GetKeyDown( KeyCode.Space );
        gameInput.buttonStates.west.buttonPressed = Input.GetKeyDown( KeyCode.Q );
        gameInput.buttonStates.east.buttonPressed = Input.GetKeyDown( KeyCode.E );

        //NOTE: Fake DPAD
        Vector2 dpad = new Vector2();
        if( Input.GetKey( KeyCode.Keypad1 ) )
        {
            dpad.x = 1.0f;
        }
        else if( Input.GetKey( KeyCode.Keypad2 ) )
        {
            dpad.x = -1.0f;
        }
        else
        {
            dpad.x = 0.0f;
        }

        if( Input.GetKey( KeyCode.Keypad3 ) )
        {
            dpad.y = 1.0f;
        }
        else if( Input.GetKey( KeyCode.Keypad4 ) )
        {
            dpad.y = -1.0f;
        }
        else
        {
            dpad.y = 0.0f;
        }

        gameInput.dpad = dpad;

        //NOTE: Fake Analog Sticks
        Vector2 leftStick = new Vector2();
        if( Input.GetKey( KeyCode.D ) )
        {
            leftStick.x = 1.0f;
        }
        else if( Input.GetKey( KeyCode.A ) )
        {
            leftStick.x = -1.0f;
        }
        else
        {
            leftStick.x = 0.0f;
        }

        if( Input.GetKey( KeyCode.W ) )
        {
            leftStick.y = 1.0f;
        }
        else if( Input.GetKey( KeyCode.S ) )
        {
            leftStick.y = -1.0f;
        }
        else
        {
            leftStick.y = 0.0f;
        }

        gameInput.leftStick = leftStick;

        Vector2 currentMousePosition = new Vector2();
        currentMousePosition.x = Input.mousePosition.x;
        currentMousePosition.y = Input.mousePosition.y;

        gameInput.rightStick = currentMousePosition;


        float leftTrigger = 0.0f;
        if( Input.GetKey( KeyCode.LeftShift ) )
        {
            leftTrigger = 1.0f;
        }
        else
        {
            leftTrigger = 0.0f;
        }


        //NOTE: Triggers
        gameInput.leftTrigger = leftTrigger;
        gameInput.rightTrigger = 0.0f;

        //NOTE: Start
        gameInput.buttonStates.start.buttonPressed = Input.GetKeyDown( KeyCode.Escape );

    }

    public VehicleInput
    GetVehicleInput()
    {
        GameInputStates GameInput;
        if( ConnectedController ) GetInput_Controller( out GameInput );
        else GetInput_Keyboard( out GameInput );
        VehicleInput Result = new VehicleInput();

        //NOTE: Acceleration
        Result.acceleration = GameInput.leftStick.y;

        if( GameInput.rightTrigger != 0.0f )
        {
            Result.acceleration = GameInput.rightTrigger;
        }

        Result.steering = GameInput.leftStick.x;

        Result.braking = GameInput.leftTrigger;

        return Result;
    }

    public GameInputStates
    GetInput()
    {
        GameInputStates Result;
        if( ConnectedController ) GetInput_Controller( out Result );
        else GetInput_Keyboard( out Result );
        return Result;
    }

    IEnumerator CheckForControllers()
    {

        while (true)
        {
            var controllers = Input.GetJoystickNames();

            if( !ConnectedController && controllers.Length > 0 )
            {
                foreach(string controller in controllers)
                {
                    if( !controller.Contains("Virtual") )//TODO: There has to be a better way to do this, this is very error prone
                    {
                        ConnectedController = true;
                        Debug.Log("Controller Name: " + controller + " Connected!");
                    }
                }
            }
            else if( ConnectedController && controllers.Length == 0 )
            {
                ConnectedController = false;
                Debug.Log("Controller Disconnected!");
            }
            yield return new WaitForSeconds(1f);
        }

    }

}
