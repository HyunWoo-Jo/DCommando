using UnityEngine;

[CreateAssetMenu(fileName = "ExpConfig", menuName = "Config/Exp")]
public class SO_ExpConfig : ScriptableObject
{
    public int startingLevel = 1;
    public int startingExp = 0;
    public int maxLevel = 100;
    public int baseExpPerLevel = 100;
    public int expIncreasePerLevel = 50;
}