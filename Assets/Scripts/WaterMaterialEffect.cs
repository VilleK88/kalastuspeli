using UnityEngine;

public class WaterMaterialEffect : MonoBehaviour
{
    public Vector2 speed = new Vector2(0.01f, 0.01f);
    Renderer rend;

    void Start() => rend = GetComponent<Renderer>();

    private void Update()
    {
        Vector2 offset = Time.time * speed;
        rend.material.SetTextureOffset("_BaseMap", offset);
    }
}