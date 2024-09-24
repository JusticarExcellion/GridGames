using System;
using UnityEngine;

public struct Button
{
    public bool buttonPressed;
}

public class GameInputStates
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

[Serializable]
public struct VehicleInput
{
    public float acceleration;
    public float steering;
    public float braking;
}

public class GameResources : MonoBehaviour
{}
