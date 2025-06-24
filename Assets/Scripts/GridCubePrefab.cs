using Unity.VisualScripting;
using UnityEngine;

public class GridCubePrefab : MonoBehaviour
{
    [Header("Obstacle Detection")]
    [SerializeField] LayerMask obstacleMask;
    public float scaleX, scaleY, scaleZ;

    [Header("Water Detection")]
    [SerializeField] float raycastDistance = 100f;
    [SerializeField] Color targetWaterColor = new Color32(118, 207, 239, 255);
    [SerializeField] float colorTolerance = 10f;

    private void Start()
    {
        Vector3 checkAreaSize = new Vector3(scaleX, scaleY, scaleZ);
        Vector3 center = transform.position + Vector3.up * (checkAreaSize.y / 2);

        if (Physics.CheckBox(center, checkAreaSize / 2, Quaternion.identity, obstacleMask))
            gameObject.SetActive(false);

        WaterCheckByColor();
    }

    void WaterCheckByColor()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if(Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            Texture texture = renderer?.material?.mainTexture;

            if(texture is Texture2D texture2D && texture2D.isReadable)
            {
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= texture2D.width;
                pixelUV.y *= texture2D.height;

                Color detectedColor = texture2D.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                if (IsColorMatch(detectedColor, targetWaterColor, colorTolerance))
                    gameObject.SetActive(false);
            }
        }
    }

    bool IsColorMatch(Color a, Color b, float tolerance)
    {
        float rDiff = Mathf.Abs(a.r - b.r) * 255;
        float gDiff = Mathf.Abs(a.g - b.g) * 255;
        float bDiff = Mathf.Abs(a.b - b.b) * 255;

        return rDiff < tolerance && gDiff < tolerance && bDiff < tolerance;
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(scaleX, scaleY, scaleZ));
    }*/
}