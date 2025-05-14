using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}