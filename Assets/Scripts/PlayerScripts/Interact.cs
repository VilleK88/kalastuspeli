using UnityEngine;

public class Interact : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Marker marker = other.GetComponentInParent<Marker>();
        if(marker != null)
            marker.EnableSFX();
    }

    private void OnTriggerExit(Collider other)
    {
        Marker marker = other.GetComponentInParent<Marker>();
        if(marker != null)
            marker.DisableSFX();
    }
}