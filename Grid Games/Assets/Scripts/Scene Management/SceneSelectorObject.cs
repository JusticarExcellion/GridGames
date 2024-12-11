using UnityEngine;

[CreateAssetMenu(fileName= "Data", menuName = "ScriptableObjects/Level Object", order = 1)]
public class SceneSelectorObject : ScriptableObject
{
    public int LevelIndex;
    public Sprite LevelImage;
}
