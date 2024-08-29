using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MovementInput
{
    public   float horizontalInput;
    public   Vector2 lookInputs;
    public   float acceleration;
};

public class InputSystem_test : MonoBehaviour
{
    PlayerControls PlayerControls;
    PlayerMovement playerMovement;
    MovementInput PlayerMovementInput = new MovementInput();
    //TODO: We need to get rid of these, and just handle all of these cases with the standard input system, so we are not creating these various layers of indirection
    bool boost;
    bool activate;
    bool use;
    bool pause;
    bool brake;

    void Start()
    {
        playerMovement = this.GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        if( PlayerControls == null )
        {
            PlayerControls = new PlayerControls();
            PlayerControls.SampleControls.Movement.performed += i => PlayerMovementInput.horizontalInput = i.ReadValue<float>();
            PlayerControls.SampleControls.Accelerate.performed += i => PlayerMovementInput.acceleration = i.ReadValue<float>();
            PlayerControls.SampleControls.Look.performed += i => PlayerMovementInput.lookInputs = i.ReadValue<Vector2>();
            //TODO: Get Rid of these
            PlayerControls.SampleControls.Boost.started += i => boost = true;
            PlayerControls.SampleControls.Activate.started += i => activate = true;
            PlayerControls.SampleControls.Use.started += i => use = true;
            PlayerControls.SampleControls.Pause.started += i => pause = true;
            PlayerControls.SampleControls.Brake.performed += i => brake = true;
            PlayerControls.SampleControls.Brake.canceled += i => brake = false;

        }

        PlayerControls.Enable();
    }

    void OnDisable()
    {
        PlayerControls.Disable();
    }

    public MovementInput GetMovementInput()
    {
        return PlayerMovementInput; //Does this pass the reference or the values?
    }

    void Update(){

        //TODO: Get rid of these
        if( boost )
        {
            boost = false;
            playerMovement.Boost();
        }

        if( activate )
        {
            activate = false;
            playerMovement.Activate();
        }

        if( use )
        {
            use = false;
            playerMovement.Use();
        }

        if( pause )
        {
            pause = false;
            playerMovement.Pause();
        }

        if( brake )
        {
            playerMovement.HandleBraking();
        }
        else
        {
            playerMovement.ResetBreaking();
        }

    }

}
