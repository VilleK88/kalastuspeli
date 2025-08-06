using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 60, -29);
    private Quaternion fixedRotation;

    private void Start()
    {
        fixedRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = fixedRotation;
    }
}