using UnityEngine;

[CreateAssetMenu(fileName = "IndustryData", menuName = "Scriptable Objects/IndustryData")]
public class IndustryData : ScriptableObject
{
    public IndustryType industryType;
    public string displayName;
    public string finnishName;
    public GameObject prefabIcon;
}
