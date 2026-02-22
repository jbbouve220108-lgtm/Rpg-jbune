using UnityEngine;

public class TopDownEdgeCamera : MonoBehaviour
{
    [Header("Movement (screen edges only)")]
    public float moveSpeed = 20f;
    public float edgeSize = 20f;

    [Header("Zoom")]
    public float zoomSpeed = 200f;
    public float minHeight = 10f;
    public float maxHeight = 50f;

    [Header("Rotation")]
    public float rotationSpeed = 3f;

    private Camera cam;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        HandleEdgeMovement();
        HandleZoom();
        HandleRotation();
    }

    // 🟦 Déplacement par bords d’écran
    void HandleEdgeMovement()
    {
        Vector3 move = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x <= edgeSize) move.x -= 1;
        if (mousePos.x >= Screen.width - edgeSize) move.x += 1;
        if (mousePos.y <= edgeSize) move.z -= 1;
        if (mousePos.y >= Screen.height - edgeSize) move.z += 1;

        // déplacement relatif à la rotation actuelle
        Vector3 rotatedMove = Quaternion.Euler(0, transform.eulerAngles.y, 0) * move;
        transform.position += rotatedMove.normalized * moveSpeed * Time.deltaTime;
    }

    // 🔍 Zoom vertical
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 pos = cam.transform.localPosition;
            pos.y -= scroll * zoomSpeed * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
            cam.transform.localPosition = pos;
        }
    }

    // 🔄 Rotation avec molette maintenue
    void HandleRotation()
    {
        if (Input.GetMouseButton(2)) // clic molette
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotationSpeed * 100f * Time.deltaTime);
        }
    }
}