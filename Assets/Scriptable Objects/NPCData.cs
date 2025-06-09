using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Scriptable Objects/NPCData")]
public class NPCData : ScriptableObject
{
    public NPCType npcType;
    public string displayName;
    public string finnishName;
    public GameObject npcPrefab;
}