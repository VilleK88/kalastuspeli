using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public float speed = 0.3f;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed);
    }
}