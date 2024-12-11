using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text ScreenText;
    public void
    DisplayStats( in GameStatistics gameStats )
    {
        string text = "Statistics\n\n";
        text += "Enemies Destroyed: " + gameStats.EnemiesDestroyed + "\n\n";
        text += "Times Boosted: " + gameStats.TimesBoosted + "\n\n";
        text += "Activated Light Trail: " + gameStats.LightTrailActivated + " Times\n\n";
        text += "Light Trail Active for: " + gameStats.ActiveTrailTime + " Seconds\n\n";
        text += "Consumables Used: " + gameStats.ConsumablesUsed + "\n\n";
        ScreenText.text = text;
    }
}
