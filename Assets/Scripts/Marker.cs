using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] GameObject circleObject;
    [SerializeField] ParticleSystem particleSystem;

    private void Start()
    {
        //particleSystem = circleObject.GetComponent<ParticleSystem>();
    }

    public void EnableSFX()
    {
        particleSystem.Play();
    }

    public void DisableSFX()
    {
        particleSystem.Stop();
    }
}