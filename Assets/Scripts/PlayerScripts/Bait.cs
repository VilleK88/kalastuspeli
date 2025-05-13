using Unity.Hierarchy;
using UnityEngine;

public class Bait : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Employer"))
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
            }
        }
    }
}