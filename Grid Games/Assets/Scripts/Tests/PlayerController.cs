using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent( typeof(MotorcycleController) ) ]
public class PlayerController : MonoBehaviour
{
    MotorcycleController motorcycleController;

    void
    Start()
    {
        motorcycleController = this.GetComponent<MotorcycleController>();
    }

    void
    Update()
    {
        GameInputStates GameInput;
        GetInput( out GameInput );
        VehicleInput VI = new VehicleInput();

        //NOTE: Acceleration
        VI.acceleration = GameInput.leftStick.y;

        if( GameInput.rightTrigger != 0.0f )
        {
            VI.acceleration = GameInput.rightTrigger;
        }

        VI.steering = GameInput.leftStick.x;

        VI.braking = GameInput.leftTrigger;

        motorcycleController.VInput = VI;
    }

    void
    GetInput( out GameInputStates gameInput )
    {

        gameInput = new GameInputStates();

        //TODO: We need to know if the gamepad is connected then get the keyboard input if it isn't

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
}
