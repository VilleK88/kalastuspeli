using UnityEngine;
using UnityEngine.EventSystems;

public class Interact : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Marker marker = other.GetComponentInParent<Marker>();
        if(marker != null)
        {
            marker.EnableSFX();
            Debug.Log("Entered marker area: " + marker.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Marker marker = other.GetComponentInParent<Marker>();
        if(marker != null)
        {
            marker.DisableSFX();
            Debug.Log("Exited marker area: " + marker.gameObject.name);
        }
    }
}