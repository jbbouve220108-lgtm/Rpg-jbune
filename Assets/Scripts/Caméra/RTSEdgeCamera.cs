using UnityEngine;

public class TopDownEdgeCamera : MonoBehaviour
{
    [Header("Movement (screen edges)")]
    public float moveSpeed = 20f;
    public float edgeSize = 20f;

    [Header("Zoom")]
    public float zoomSpeed = 15f;
    public float minDistance = 10f;
    public float maxDistance = 50f;

    [Header("Tilt (mouse wheel drag)")]
    public float tiltSpeed = 3f;
    public float minTilt = 45f;
    public float maxTilt = 90f;

    private Camera cam;
    private Transform camTransform;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        camTransform = cam.transform;
    }

    void Update()
    {
        if (MerchantUI.Instance != null && MerchantUI.Instance.IsOpen())
            return;

        bool rotating = Input.GetMouseButton(2);

        if (!rotating)
            HandleEdgeMovement();

        HandleZoom();

        if (rotating)
            HandleTilt();
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

        transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }

    // 🔍 Zoom vers la souris
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.01f) return;

        if (!TryGetMouseGroundPoint(out Vector3 pivot))
            return;

        Vector3 dir = (camTransform.position - pivot).normalized;
        float distance = Vector3.Distance(camTransform.position, pivot);

        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        camTransform.position = pivot + dir * distance;
    }

    // 🔽 Inclinaison haut / bas uniquement
    void HandleTilt()
    {
        if (!TryGetMouseGroundPoint(out Vector3 pivot))
            return;

        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 right = camTransform.right;
        camTransform.RotateAround(
            pivot,
            right,
            -mouseY * tiltSpeed * 100f * Time.deltaTime
        );

        // Clamp inclinaison
        Vector3 euler = camTransform.eulerAngles;
        float x = euler.x > 180 ? euler.x - 360 : euler.x;
        x = Mathf.Clamp(x, minTilt, maxTilt);

        camTransform.eulerAngles = new Vector3(x, 0, 0);
    }

    // 🎯 Point au sol sous la souris
    bool TryGetMouseGroundPoint(out Vector3 point)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}