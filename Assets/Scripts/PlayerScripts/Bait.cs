using UnityEngine;

public class Bait : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    private void OnCollisionEnter(Collision collision)
    {
        /*if(collision.gameObject.layer == LayerMask.NameToLayer("Employer"))
        {
            Transform hips = collision.transform.root.Find("mixamorig:Hips");
            if(hips != null)
            {
                Employer employer = hips.GetComponentInParent<Employer>();
                if(employer != null)
                {
                    employer.transform.SetParent(transform);
                    Animator anim = employer.GetComponent<Animator>();
                    if (anim != null) anim.enabled = false;
                }

                Rigidbody hipsRb = hips.GetComponent<Rigidbody>();
                if(hipsRb != null)
                {
                    FixedJoint joint = gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = hipsRb;
                }
            }
        }*/
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