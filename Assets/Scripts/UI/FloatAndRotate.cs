using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float amplitude = 2f;
    [SerializeField] float frequency = 0.5f;
    //float baseHeight = 8f;

    float baseHeightOffset = 10f;
    float baseHeight;
    float raycastDistance = 100f;

    private void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
            baseHeight = hit.point.y + baseHeightOffset;
        else
            baseHeight = transform.position.y;
    }

    private void Update()
    {
        FloatAndRotatePointer();
    }

    void FloatAndRotatePointer()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float floatOffset = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = new Vector3(transform.position.x, baseHeight + floatOffset, transform.position.z);
    }
}