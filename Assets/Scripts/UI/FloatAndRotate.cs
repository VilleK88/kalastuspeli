using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float amplitude = 2f;
    [SerializeField] float frequency = 0.5f;
    float baseHeight = 8f;

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