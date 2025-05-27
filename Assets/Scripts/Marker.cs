using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] GameObject circleObject;
    [SerializeField] ParticleSystem particleSystem;
    public bool canInteract;

    public void EnableSFX()
    {
        canInteract = true;
        particleSystem.Play();
    }

    public void DisableSFX()
    {
        canInteract = false;
        particleSystem.Stop();
    }

    public void StartInteraction()
    {
        if(canInteract)
        {
            Debug.Log("canInteract: " + canInteract);
        }
        else
        {
            Debug.Log("canInteract: " + canInteract);
        }
    }
}