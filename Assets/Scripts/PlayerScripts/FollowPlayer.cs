using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    //public float distance = 6f;
    //public float height = 3f;
    public float smoothSpeed = 5f;
    //public Vector3 lookOffset = new Vector3(0, 1.5f, 2f);
    public Vector3 offset = new Vector3(0, 10, -10);
    private Quaternion fixedRotation;

    private void Start()
    {
        fixedRotation = transform.rotation;
    }

    /*private void FixedUpdate()
    {
        // Kamera pelaajan taakse ja ylös (vain Y-rotaatio mukaan)
        Vector3 behindPlayer = Quaternion.Euler(0, player.eulerAngles.y, 0) * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = player.position + behindPlayer + new Vector3(0, height, 0);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Kamera katsoo pelaajan etupuolelle
        Vector3 lookAtPosition = player.position + player.forward * lookOffset.z + Vector3.up * lookOffset.y;
        transform.LookAt(lookAtPosition);
    }*/

    private void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = fixedRotation;
    }
}