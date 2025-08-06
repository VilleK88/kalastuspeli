using UnityEngine;

public class Bait : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Marker"))
        {
            if(rb != null)
            {
                rb.angularVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
    }
}