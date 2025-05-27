using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] Marker marker;
    public void OnMouseDown()
    {
        marker.StartInteraction();
    }
}