using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BetterLevelSelector : BetterUI
{
    [Header("Level Images:")]
    [SerializeField] private List<SceneSelectorObject> LevelSelectorObjects;

    [Header("Connected Components")]
    [SerializeField] private Image PreviousSelector;
    [SerializeField] private Image MainSelected;
    [SerializeField] private Image NextSelector;
    [SerializeField] private GameObject LeftArrow;
    [SerializeField] private GameObject RightArrow;
    [SerializeField] private SceneChanger SC;

    [Header("Activate Element:")]
    [SerializeField] private UnityEvent ActivateFunction;

    private int CurrentSelectedLevel;
    private int LevelCount;
    private float CurrentTimer;
    private float ResetTimer;

    private new void Start()
    {
        CurrentSelectedLevel = 0;
        CurrentTimer = 0f;
        ResetTimer = .4f;
        LevelCount = LevelSelectorObjects.Count;
        UpdateImages();
    }

    private new void
    OnEnable()
    {
        CurrentSelectedLevel = 0;
        CurrentTimer = 0f;
        UpdateImages();
    }

    void Update()
    {
        if( CurrentTimer > 0 )
        {
            CurrentTimer -= Time.deltaTime;
        }
    }

    public override void
    SelectElement()
    {
        this.Select();
        ActivateFunction.Invoke();
        Debug.Log( this.gameObject.name + ": Activated!!!");
    }

    public override void
    Press()
    {
        SC.LoadLevel( LevelSelectorObjects[ CurrentSelectedLevel ].LevelIndex );
    }

    private void
    UpdateImages()
    {
        if( CurrentSelectedLevel > 0 )
        {
            PreviousSelector.sprite = LevelSelectorObjects[ CurrentSelectedLevel - 1 ].LevelImage;
            PreviousSelector.gameObject.SetActive( true );
            LeftArrow.SetActive( true );
        }
        else
        {
            PreviousSelector.gameObject.SetActive( false );
            LeftArrow.SetActive( false );
        }

        MainSelected.sprite = LevelSelectorObjects[ CurrentSelectedLevel ].LevelImage;

        if( CurrentSelectedLevel <  LevelCount - 1 )
        {
            NextSelector.sprite = LevelSelectorObjects[ CurrentSelectedLevel + 1 ].LevelImage;
            NextSelector.gameObject.SetActive( true );
            RightArrow.SetActive( true );
        }
        else
        {
            NextSelector.gameObject.SetActive( false );
            RightArrow.SetActive( false );
        }
    }

    public override bool
    HandleInput( in GameInputStates userInput, in GameInputStates PreviousInput )
    {
        //DEBUG:
        int PreviousSelectedLevel = CurrentSelectedLevel;

        if( userInput.buttonStates.south.buttonPressed )
        {
            Press();
        }

        if( userInput.dpad.y == -1f && ( userInput.dpad.y != PreviousInput.dpad.y ) ||  ( ( userInput.leftStick.y < -.8f && PreviousInput.leftStick.y > -.8f ) || ( userInput.leftStick.y > .8f && PreviousInput.leftStick.y < .8f) ) )
        {
            return false;
        }

        if( CurrentTimer <= 0 )
        {
            if( userInput.dpad.x == -1f )
            {
                CurrentSelectedLevel--;
            }
            else if( userInput.dpad.x == 1f )
            {
                CurrentSelectedLevel++;
            }

            if( userInput.leftStick.x < -.8f )
            {
                CurrentSelectedLevel--;
            }
            else if( userInput.leftStick.x > .8f )
            {
                CurrentSelectedLevel++;
            }

            CurrentTimer = ResetTimer;
        }

        //Clamping
        if( CurrentSelectedLevel > LevelCount - 1)
        {
            CurrentSelectedLevel = LevelCount - 1;
        }

        if( CurrentSelectedLevel < 0)
        {
            CurrentSelectedLevel = 0;
        }

        if( PreviousSelectedLevel != CurrentSelectedLevel )
        {
            UpdateImages();
        }

        return true;
    }

    public void
    IncreaseLevelCount()
    {
        CurrentSelectedLevel++;
        if( CurrentSelectedLevel > LevelCount - 1)
        {
            CurrentSelectedLevel = LevelCount - 1;
        }
        UpdateImages();
    }

    public void
    DecreaseLevelCount()
    {
        CurrentSelectedLevel--;
        if( CurrentSelectedLevel < 0)
        {
            CurrentSelectedLevel = 0;
        }
        UpdateImages();
    }
}
