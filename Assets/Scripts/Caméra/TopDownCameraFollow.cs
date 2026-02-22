using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10f, -6f);
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (!target) return;

        // Position
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // Orientation : regarde le joueur
        transform.LookAt(target.position);
    }
}