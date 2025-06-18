using UnityEngine;

public class TextureColorProbe : MonoBehaviour
{
    [Header("Raycast Settigns")]
    [SerializeField] float maxDistance = 500f;

    [Header("Color Output")]
    public Color pickedColor;
    public string pickedHexCode;

    bool rayHit = false;
    Vector3 hitPoint;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //GetColor();

            Ray ray = new Ray(transform.position, transform.forward);

            if(Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                Debug.Log($"Raycast hit: {hit.collider.name}");

                Renderer renderer = hit.collider.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if(renderer != null && renderer.material != null && renderer.material.mainTexture != null && meshCollider != null)
                {
                    Texture2D texture = renderer.material.mainTexture as Texture2D;

                    Vector2 pixelUV = hit.textureCoord;
                    pixelUV.x *= texture.width;
                    pixelUV.y *= texture.height;

                    RenderTexture currentRT = RenderTexture.active;
                    RenderTexture tempRT = RenderTexture.GetTemporary(texture.width, texture.height, 0);
                    Graphics.Blit(texture, tempRT);
                    RenderTexture.active = tempRT;

                    Texture2D readableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
                    readableTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
                    readableTexture.Apply();

                    pickedColor = readableTexture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                    pickedHexCode = ColorUtility.ToHtmlStringRGB(pickedColor);

                    Debug.Log($"Picked Color: {pickedColor}, Hex: #{pickedHexCode}");

                    RenderTexture.ReleaseTemporary(tempRT);
                    RenderTexture.active = currentRT;

                    rayHit = true;
                    hitPoint = hit.point;
                }
                else
                {
                    Debug.LogWarning("Raycast hit an object without readable texture or MeshCollider.");
                    rayHit = false;
                }
            }
            else
            {
                Debug.Log("Raycast dit not hit anything.");
                rayHit = false;
            }


        }
    }

    void GetColor()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            Debug.Log($"Raycast hit: {hit.collider.name}");

            Renderer renderer = hit.collider.GetComponent<Renderer>();
            Texture texture = renderer?.material?.mainTexture;

            if(texture == null)
            {
                Debug.LogWarning("Material has no mainTexture.");
                return;
            }

            Debug.Log($"Texture type: {texture.GetType().Name}, name: {texture.name}");

            if(texture is Texture2D texture2D)
            {
                if(!texture2D.isReadable)
                {
                    Debug.LogWarning("Texture is not readable!");
                    return;
                }

                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= texture2D.width;
                pixelUV.y *= texture2D.height;

                pickedColor = texture2D.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                pickedHexCode = ColorUtility.ToHtmlStringRGB(pickedColor);

                Debug.Log($"Picked Color: {pickedColor}, Hex: {pickedHexCode}");

                rayHit = true;
                hitPoint = hit.point;
            }
            else
            {
                Debug.LogWarning("Texture is not a Texture2D. It is: " + texture.GetType().Name);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * maxDistance);

        if(rayHit)
        {
            Gizmos.color = pickedColor;
            Gizmos.DrawSphere(hitPoint, 0.1f);
        }
    }
}