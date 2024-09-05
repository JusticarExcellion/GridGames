using UnityEngine;
using UnityEngine.InputSystem;

public struct Button
{
    public bool buttonPressed;
}

public struct GameInputStates
{
    public struct ButtonStates
    {
        public Button north;
        public Button south;
        public Button east;
        public Button west;

        public Button start;
    };

    public ButtonStates buttonStates;
    public Vector2 leftStick;
    public Vector2 rightStick;
    public Vector2 dpad;

    public float leftTrigger;
    public float rightTrigger;
}

public struct VehicleInput
{
    public float acceleration;
    public float steering;
    public float braking;
}

public class GameResources : MonoBehaviour
{}
