using UnityEngine;

[CreateAssetMenu(fileName = "GoldConfig", menuName = "Config/Gold")]
public class SO_GoldConfig : ScriptableObject
{
    [Header("골드 설정")]
    public int minGold = 0;
    public int maxGold = 999999;
    public int startingGold = 1000;
}