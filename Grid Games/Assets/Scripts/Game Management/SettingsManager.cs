using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioChannelType
{
    Master,
    Music,
    SFX
}

// NOTE: If we are using more than one type of dropdown pivot to using this
  public enum DropdownType
  {
       Difficulty,
       GraphicLevel
  }

public class SettingsManager : MonoBehaviour
{

    //NOTE: Player Preference Key Names
    public const string Master = "MasterAudio";
    public const string Music = "MusicAudio";
    public const string SFX = "SFXAudio";

    public const string Difficulty = "Difficulty";
    public const string GraphicalLevel = "GraphicalLevel";
    //TODO: Other Options key names go here

    public bool
    SaveAudio( AudioChannelType channel, int value )
    {
        if( value < 0 || value > 100 )
        {
            return false;
        }

        string keyName = "";
        switch( channel )
        {
            case AudioChannelType.Master:
                keyName = Master;
                break;
            case AudioChannelType.Music:
                keyName = Music;
                break;
            case AudioChannelType.SFX:
                keyName = SFX;
                break;
        }

        Debug.Log( "Saving Audio Channel: " + keyName + ", value = " + value );

        PlayerPrefs.SetInt( keyName, value );
        PlayerPrefs.Save();
        return true;
    }

    public void
    SaveDropdown( int value, DropdownType type )
    {
        if( type == DropdownType.Difficulty )
        {
            SaveDifficulty( value );
        }
        else if( type == DropdownType.GraphicLevel )
        {
            SaveGraphicalLevel( value );
        }
    }

    private bool
    SaveDifficulty( int value )
    {
        if( value < 0 )
        {
            return false;
        }
        Debug.Log( "Saving Difficulty: value = " + value );
        PlayerPrefs.SetInt( Difficulty, value );
        PlayerPrefs.Save();
        return true;
    }

    public int
    GetDifficulty()
    {
        if( PlayerPrefs.HasKey( Difficulty ) )
        {
            int difficultyLevel = PlayerPrefs.GetInt( Difficulty );
            Debug.Log("Data Found: Difficulty = " + difficultyLevel);
            return difficultyLevel;
        }
        else
        {
            Debug.Log("No Saved Data. Setting Defaults...");
            PlayerPrefs.SetInt( Difficulty , 1 );
            return 1;
        }
    }

    private bool
    SaveGraphicalLevel( int value )
    {
        if( value < 0 )
        {
            return false;
        }
        Debug.Log( "Saving Graphical Level: value = " + value );
        PlayerPrefs.SetInt( GraphicalLevel, value );
        PlayerPrefs.Save();
        return true;
    }

    public int
    GetGraphicalLevel()
    {
        if( PlayerPrefs.HasKey( GraphicalLevel ) )
        {
            int GraphicsValue = PlayerPrefs.GetInt( GraphicalLevel );
            Debug.Log("Data Found: Difficulty = " + GraphicsValue );
            return GraphicsValue;
        }
        else
        { //TODO: We may need to change this default setting
            Debug.Log("No Saved Data. Setting Defaults...");
            PlayerPrefs.SetInt( GraphicalLevel , 1 );
            return 1;
        }
    }


    public int
    GetAudio( AudioChannelType channel )
    {
        string keyName = "";
        switch( channel )
        {
            case AudioChannelType.Master:
                keyName = Master;
                break;
            case AudioChannelType.Music:
                keyName = Music;
                break;
            case AudioChannelType.SFX:
                keyName = SFX;
                break;
        }

        if( PlayerPrefs.HasKey( keyName ) )
        {
            int audioLevel = PlayerPrefs.GetInt( keyName );
            Debug.Log("Data Found: " + keyName + " = " + audioLevel );
            return audioLevel;
        }
        else
        {
            Debug.Log("No Saved Data. Setting Defaults...");
            PlayerPrefs.SetInt( keyName , 50 );
            return 50;
        }
    }

}
