using UnityEngine;

public class Interact : MonoBehaviour
{
    public float radius = 20;
    public Transform interactionTransform;
    public float heightOffset = 7;
    bool isFocus = false;
    Transform player;
    bool hasInteracted = false;
    float distance;

    private void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        //if (interactionTransform == null)
            //interactionTransform = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position + Vector3.up * heightOffset, radius);
    }
}