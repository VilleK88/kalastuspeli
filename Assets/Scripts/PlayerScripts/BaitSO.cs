using UnityEngine;

[CreateAssetMenu(fileName = "Bait", menuName = "Scriptable Objects/Bait")]
public class BaitSO : ScriptableObject
{
    new public string name = "New lure";
    public GameObject prefab;
}