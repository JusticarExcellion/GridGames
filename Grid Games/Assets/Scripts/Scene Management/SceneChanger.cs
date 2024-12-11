using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static int MainMenuIndex = 0;
    public static int ArenaOneIndex = 1;

    public void
    LoadLevel( int LevelIndex )
    {
        Debug.Log("Load Scene: " + LevelIndex);
        SettingsManager SM = FindObjectOfType< SettingsManager >();
        int GraphicalLevel = SM.GetGraphicalLevel();
        string qualityLevelName = "";
        switch( GraphicalLevel )
        {
            case 0:
                qualityLevelName = "Low";
                break;
            case 1:
                qualityLevelName = "Medium";
                break;
            case 2:
                qualityLevelName = "High";
                break;
        }

        string[] names = QualitySettings.names;
        for( int i = 0; i < names.Length; i++ )
        {
            if( names[i] == qualityLevelName && QualitySettings.GetQualityLevel() != i )
            {//NOTE: We only want to change our quality setting if we've changed it
                QualitySettings.SetQualityLevel( i, true );
                Debug.Log("Setting Quality Level To: " + qualityLevelName );
            }
        }

        SceneManager.LoadScene( LevelIndex );
    }
}
