using UnityEngine;

public class KenshiCamera : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 20f;
    public float zoomSpeed = 200f;
    public float rotateSpeed = 5f;

    public float minZoom = 5f;
    public float maxZoom = 30f;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }

    void Update()
    {
        // Déplacement caméra (clavier)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        transform.position += Quaternion.Euler(0, transform.eulerAngles.y, 0) * move * moveSpeed * Time.deltaTime;

        // Rotation (clic molette)
        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotateSpeed, Space.World);
        }

        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        offset += offset.normalized * -scroll * zoomSpeed * Time.deltaTime;

        float currentZoom = offset.magnitude;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        offset = offset.normalized * currentZoom;

        // Follow léger
        transform.position = target.position + offset;
    }
}